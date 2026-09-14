using BeastmasterAssist.Combat;
using BeastmasterAssist.Data;
using BeastmasterAssist.Tracking;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

namespace BeastmasterAssist.Ui;

public sealed class OverlayWindow : Window
{
    private readonly Configuration config;
    private readonly CombatState state;
    private readonly RotationAdvisor rotation;
    private readonly ReactionAdvisor reactions;
    private readonly MitigationAdvisor mitigation;
    private readonly CaptureTracker capture;

    public OverlayWindow(Configuration config, CombatState state, RotationAdvisor rotation, ReactionAdvisor reactions, MitigationAdvisor mitigation, CaptureTracker capture)
        : base("BST##BeastmasterAssistOverlay", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize)
    {
        this.config = config;
        this.state = state;
        this.rotation = rotation;
        this.reactions = reactions;
        this.mitigation = mitigation;
        this.capture = capture;
        RespectCloseHotkey = false;
    }

    public override void Draw()
    {
        Loc.Polish = config.PreferPolishUi;
        ImGui.TextUnformatted("Beastmaster Assist");
        ImGui.TextDisabled($"TP {state.PlayerTp}/250  Fam {state.FamiliarTp}/250  {state.ActiveKinship}");
        if (config.ShowRotation)
            foreach (var a in rotation.Advise(state).Take(4))
                ImGui.BulletText($"{a.Action}: {a.Reason}");
        if (config.ShowReactions)
            foreach (var a in reactions.Advise(state).Take(3))
                ImGui.BulletText($"{a.Action}: {a.Reason}");
        if (config.ShowMitigation)
            foreach (var a in mitigation.Advise(state).Take(3))
                ImGui.BulletText($"{a.Action}: {a.Reason}");
        if (config.ShowCaptureHud && !string.IsNullOrEmpty(capture.CapturePrompt))
        {
            ImGui.Separator();
            ImGui.TextWrapped(capture.CapturePrompt);
        }
    }
}
