using BeastmasterAssist.Data;
using BeastmasterAssist.Tracking;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
namespace BeastmasterAssist.Ui;
public sealed class ProgressWindow:Window
{
 private readonly Configuration config;private readonly ProgressTracker tracker;
 public ProgressWindow(Configuration config,ProgressTracker tracker,GameData data):base("BST Progress##BeastmasterAssistProgress"){this.config=config;this.tracker=tracker;}
 public override void Draw(){Nav();ImGui.Separator();ImGui.TextUnformatted("Achievementy");foreach(var a in ProgressCatalog.Achievements)ImGui.BulletText($"{(tracker.AchievementDone(a)?"[x]":"[ ]")} {a.Name} — {a.Description}");ImGui.Separator();ImGui.TextUnformatted("Gear");foreach(var g in ProgressCatalog.ExclusiveGear){var have=tracker.GearOwned(g);if(ImGui.Checkbox($"##g{g.Key}",ref have))tracker.ToggleGear(g.Key);ImGui.SameLine();ImGui.TextWrapped($"{g.Name} — {g.Source}");}ImGui.Separator();ImGui.TextUnformatted("Questy");foreach(var q in ProgressCatalog.Quests){var done=config.CompletedQuestKeys.Contains(q.Key);if(ImGui.Checkbox($"##q{q.Key}",ref done))tracker.ToggleQuest(q.Key);ImGui.SameLine();ImGui.TextWrapped($"Lv{q.Level} {q.Name} — {q.Rewards}");}}
 private void Nav(){if(ImGui.Button("BESTIARIUSZ"))UiNavigator.OpenBestiary?.Invoke();ImGui.SameLine();if(ImGui.Button("PROGRES"))IsOpen=true;ImGui.SameLine();if(ImGui.Button("USTAWIENIA"))UiNavigator.OpenSettings?.Invoke();ImGui.SameLine();if(ImGui.Button("ZAMKNIJ"))IsOpen=false;}
}
