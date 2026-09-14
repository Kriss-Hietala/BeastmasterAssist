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
    private readonly LevelingAdvisor leveling;
    private DateTime nextSnapshotAt = DateTime.MinValue;
    private string captureText = "";
    private List<Advice> rotationSnapshot = [];
    private List<Advice> reactionSnapshot = [];
    private List<Advice> mitigationSnapshot = [];
    private List<LevelingAdvisor.Spot> levelingSnapshot = [];

    public OverlayWindow(Configuration config, CombatState state, RotationAdvisor rotation, ReactionAdvisor reactions, MitigationAdvisor mitigation, CaptureTracker capture, LevelingAdvisor leveling)
        : base("Beastmaster Assist##BeastmasterAssistOverlay", ImGuiWindowFlags.NoCollapse)
    {
        this.config = config;
        this.state = state;
        this.rotation = rotation;
        this.reactions = reactions;
        this.mitigation = mitigation;
        this.capture = capture;
        this.leveling = leveling;
        RespectCloseHotkey = false;
        Size = new Vector2(440, 0);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void PreDraw()
    {
        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        if (config.LockOverlay) Flags |= ImGuiWindowFlags.NoMove;
        ImGui.SetNextWindowBgAlpha(0.90f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 10f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 8f);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 6f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(14, 12));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(6, 8));
    }

    public override void PostDraw() => ImGui.PopStyleVar(5);

    public override void Draw()
    {
        UiText.Sync(config);
        RefreshSnapshot();
        var scale = Math.Clamp(config.OverlayScale * 1.15f, 1.0f, 2.0f);
        ImGui.SetWindowFontScale(scale);

        DrawHeader(scale);
        DrawGauges(scale);

        DrawCard(UiText.Rotation, new Vector4(1.00f, 0.80f, 0.30f, 1f), () => DrawAdviceList(rotationSnapshot, 3));
        if (config.ShowReactions) DrawCard(UiText.Reactions, new Vector4(1.00f, 0.45f, 0.40f, 1f), () => DrawAdviceList(reactionSnapshot, 2));
        if (config.ShowMitigation) DrawCard(UiText.Mitigation, new Vector4(0.40f, 0.75f, 1.00f, 1f), () => DrawAdviceList(mitigationSnapshot, 2));
        if (config.ShowCaptureHud && !string.IsNullOrEmpty(captureText))
            DrawCard(UiText.Capture, new Vector4(0.55f, 1.00f, 0.55f, 1f), () => ImGui.TextWrapped(captureText));
        if (levelingSnapshot.Count > 0)
            DrawCard(UiText.Leveling, new Vector4(0.75f, 0.55f, 1.00f, 1f), DrawLevelingList);

        ImGui.SetWindowFontScale(1f);
    }

    private void DrawHeader(float scale)
    {
        var accent = state.InCombat ? new Vector4(1f, 0.35f, 0.30f, 1f) : new Vector4(0.35f, 0.85f, 0.45f, 1f);
        var drawList = ImGui.GetWindowDrawList();
        var origin = ImGui.GetCursorScreenPos();
        drawList.AddCircleFilled(origin + new Vector2(6, 8 * scale), 5f * scale, ImGui.ColorConvertFloat4ToU32(accent));
        ImGui.Dummy(new Vector2(16 * scale, 0));
        ImGui.SameLine();
        ImGui.TextColored(new Vector4(1f, 0.85f, 0.35f, 1f), "BEASTMASTER ASSIST");
        ImGui.SameLine();
        ImGui.TextDisabled(state.Player is not null ? $"Lv{state.Level}" : UiText.NoPlayer);
        if (ImGui.SmallButton(UiText.HideOverlay)) UiNavigator.ToggleOverlay?.Invoke();
        ImGui.Separator();
    }

    private void DrawGauges(float scale)
    {
        var tpFrac = Math.Clamp(state.PlayerTp / 250f, 0f, 1f);
        var famFrac = Math.Clamp(state.FamiliarTp / 250f, 0f, 1f);
        var barHeight = 16f * scale;

        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new Vector4(1.0f, 0.72f, 0.20f, 1f));
        ImGui.ProgressBar(tpFrac, new Vector2(-1, barHeight), $"TP {state.PlayerTp}/250");
        ImGui.PopStyleColor();

        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new Vector4(0.35f, 0.70f, 1.0f, 1f));
        ImGui.ProgressBar(famFrac, new Vector2(-1, barHeight), $"{UiText.FamiliarLabel} {state.FamiliarTp}/250");
        ImGui.PopStyleColor();

        ImGui.TextDisabled($"{state.ActiveKinship}   {BestiaryCatalog.BeastModeName(state.ActiveKinship)}");
        ImGui.Dummy(new Vector2(0, 4f * scale));
    }

    private static void DrawCard(string title, Vector4 accent, Action body)
    {
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(accent.X, accent.Y, accent.Z, 0.06f));
        ImGui.PushStyleColor(ImGuiCol.Border, accent);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 1.4f);
        ImGui.BeginChild($"card_{title}", new Vector2(0, 0), true, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        ImGui.TextColored(accent, title.ToUpperInvariant());
        ImGui.Separator();
        body();
        ImGui.EndChild();
        ImGui.PopStyleVar();
        ImGui.PopStyleColor(2);
        ImGui.Dummy(new Vector2(0, 2));
    }

    private static void DrawAdviceList(List<Advice> advice, int max)
    {
        var shown = advice.Take(max).ToList();
        if (shown.Count == 0) { ImGui.TextDisabled("-"); return; }
        foreach (var item in shown)
        {
            ImGui.BulletText(item.Action);
            ImGui.SameLine();
            ImGui.TextDisabled(item.Reason);
        }
    }

    private void DrawLevelingList()
    {
        foreach (var spot in levelingSnapshot)
        {
            var tag = spot.Captured ? UiText.Captured : UiText.Missing;
            ImGui.BulletText($"Lv{spot.Level} {spot.Name} ({tag})");
            ImGui.SameLine();
            ImGui.TextDisabled(spot.Location);
        }
    }

    private void RefreshSnapshot()
    {
        if (DateTime.UtcNow < nextSnapshotAt) return;
        nextSnapshotAt = DateTime.UtcNow.AddMilliseconds(750);
        captureText = capture.CapturePrompt;
        rotationSnapshot = rotation.Advise(state);
        reactionSnapshot = reactions.Advise(state);
        mitigationSnapshot = mitigation.Advise(state);
        levelingSnapshot = leveling.Suggest(state.Level, capture.Has);
    }
}
