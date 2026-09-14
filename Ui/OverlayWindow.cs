using System.Numerics;
using BeastmasterAssist.Combat;
using BeastmasterAssist.Data;
using BeastmasterAssist.Tracking;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace BeastmasterAssist.Ui;

public sealed class OverlayWindow : Window
{
    private readonly Configuration config;
    private readonly CombatState state;
    private readonly RotationAdvisor rotation;
    private readonly ReactionAdvisor reactions;
    private readonly MitigationAdvisor mitigation;
    private readonly CaptureTracker capture;
    private DateTime nextSnapshotAt = DateTime.MinValue;
    private string gaugeText = "";
    private string captureText = "";
    private List<Advice> rotationSnapshot = [];
    private List<Advice> reactionSnapshot = [];
    private List<Advice> mitigationSnapshot = [];

    public OverlayWindow(Configuration config, CombatState state, RotationAdvisor rotation, ReactionAdvisor reactions, MitigationAdvisor mitigation, CaptureTracker capture)
        : base("Beastmaster Assist##BeastmasterAssistOverlay", ImGuiWindowFlags.NoCollapse)
    {
        this.config = config;
        this.state = state;
        this.rotation = rotation;
        this.reactions = reactions;
        this.mitigation = mitigation;
        this.capture = capture;
        RespectCloseHotkey = false;
        Size = new Vector2(420, 0);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void PreDraw()
    {
        Flags = ImGuiWindowFlags.NoCollapse;
        if (config.LockOverlay) Flags |= ImGuiWindowFlags.NoMove;
        ImGui.SetNextWindowBgAlpha(0.94f);
    }

    public override void Draw()
    {
        UiText.Sync(config);
        RefreshSnapshot();
        ImGui.SetWindowFontScale(Math.Clamp(config.OverlayScale * 1.28f, 1.0f, 2.0f));
        ImGui.TextColored(new Vector4(1f, .78f, .25f, 1f), "BEASTMASTER ASSIST");
        ImGui.SameLine();
        ImGui.TextDisabled(state.Player is not null ? $"Lv{state.Level}" : "No player");
        ImGui.SameLine();
        if (ImGui.Button(UiText.HideOverlay)) UiNavigator.ToggleOverlay?.Invoke();
        ImGui.TextDisabled(gaugeText);
        ImGui.Separator();
        DrawSection(UiText.Rotation, rotationSnapshot, 3, new Vector4(.95f,.95f,.95f,1f));
        if (config.ShowReactions) DrawSection(UiText.Reactions, reactionSnapshot, 2, new Vector4(1f,.58f,.34f,1f));
        if (config.ShowMitigation) DrawSection(UiText.Mitigation, mitigationSnapshot, 2, new Vector4(.4f,.83f,1f,1f));
        if (config.ShowCaptureHud && !string.IsNullOrEmpty(captureText)) { ImGui.Separator(); ImGui.TextColored(new Vector4(.6f,1f,.48f,1f), UiText.Capture); ImGui.TextWrapped(captureText); }
        ImGui.SetWindowFontScale(1f);
    }

    private void RefreshSnapshot()
    {
        if (DateTime.UtcNow < nextSnapshotAt) return;
        nextSnapshotAt = DateTime.UtcNow.AddMilliseconds(750);
        gaugeText = $"TP {state.PlayerTp}/250    Familiar {state.FamiliarTp}/250    {state.ActiveKinship}";
        captureText = capture.CapturePrompt;
        rotationSnapshot = rotation.Advise(state);
        reactionSnapshot = reactions.Advise(state);
        mitigationSnapshot = mitigation.Advise(state);
    }

    private static void DrawSection(string title, IEnumerable<Advice> advice, int max, Vector4 colour)
    {
        var list = advice.Take(max).ToList();
        if (list.Count == 0) return;
        ImGui.TextColored(colour, title);
        foreach (var item in list) { ImGui.TextColored(colour, $"▶ {item.Action}"); ImGui.SameLine(); ImGui.TextDisabled(item.Reason); }
    }
}
