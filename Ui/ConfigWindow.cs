using System.Numerics;
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

    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 8f);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(14, 12));
    }

    public override void PostDraw() => ImGui.PopStyleVar(3);

    public override void Draw()
    {
        UiText.Sync(config);

        var scale = Math.Clamp(config.OverlayScale, 0.80f, 1.60f);
        ImGui.SetWindowFontScale(scale);

        Nav();
        ImGui.Separator();
        ImGui.Spacing();

        SectionHeader(UiText.Language);
        var polish = config.PreferPolishUi;
        if (ImGui.RadioButton(UiText.PolishUi, polish)) { config.PreferPolishUi = true; config.Save(); }
        ImGui.SameLine();
        if (ImGui.RadioButton(UiText.EnglishUi, !polish)) { config.PreferPolishUi = false; config.Save(); }
        UiText.Sync(config);
        ImGui.Spacing();

        SectionHeader(UiText.Overlay);
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
        var tp = config.ShowTpGauges;
        if (ImGui.Checkbox(UiText.T("Pokazuj paski TP", "Show TP gauges"), ref tp)) { config.ShowTpGauges = tp; config.Save(); }
        var lvl = config.ShowLeveling;
        if (ImGui.Checkbox(UiText.T("Pokazuj sugestie levelowania", "Show leveling suggestion"), ref lvl)) { config.ShowLeveling = lvl; config.Save(); }
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

        ImGui.Spacing();
        var sliderScale = config.OverlayScale;
        ImGui.TextColored(new Vector4(0.6f, 0.85f, 1f, 1f), $"{UiText.OverlayScale}: {sliderScale:0.00}x");
        if (ImGui.SliderFloat("##overlayscale", ref sliderScale, .8f, 1.6f)) { config.OverlayScale = sliderScale; config.Save(); }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        SectionHeader(UiText.CommandHelp);
        ImGui.TextWrapped(UiText.Commands);
        ImGui.TextDisabled(UiText.PluginDisclaimer);

        ImGui.SetWindowFontScale(1f);
    }

    private static void SectionHeader(string label)
    {
        ImGui.TextColored(new Vector4(1f, .78f, .25f, 1f), label);
        ImGui.Spacing();
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
