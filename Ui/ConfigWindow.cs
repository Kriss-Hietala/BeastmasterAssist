using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

namespace BeastmasterAssist.Ui;

public sealed class ConfigWindow : Window
{
    private readonly Configuration config;

    public ConfigWindow(Configuration config) : base("BST Assist Config##BeastmasterAssistCfg")
    {
        this.config = config;
    }

    public override void Draw()
    {
        var pl = config.PreferPolishUi;
        if (ImGui.Checkbox("Polski UI", ref pl)) { config.PreferPolishUi = pl; config.Save(); }
        var ov = config.OverlayEnabled;
        if (ImGui.Checkbox("Overlay", ref ov)) { config.OverlayEnabled = ov; config.Save(); }
        var only = config.ShowOnlyOnBeastmaster;
        if (ImGui.Checkbox("Tylko na BST", ref only)) { config.ShowOnlyOnBeastmaster = only; config.Save(); }
        var rot = config.ShowRotation;
        if (ImGui.Checkbox("Rotacja", ref rot)) { config.ShowRotation = rot; config.Save(); }
        var rec = config.ShowReactions;
        if (ImGui.Checkbox("Reakcje", ref rec)) { config.ShowReactions = rec; config.Save(); }
        var mit = config.ShowMitigation;
        if (ImGui.Checkbox("Mitygacja", ref mit)) { config.ShowMitigation = mit; config.Save(); }
        var cap = config.ShowCaptureHud;
        if (ImGui.Checkbox("HUD lapania", ref cap)) { config.ShowCaptureHud = cap; config.Save(); }
        var lockOv = config.LockOverlay;
        if (ImGui.Checkbox("Zablokuj overlay", ref lockOv)) { config.LockOverlay = lockOv; config.Save(); }
        var hold = config.HoldTpForUniversality;
        if (ImGui.Checkbox("Trzymaj TP na Universality", ref hold)) { config.HoldTpForUniversality = hold; config.Save(); }
        var notify = config.ChatCaptureNotify;
        if (ImGui.Checkbox("Czat przy pakcie", ref notify)) { config.ChatCaptureNotify = notify; config.Save(); }
        var scale = config.OverlayScale;
        if (ImGui.SliderFloat("Skala overlay", ref scale, 0.8f, 1.6f)) { config.OverlayScale = scale; config.Save(); }
        ImGui.Separator();
        ImGui.TextWrapped("/bstassist  |  /bestia  |  /bstassist overlay|bestiary|progress|config|debug");
        ImGui.TextDisabled("Plugin nie wciska skilli. To nakladka i tracker.");
    }
}
