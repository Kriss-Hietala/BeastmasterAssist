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

    public BestiaryWindow(Configuration config, CaptureTracker capture, GameData data) : base("Master's Bestiary##BeastmasterAssistBestiary")
    {
        this.config = config;
        this.capture = capture;
        Size = new Vector2(960, 620);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void Draw()
    {
        UiText.Sync(config);
        Nav();
        ImGui.Separator();
        ImGui.TextColored(new Vector4(1f, .78f, .25f, 1f), UiText.Bestiary);
        ImGui.SameLine();
        ImGui.ProgressBar(capture.CapturedCount / 50f, new Vector2(220, 18), UiText.CapturedCount(capture.CapturedCount));
        ImGui.SameLine();
        ImGui.TextDisabled(UiText.MissingCount(BestiaryCatalog.All.Count - capture.CapturedCount));
        Tabs();
        ImGui.InputText(UiText.Search, ref filter, 64);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(150);
        ImGui.Combo(UiText.Filters, ref classFilter, "All\0Beastkin\0Vilekin\0Cloudkin\0Seedkin\0Wavekin\0Scalekin\0Soulkin\0Ashkin\0");
        ImGui.Separator();
        if (ImGui.BeginTable("bst-layout", 2, ImGuiTableFlags.SizingStretchProp))
        {
            ImGui.TableSetupColumn("list", ImGuiTableColumnFlags.WidthStretch, .48f);
            ImGui.TableSetupColumn("details", ImGuiTableColumnFlags.WidthStretch, .52f);
            ImGui.TableNextRow(); ImGui.TableNextColumn(); DrawList(); ImGui.TableNextColumn(); DrawDetails(); ImGui.EndTable();
        }
    }

    private void Tabs()
    {
        if (ImGui.Button(UiText.All)) tab = 0; ImGui.SameLine();
        if (ImGui.Button(UiText.Missing)) tab = 1; ImGui.SameLine();
        if (ImGui.Button(UiText.Captured)) tab = 2; ImGui.SameLine();
        if (ImGui.Button(UiText.Duty)) tab = 3;
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
            if (!string.IsNullOrWhiteSpace(filter) && !b.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) && !loc.Contains(filter, StringComparison.OrdinalIgnoreCase)) continue;
            var label = $"{(have ? "[x]" : "[ ]")} #{b.Id:00}  {b.Name}";
            if (ImGui.Selectable(label, selectedId == b.Id)) selectedId = b.Id;
            ImGui.SameLine(); ImGui.TextDisabled($"{b.Class} - {b.Affinity}");
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

    private static void Row(string key, string value) { ImGui.TextDisabled(key); ImGui.TextWrapped(value); }
    private void Nav() { if (ImGui.Button(UiText.Bestiary)) IsOpen = true; ImGui.SameLine(); if (ImGui.Button(UiText.Progress)) UiNavigator.OpenProgress?.Invoke(); ImGui.SameLine(); if (ImGui.Button(UiText.Settings)) UiNavigator.OpenSettings?.Invoke(); ImGui.SameLine(); if (ImGui.Button(UiText.Close)) IsOpen = false; }
}
