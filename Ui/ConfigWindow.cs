using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

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
        UiText.Sync(config);
        Nav();
        ImGui.Separator();

        ImGui.TextColored(new System.Numerics.Vector4(1f, .78f, .25f, 1f), UiText.Language);
        var polish = config.PreferPolishUi;
        if (ImGui.RadioButton(UiText.PolishUi, polish)) { config.PreferPolishUi = true; config.Save(); }
        ImGui.SameLine();
        if (ImGui.RadioButton(UiText.EnglishUi, !polish)) { config.PreferPolishUi = false; config.Save(); }
        UiText.Sync(config);
        ImGui.Separator();
        var compact = config.CompactOverlay;
if (ImGui.Checkbox(UiText.T("Tryb kompaktowy (mini overlay)", "Compact overlay mode"), ref compact))
{
    config.CompactOverlay = compact;
    config.Save();
}

        var ov = config.OverlayEnabled;
        if (ImGui.Checkbox(UiText.Overlay, ref ov)) { config.OverlayEnabled = ov; config.Save(); }
        var only = config.ShowOnlyOnBeastmaster;
        if (ImGui.Checkbox(UiText.ShowOnlyOnBst, ref only)) { config.ShowOnlyOnBeastmaster = only; config.Save(); }
        var rot = config.ShowRotation;
        if (ImGui.Checkbox(UiText.ShowRotationOpt, ref rot)) { config.ShowRotation = rot; config.Save(); }
        var rec = config.ShowReactions;
        if (ImGui.Checkbox(UiText.ShowReactionsOpt, ref rec)) { config.ShowReactions = rec; config.Save(); }
        var mit = config.ShowMitigation;
        if (ImGui.Checkbox(UiText.ShowMitigationOpt, ref mit)) { config.ShowMitigation = mit; config.Save(); }
        var cap = config.ShowCaptureHud;
        if (ImGui.Checkbox(UiText.ShowCaptureOpt, ref cap)) { config.ShowCaptureHud = cap; config.Save(); }
        var markers = config.ShowNameplateMarkers;
        if (ImGui.Checkbox(UiText.T("Pokazuj ikonki nad mobami do zlapania", "Show nameplate markers on catchable mobs"), ref markers)) { config.ShowNameplateMarkers = markers; config.Save(); }
        var lockOv = config.LockOverlay;
        if (ImGui.Checkbox(UiText.LockOverlay, ref lockOv)) { config.LockOverlay = lockOv; config.Save(); }
        var hold = config.HoldTpForUniversality;
        if (ImGui.Checkbox(UiText.HoldTpOpt, ref hold)) { config.HoldTpForUniversality = hold; config.Save(); }
        var notify = config.ChatCaptureNotify;
        if (ImGui.Checkbox(UiText.ChatNotifyOpt, ref notify)) { config.ChatCaptureNotify = notify; config.Save(); }
        var scale = config.OverlayScale;
        if (ImGui.SliderFloat(UiText.OverlayScale, ref scale, .8f, 1.6f)) { config.OverlayScale = scale; config.Save(); }

        ImGui.Separator();
        ImGui.TextColored(new System.Numerics.Vector4(1f, .78f, .25f, 1f), UiText.CommandHelp);
        ImGui.TextWrapped(UiText.Commands);
        ImGui.TextDisabled(UiText.PluginDisclaimer);
    }

    private void Nav()
    {
        if (ImGui.Button(UiText.Bestiary)) UiNavigator.OpenBestiary?.Invoke();
        ImGui.SameLine();
        if (ImGui.Button(UiText.Progress)) UiNavigator.OpenProgress?.Invoke();
        ImGui.SameLine();
        if (ImGui.Button(UiText.Settings)) IsOpen = true;
        ImGui.SameLine();
        if (ImGui.Button(UiText.Close)) IsOpen = false;
    }
}
