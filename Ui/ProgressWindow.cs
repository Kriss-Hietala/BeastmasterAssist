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

    public override void Draw()
    {
        UiText.Sync(config);
        Nav();
        ImGui.Separator();
        if (ImGui.Button(UiText.Achievement)) tab = 0; ImGui.SameLine();
        if (ImGui.Button(UiText.Gear)) tab = 1; ImGui.SameLine();
        if (ImGui.Button(UiText.Quests)) tab = 2;
        ImGui.Separator();

        switch (tab)
        {
            case 0: DrawAchievements(); break;
            case 1: DrawGear(); break;
            default: DrawQuests(); break;
        }
    }

    private void DrawAchievements()
    {
        var done = ProgressCatalog.Achievements.Count(tracker.AchievementDone);
        ImGui.TextColored(new Vector4(1f, .78f, .25f, 1f), $"{UiText.Achievement}: {done}/{ProgressCatalog.Achievements.Count}");
        foreach (var a in ProgressCatalog.Achievements)
        {
            var complete = tracker.AchievementDone(a);
            ImGui.TextColored(complete ? new Vector4(.6f, 1f, .48f, 1f) : new Vector4(.85f, .85f, .85f, 1f), $"{(complete ? "[x]" : "[ ]")} {a.Name}");
            ImGui.Indent();
            ImGui.TextWrapped(a.LocalizedDescription(UiText.Polish));
            var reward = a.LocalizedReward(UiText.Polish);
            if (reward is not null) ImGui.TextColored(new Vector4(1f, .85f, .4f, 1f), $"{UiText.Reward}: {reward}");
            ImGui.Unindent();
            ImGui.Separator();
        }
    }

    private void DrawGear()
    {
        var owned = ProgressCatalog.ExclusiveGear.Count(tracker.GearOwned);
        ImGui.TextColored(new Vector4(1f, .78f, .25f, 1f), $"{UiText.Gear}: {owned}/{ProgressCatalog.ExclusiveGear.Count}");
        foreach (var g in ProgressCatalog.ExclusiveGear)
        {
            var have = tracker.GearOwned(g);
            if (ImGui.Checkbox($"##g{g.Key}", ref have)) tracker.ToggleGear(g.Key);
            ImGui.SameLine();
            ImGui.TextWrapped($"{g.Name}{(g.Upgradeable ? $" +{g.UpgradeTier}" : "")} [{g.Slot}] - {g.LocalizedSource(UiText.Polish)}");
        }
    }

    private void DrawQuests()
    {
        var done = ProgressCatalog.Quests.Count(q => config.CompletedQuestKeys.Contains(q.Key));
        ImGui.TextColored(new Vector4(1f, .78f, .25f, 1f), $"{UiText.Quests}: {done}/{ProgressCatalog.Quests.Count}");
        foreach (var q in ProgressCatalog.Quests)
        {
            var qdone = config.CompletedQuestKeys.Contains(q.Key);
            if (ImGui.Checkbox($"##q{q.Key}", ref qdone)) tracker.ToggleQuest(q.Key);
            ImGui.SameLine();
            ImGui.TextWrapped($"Lv{q.Level} {q.Name} ({q.Npc}) - {q.LocalizedRewards(UiText.Polish)}");
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
