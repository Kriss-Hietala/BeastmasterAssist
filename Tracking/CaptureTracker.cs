using BeastmasterAssist.Combat;
using BeastmasterAssist.Data;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;

namespace BeastmasterAssist.Tracking;

public sealed class CaptureTracker
{
    private readonly Configuration config;
    private readonly IChatGui chat;
    private readonly IObjectTable objects;

    public BeastEntry? NearbyHint { get; private set; }
    public string CapturePrompt { get; private set; } = "";

    public CaptureTracker(Configuration config, GameData data, IChatGui chat, IClientState clientState, IObjectTable objects, IPluginLog log)
    {
        this.config = config;
        this.chat = chat;
        this.objects = objects;
    }

    public void Attach() => chat.ChatMessage += OnChat;
    public void Detach() => chat.ChatMessage -= OnChat;
    public int CapturedCount => config.CapturedBeastIds.Count;
    public bool Has(int id) => config.CapturedBeastIds.Contains(id);

    public void Toggle(int id)
    {
        if (!config.CapturedBeastIds.Add(id)) config.CapturedBeastIds.Remove(id);
        config.Save();
    }

    public void MarkCaptured(int id)
    {
        if (config.CapturedBeastIds.Add(id))
        {
            config.Save();
            if (config.ChatCaptureNotify)
                chat.Print($"[BST] Pact: {BestiaryCatalog.Get(id)?.Name} ({CapturedCount}/50)");
        }
    }

    public void Tick(CombatState state)
    {
        NearbyHint = null;
        foreach (var obj in objects)
        {
            if (obj is not Dalamud.Game.ClientState.Objects.Types.IBattleNpc npc) continue;
            var match = MatchByMonster(npc.Name.TextValue);
            if (match is not null && !Has(match.Id))
            {
                NearbyHint = match;
                break;
            }
        }

        if (state.Target is null)
        {
            CapturePrompt = NearbyHint is null ? "" : $"{NearbyHint.Name}: {NearbyHint.Location}";
            return;
        }

        var b = MatchByMonster(state.Target.Name.TextValue);
        if (b is null) { CapturePrompt = Loc.T("Gauge na celu.", "Gauge the target."); return; }
        if (Has(b.Id)) { CapturePrompt = Loc.T($"{b.Name} juz w bestiariuszu.", $"{b.Name} already captured."); return; }
        CapturePrompt = Loc.T($"{b.Name}: Gauge 30y, Capture 10y. HP {state.TargetHpPct:P0}", $"{b.Name}: Gauge 30y, Capture 10y. HP {state.TargetHpPct:P0}");
    }

    public BeastEntry? MatchByMonster(string name) =>
        BestiaryCatalog.All.FirstOrDefault(b =>
            name.Contains(b.Monster, StringComparison.OrdinalIgnoreCase) ||
            name.Contains(b.Name, StringComparison.OrdinalIgnoreCase));

    private void OnChat(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool handled)
    {
        var text = message.TextValue;
        if (!(text.Contains("pact", StringComparison.OrdinalIgnoreCase) || text.Contains("Bestiary", StringComparison.OrdinalIgnoreCase)))
            return;
        foreach (var b in BestiaryCatalog.All)
        {
            if (text.Contains(b.Name, StringComparison.OrdinalIgnoreCase) || text.Contains(b.Monster, StringComparison.OrdinalIgnoreCase))
            {
                MarkCaptured(b.Id);
                break;
            }
        }
    }
}
