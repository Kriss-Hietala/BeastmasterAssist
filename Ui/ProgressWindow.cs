using System.Numerics;
using BeastmasterAssist.Data;
using BeastmasterAssist.Tracking;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace BeastmasterAssist.Ui;

public sealed class ProgressWindow : Window
{
    private readonly Configuration config;
    private readonly ProgressTracker tracker;
    private int tab;

    public ProgressWindow(Configuration config, ProgressTracker tracker, GameData data)
        : base("BST Progress##BeastmasterAssistProgress")
    {
        this.config = config;
        this.tracker = tracker;
        Size = new Vector2(760, 620);
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
        Nav();
        ImGui.Separator();
        ImGui.Spacing();

        DrawTabButton(UiText.Achievement, 0, ProgressCatalog.Achievements.Count(tracker.AchievementDone), ProgressCatalog.Achievements.Count);
        ImGui.SameLine();
        DrawTabButton(UiText.Gear, 1, ProgressCatalog.ExclusiveGear.Count(tracker.GearOwned), ProgressCatalog.ExclusiveGear.Count);
        ImGui.SameLine();
        DrawTabButton(UiText.Quests, 2, ProgressCatalog.Quests.Count(q => config.CompletedQuestKeys.Contains(q.Key)), ProgressCatalog.Quests.Count);

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        switch (tab)
        {
            case 0: DrawAchievements(); break;
            case 1: DrawGear(); break;
            default: DrawQuests(); break;
        }
    }

    private void DrawTabButton(string label, int index, int done, int total)
    {
        var active = tab == index;
        var accent = active ? new Vector4(1f, 0.78f, 0.25f, 1f) : new Vector4(0.55f, 0.55f, 0.55f, 1f);

        ImGui.PushStyleColor(ImGuiCol.Button, active ? new Vector4(0.35f, 0.28f, 0.08f, 1f) : new Vector4(0.18f, 0.18f, 0.18f, 1f));
        ImGui.PushStyleColor(ImGuiCol.Text, accent);
        if (ImGui.Button($"{label} ({done}/{total})##tab{index}", new Vector2(160, 0))) tab = index;
        ImGui.PopStyleColor(2);

        if (active)
        {
            var min = ImGui.GetItemRectMin();
            var max = ImGui.GetItemRectMax();
            ImGui.GetWindowDrawList().AddLine(new Vector2(min.X, max.Y), new Vector2(max.X, max.Y), ImGui.ColorConvertFloat4ToU32(accent), 2f);
        }
    }

    private static void DrawProgressBar(int done, int total, Vector4 color)
    {
        var frac = total == 0 ? 0f : Math.Clamp(done / (float)total, 0f, 1f);
        var barHeight = MathF.Max(16f, ImGui.GetTextLineHeight() + ImGui.GetStyle().FramePadding.Y * 2f);
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, color);
        ImGui.ProgressBar(frac, new Vector2(-1, barHeight), $"{done}/{total}");
        ImGui.PopStyleColor();
        ImGui.Spacing();
    }

    private void DrawAchievements()
    {
        var done = ProgressCatalog.Achievements.Count(tracker.AchievementDone);
        DrawProgressBar(done, ProgressCatalog.Achievements.Count, new Vector4(1f, 0.72f, 0.25f, 1f));

        foreach (var a in ProgressCatalog.Achievements)
        {
            var complete = tracker.AchievementDone(a);
            var accent = complete ? new Vector4(0.55f, 1f, 0.55f, 1f) : new Vector4(0.55f, 0.55f, 0.55f, 1f);

            ImGui.PushStyleColor(ImGuiCol.Border, accent);
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 1.2f);
            ImGui.BeginChild($"ach_{a.Name}", new Vector2(0, 0), true, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

            ImGui.TextColored(accent, $"{(complete ? "[x]" : "[ ]")} {a.Name}");
            ImGui.Indent(8);
            ImGui.TextWrapped(a.LocalizedDescription(UiText.Polish));
            var reward = a.LocalizedReward(UiText.Polish);
            if (reward is not null) ImGui.TextColored(new Vector4(1f, 0.85f, 0.4f, 1f), $"{UiText.Reward}: {reward}");
            ImGui.Unindent(8);

            ImGui.EndChild();
            ImGui.PopStyleVar();
            ImGui.PopStyleColor();
            ImGui.Spacing();
        }
    }

    private void DrawGear()
    {
        var owned = ProgressCatalog.ExclusiveGear.Count(tracker.GearOwned);
        DrawProgressBar(owned, ProgressCatalog.ExclusiveGear.Count, new Vector4(0.6f, 0.85f, 1f, 1f));

        foreach (var g in ProgressCatalog.ExclusiveGear)
        {
            var have = tracker.GearOwned(g);
            if (ImGui.Checkbox($"##g{g.Key}", ref have)) tracker.ToggleGear(g.Key);
            ImGui.SameLine();
            var color = have ? new Vector4(0.55f, 1f, 0.55f, 1f) : new Vector4(0.85f, 0.85f, 0.85f, 1f);
            ImGui.TextColored(color, $"{g.Name}{(g.Upgradeable ? $" +{g.UpgradeTier}" : "")} [{g.Slot}]");
            ImGui.Indent(20);
            ImGui.TextWrapped(g.LocalizedSource(UiText.Polish));
            ImGui.Unindent(20);
        }
    }

    private void DrawQuests()
    {
        var done = ProgressCatalog.Quests.Count(q => config.CompletedQuestKeys.Contains(q.Key));
        DrawProgressBar(done, ProgressCatalog.Quests.Count, new Vector4(0.75f, 0.55f, 1f, 1f));

        foreach (var q in ProgressCatalog.Quests)
        {
            var qdone = config.CompletedQuestKeys.Contains(q.Key);
            if (ImGui.Checkbox($"##q{q.Key}", ref qdone)) tracker.ToggleQuest(q.Key);
            ImGui.SameLine();
            var color = qdone ? new Vector4(0.55f, 1f, 0.55f, 1f) : new Vector4(0.85f, 0.85f, 0.85f, 1f);
            ImGui.TextColored(color, $"Lv{q.Level} {q.Name} ({q.Npc})");
            ImGui.Indent(20);
            ImGui.TextWrapped(q.LocalizedRewards(UiText.Polish));
            ImGui.Unindent(20);
        }
    }

    private void Nav()
    {
        if (ImGui.Button(UiText.Bestiary)) UiNavigator.OpenBestiary?.Invoke();
        ImGui.SameLine();
        if (ImGui.Button(UiText.Progress)) IsOpen = true;
        ImGui.SameLine();
        if (ImGui.Button(UiText.Settings)) UiNavigator.OpenSettings?.Invoke();
        ImGui.SameLine();
        if (ImGui.Button(UiText.Close)) IsOpen = false;
    }
}
