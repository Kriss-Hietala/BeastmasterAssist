using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace BeastmasterAssist.Data;

public sealed class GameData
{
    private readonly IDataManager data;
    private readonly IPluginLog log;

    public uint JobId { get; private set; }
    public int ResolvedActionCount => Actions.Count;
    public readonly Dictionary<string, uint> Actions = new(StringComparer.OrdinalIgnoreCase);
    public readonly Dictionary<string, uint> Statuses = new(StringComparer.OrdinalIgnoreCase);
    public readonly Dictionary<string, uint> Items = new(StringComparer.OrdinalIgnoreCase);

    public GameData(IDataManager data, IPluginLog log)
    {
        this.data = data;
        this.log = log;
    }

    public void Resolve()
    {
        ResolveJob();
        ResolveActions();
        ResolveStatuses();
        ResolveItems();
        ResolveAchievements();
    }

    public bool IsBeastmaster(IPlayerCharacter? player) =>
        player is not null && JobId != 0 && player.ClassJob.RowId == JobId;

    public uint Action(params string[] names)
    {
        foreach (var n in names)
            if (Actions.TryGetValue(n, out var id))
                return id;
        return 0;
    }

    // Uzywane przez integracje z Rotation Solver Reborn: zamienia surowe ID akcji
    // otrzymane przez IPC na nazwe i ID ikony do wyswietlenia w overlayu.
    public (string Name, uint IconId)? ResolveAction(uint actionId)
    {
        if (actionId == 0) return null;
        var sheet = data.GetExcelSheet<Lumina.Excel.Sheets.Action>();
        if (sheet is null) return null;
        var row = sheet.GetRowOrDefault(actionId);
        if (row is null) return null;
        var name = row.Value.Name.ToString();
        if (string.IsNullOrWhiteSpace(name)) return null;
        return (name, (uint)row.Value.Icon);
    }

    public void Dump(IPluginLog pluginLog)
    {
        pluginLog.Info("BST JobId={0}", JobId);
        foreach (var kv in Actions.OrderBy(k => k.Value))
            pluginLog.Info("  action {0} = {1}", kv.Value, kv.Key);
        foreach (var kv in Statuses.OrderBy(k => k.Value))
            pluginLog.Info("  status {0} = {1}", kv.Value, kv.Key);
    }

    private void ResolveJob()
    {
        var sheet = data.GetExcelSheet<ClassJob>();
        if (sheet is null) return;
        foreach (var row in sheet)
        {
            var abbr = row.Abbreviation.ToString();
            var name = row.Name.ToString();
            if (abbr.Equals("BST", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Beastmaster", StringComparison.OrdinalIgnoreCase))
            {
                JobId = row.RowId;
                log.Info("Resolved Beastmaster ClassJob {0} ({1} / {2})", JobId, abbr, name);
                return;
            }
        }
        log.Warning("Beastmaster ClassJob not found.");
    }

    private void ResolveActions()
    {
        var sheet = data.GetExcelSheet<Lumina.Excel.Sheets.Action>();
        if (sheet is null) return;
        foreach (var row in sheet)
        {
            if (JobId != 0 && row.ClassJob.RowId != JobId) continue;
            var name = row.Name.ToString();
            if (string.IsNullOrWhiteSpace(name)) continue;
            if (JobId != 0 && row.ClassJob.RowId == JobId)
                Actions[name] = row.RowId;
        }
        foreach (var n in new[] { "Smash Axe", "Capture", "First Battlehorn", "Gauge", "Axeblade Bite", "Avalanche Axe", "Parting Blow", "Mistral Axe", "Trick", "Second Battlehorn", "Shieldsplitter", "Spinning Axe", "Gale Axe", "Tempered Release", "Third Battlehorn", "Borrow", "Beast Mode", "Shield Charge", "Rally", "Rallying Cheer", "Brutal Rage", "Hawkish Talons", "Risen Fall", "Calamity", "Snarl", "Challenge" })
            Actions.TryAdd(n, 0);
    }

    private void ResolveStatuses()
    {
        var sheet = data.GetExcelSheet<Status>();
        if (sheet is null) return;
        string[] wanted =
        [
            "One with Nature", "Interest Captured", "Lingering Vantage", "Beastskin", "Vileskin", "Scaleskin",
            "Sunstrider", "Moonstalker", "Mastered Instinct", "Natural Instinct",
            "Beast Kinship", "Vile Kinship", "Cloud Kinship", "Seed Kinship", "Wave Kinship", "Scale Kinship", "Soul Kinship", "Ash Kinship",
            "Heart of Rampant", "Heart of Durant", "Heart of Eldritch", "Heart of Volant", "Cover"
        ];
        foreach (var row in sheet)
        {
            var name = row.Name.ToString();
            if (string.IsNullOrWhiteSpace(name)) continue;
            foreach (var w in wanted)
                if (name.Contains(w, StringComparison.OrdinalIgnoreCase))
                    Statuses.TryAdd(w, row.RowId);
        }
    }

    // Rozwiazuje ID przedmiotow dla remnantow Universality oraz calego
    // ekwipunku z ProgressCatalog.ExclusiveGear (pomijajac Set/Mount, ktore nie
    // maja pojedynczego trwalego ID mozliwego do wykrycia w ekwipunku).
    // Dwuprzebiegowo: najpierw dokladne dopasowanie nazwy (unika kolizji typu
    // "Guttler" bedacego podciagiem "Guttler Unleashed"), potem "contains"
    // jako fallback dla pozycji nieznalezionych dokladnie.
    private void ResolveItems()
    {
        var sheet = data.GetExcelSheet<Item>();
        if (sheet is null) return;

        var wanted = new List<string> { "Faded Remnant of Resilience", "Bright Remnant of Resilience" };
        foreach (var piece in ProgressCatalog.ExclusiveGear)
            if (piece.Slot != "Set" && piece.Slot != "Mount")
                wanted.Add(piece.Name);

        foreach (var row in sheet)
        {
            var name = row.Name.ToString();
            if (string.IsNullOrWhiteSpace(name)) continue;
            foreach (var w in wanted)
                if (name.Equals(w, StringComparison.OrdinalIgnoreCase))
                    Items.TryAdd(w, row.RowId);
        }

        foreach (var row in sheet)
        {
            var name = row.Name.ToString();
            if (string.IsNullOrWhiteSpace(name)) continue;
            foreach (var w in wanted)
                if (!Items.ContainsKey(w) && name.Contains(w, StringComparison.OrdinalIgnoreCase))
                    Items.TryAdd(w, row.RowId);
        }
    }

    private void ResolveAchievements()
    {
        var sheet = data.GetExcelSheet<Achievement>();
        if (sheet is null) return;
        foreach (var def in ProgressCatalog.Achievements)
        {
            foreach (var row in sheet)
            {
                var name = row.Name.ToString();
                if (name.Contains(def.Name, StringComparison.OrdinalIgnoreCase))
                {
                    def.ResolvedId = row.RowId;
                    break;
                }
            }
        }
    }
}
