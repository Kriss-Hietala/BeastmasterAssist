using Dalamud.Configuration;
using Dalamud.Plugin;


namespace BeastmasterAssist;


[Serializable]
public sealed class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public bool OverlayEnabled { get; set; } = true;
    public bool CompactOverlay { get; set; } = false; // Przelacznik trybu
    public bool ShowRotation { get; set; } = true;
    public bool ShowReactions { get; set; } = true;
    public bool ShowMitigation { get; set; } = true;
    public bool ShowCaptureHud { get; set; } = true;
    public bool ShowNameplateMarkers { get; set; } = true;
    public bool ShowOnlyOnBeastmaster { get; set; } = true;
    public bool ShowTpGauges { get; set; } = true;
    public bool ShowLeveling { get; set; } = true;
    public bool LockOverlay { get; set; }
    public float OverlayScale { get; set; } = 1f;
    public bool ChatCaptureNotify { get; set; } = true;
    public bool PreferPolishUi { get; set; } = true;
    public bool HoldTpForUniversality { get; set; } = true;


    public HashSet<int> CapturedBeastIds { get; set; } = [];
    public HashSet<int> Rank25BeastIds { get; set; } = [];
    public Dictionary<int, int> BeastRanks { get; set; } = [];
    public HashSet<string> CompletedQuestKeys { get; set; } = [];
    public HashSet<string> OwnedGearKeys { get; set; } = [];


    public int BrightRemnants { get; set; }
    public int FadedRemnants { get; set; }


    [NonSerialized] private IDalamudPluginInterface? pluginInterface;
    public void Initialize(IDalamudPluginInterface pi) => pluginInterface = pi;
    public void Save() => pluginInterface?.SavePluginConfig(this);
}
