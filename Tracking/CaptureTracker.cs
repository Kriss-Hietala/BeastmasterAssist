using BeastmasterAssist.Combat;
using BeastmasterAssist.Data;
using Dalamud.Game.Chat;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using System.Text.RegularExpressions;

namespace BeastmasterAssist.Tracking;

public sealed class CaptureTracker
{
    private readonly Configuration config;
    private readonly IChatGui chat;
    private readonly ICondition condition;
    private readonly IObjectTable objects;

    public BeastEntry? NearbyHint { get; private set; }
    public string CapturePrompt { get; private set; } = "";

    public CaptureTracker(Configuration config, GameData data, IChatGui chat, IClientState clientState, ICondition condition, IObjectTable objects, IPluginLog log)
    {
        this.config = config;
        this.chat = chat;
        this.condition = condition;
        this.objects = objects;
    }

    public void Attach() => chat.ChatMessage += OnChat;
    public void Detach() => chat.ChatMessage -= OnChat;
    public int CapturedCount => config.CapturedBeastIds.Count;
    public bool Has(int id) => config.CapturedBeastIds.Contains(id);

    // Niektore bestie (Duty=true) sa lapane wylacznie wewnatrz konkretnej duty
    // (np. Antling w Cutter's Cry). Poza duty ich nazwa handlowa czesto pokrywa
    // sie z niepowiazanymi mobami overworldowymi (np. "Antling Worker"),
    // wiec dopasowanie musi byc wylaczone poza instancja.
    private bool InDuty =>
        condition[ConditionFlag.BoundByDuty] ||
        condition[ConditionFlag.BoundByDuty56] ||
        condition[ConditionFlag.BoundByDuty95];

    public void Toggle(int id)
    {
        if (!config.CapturedBeastIds.Add(id))
            config.CapturedBeastIds.Remove(id);
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

    // Dopasowanie po calych slowach (regex \b), nie po dowolnym podciagu -
    // zwykly string.Contains lapal falszywe trafienia typu "Coeurl" wewnatrz
    // "Coeurlclaw Hunter/Poacher" (Sylphowie z plemienia, nie bestie do
    // zlapania), bo "Coeurl" jest tam fragmentem innego, dluzszego slowa bez
    // spacji/granicy miedzy nimi. WordMatches wymaga, zeby dopasowany fragment
    // zaczynal i konczyl sie na granicy slowa w nazwie NPC-a.
    public BeastEntry? MatchByMonster(string name) =>
        BestiaryCatalog.All.FirstOrDefault(b =>
            (!b.Duty || InDuty) &&
            (WordMatches(name, b.Monster) || WordMatches(name, b.Name)));

    private static bool WordMatches(string haystack, string needle)
    {
        if (string.IsNullOrWhiteSpace(needle)) return false;
        return Regex.IsMatch(haystack, $@"\b{Regex.Escape(needle)}\b", RegexOptions.IgnoreCase);
    }

    // Dalamud v15+: IChatGui.ChatMessage przyjmuje teraz pojedynczy obiekt
    // IHandleableChatMessage zamiast starego 5-parametrowego delegata z
    // ref SeString/ref bool. Tekst wiadomosci czytamy z Message.TextValue.
    private void OnChat(IHandleableChatMessage chatMessage)
    {
        var text = chatMessage.Message.TextValue;
        if (!(text.Contains("pact", StringComparison.OrdinalIgnoreCase) || text.Contains("Bestiary", StringComparison.OrdinalIgnoreCase)))
            return;
        foreach (var b in BestiaryCatalog.All)
        {
            if (WordMatches(text, b.Name) || WordMatches(text, b.Monster))
            {
                MarkCaptured(b.Id);
                break;
            }
        }
    }
}
