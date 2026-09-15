using System.Numerics;
using BeastmasterAssist.Data;
using BeastmasterAssist.Tracking;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace BeastmasterAssist.Ui;

public sealed class BestiaryWindow : Window
{
    private readonly Configuration config;
    private readonly CaptureTracker capture;
    private string filter = "";
    private int tab;
    private int classFilter;
    private int selectedId = 1;

    public BestiaryWindow(Configuration config, CaptureTracker capture, GameData data)
        : base("Master's Bestiary##BeastmasterAssistBestiary")
    {
        this.config = config;
        this.capture = capture;
        Size = new Vector2(960, 620);
        SizeCondition = ImGuiCond.FirstUseEver;
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

        ImGui.TextColored(new Vector4(1f, .78f, .25f, 1f), UiText.Bestiary);
        ImGui.SameLine();
        var barHeight = MathF.Max(18f, ImGui.GetTextLineHeight() + ImGui.GetStyle().FramePadding.Y * 2f);
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new Vector4(1f, .78f, .25f, 1f));
        ImGui.ProgressBar(capture.CapturedCount / 50f, new Vector2(220, barHeight), UiText.CapturedCount(capture.CapturedCount));
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGui.TextDisabled(UiText.MissingCount(BestiaryCatalog.All.Count - capture.CapturedCount));

        ImGui.Spacing();
        Tabs();
        ImGui.InputText(UiText.Search, ref filter, 64);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(150);

        // Podwojny terminator null (\0\0) wymagany przez specyfikacje ImGui Combo dla pojedynczego bufora opcji
        ImGui.Combo(UiText.Filters, ref classFilter, "All\0Beastkin\0Vilekin\0Cloudkin\0Seedkin\0Wavekin\0Scalekin\0Soulkin\0Ashkin\0\0");
        ImGui.Separator();

        if (ImGui.BeginTable("bst-layout", 2, ImGuiTableFlags.SizingStretchProp))
        {
            ImGui.TableSetupColumn("list", ImGuiTableColumnFlags.WidthStretch, .48f);
            ImGui.TableSetupColumn("details", ImGuiTableColumnFlags.WidthStretch, .52f);
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            DrawList();
            ImGui.TableNextColumn();
            DrawDetails();
            ImGui.EndTable();
        }

        ImGui.SetWindowFontScale(1f);
    }

    private void Tabs()
    {
        DrawTabButton(UiText.All, 0);
        ImGui.SameLine();
        DrawTabButton(UiText.Missing, 1);
        ImGui.SameLine();
        DrawTabButton(UiText.Captured, 2);
        ImGui.SameLine();
        DrawTabButton(UiText.Duty, 3);
    }

    private void DrawTabButton(string label, int index)
    {
        var active = tab == index;
        var accent = active ? new Vector4(1f, 0.78f, 0.25f, 1f) : new Vector4(0.55f, 0.55f, 0.55f, 1f);

        ImGui.PushStyleColor(ImGuiCol.Button, active ? new Vector4(0.35f, 0.28f, 0.08f, 1f) : new Vector4(0.18f, 0.18f, 0.18f, 1f));
        ImGui.PushStyleColor(ImGuiCol.Text, accent);
        if (ImGui.Button($"{label}##btab{index}")) tab = index;
        ImGui.PopStyleColor(2);

        if (active)
        {
            var min = ImGui.GetItemRectMin();
            var max = ImGui.GetItemRectMax();
            ImGui.GetWindowDrawList().AddLine(new Vector2(min.X, max.Y), new Vector2(max.X, max.Y), ImGui.ColorConvertFloat4ToU32(accent), 2f);
        }
    }

    private void DrawList()
    {
        ImGui.BeginChild("beast-list", new Vector2(0, 0), true);
        foreach (var b in BestiaryCatalog.All)
        {
            var have = capture.Has(b.Id);
            if ((tab == 1 && have) || (tab == 2 && !have) || (tab == 3 && !b.Duty)) continue;
            if (classFilter > 0 && (int)b.Class != classFilter) continue;

            var loc = b.LocalizedLocation(UiText.Polish);
            if (!string.IsNullOrWhiteSpace(filter) &&
                !b.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) &&
                !loc.Contains(filter, StringComparison.OrdinalIgnoreCase))
                continue;

            var accent = have ? new Vector4(0.55f, 1f, 0.55f, 1f) : new Vector4(0.85f, 0.85f, 0.85f, 1f);
            var label = $"{(have ? "[x]" : "[ ]")} #{b.Id:00} {b.Name}";
            ImGui.PushStyleColor(ImGuiCol.Text, accent);
            if (ImGui.Selectable(label, selectedId == b.Id)) selectedId = b.Id;
            ImGui.PopStyleColor();
            ImGui.SameLine();
            ImGui.TextDisabled($"{b.Class} - {b.Affinity}");
        }
        ImGui.EndChild();
    }

    private void DrawDetails()
    {
        var b = BestiaryCatalog.Get(selectedId) ?? BestiaryCatalog.All[0];
        var have = capture.Has(b.Id);

        ImGui.BeginChild("beast-details", new Vector2(0, 0), true);
        ImGui.TextColored(new Vector4(.60f, 1f, .48f, 1f), $"#{b.Id:00} {b.Name}");
        ImGui.SameLine();
        if (ImGui.Button(have ? UiText.Captured : UiText.Missing)) capture.Toggle(b.Id);
        ImGui.Separator();

        Row(UiText.Location, b.LocalizedLocation(UiText.Polish));
        Row(UiText.Level, $"Lv {b.Level}");
        Row(UiText.KinshipLabel, $"{b.Class} - {BestiaryCatalog.BeastModeName(b.Class)}");
        Row(UiText.AffinityLabel, b.Affinity.ToString());
        ImGui.Separator();

        Row(UiText.Trick, b.Trick);
        Row(UiText.TemperedRelease, b.TemperedRelease);

        if (b.Duty)
        {
            ImGui.Separator();
            ImGui.TextColored(new Vector4(1f, .58f, .34f, 1f), UiText.Duty);
            ImGui.TextWrapped(b.LocalizedGourdHint(UiText.Polish) ?? UiText.T("Lapanie w duty.", "Duty capture."));
        }

        ImGui.Separator();
        ImGui.TextWrapped(b.LocalizedNotes(UiText.Polish));
        ImGui.EndChild();
    }

    private static void Row(string key, string value)
    {
        ImGui.TextDisabled(key);
        ImGui.TextWrapped(value);
    }

    private void Nav()
    {
        if (ImGui.Button(UiText.Bestiary)) IsOpen = true;
        ImGui.SameLine();
        if (ImGui.Button(UiText.Progress)) UiNavigator.OpenProgress?.Invoke();
        ImGui.SameLine();
        if (ImGui.Button(UiText.Settings)) UiNavigator.OpenSettings?.Invoke();
        ImGui.SameLine();
        if (ImGui.Button(UiText.Close)) IsOpen = false;
    }
}
