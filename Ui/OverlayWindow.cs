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
    private bool showMoreLeveling;


    private readonly List<Advice> rotationSnapshot = [];
    private readonly List<Advice> reactionSnapshot = [];
    private readonly List<Advice> mitigationSnapshot = [];
    private readonly List<LevelingAdvisor.Spot> levelingSnapshot = [];


    public OverlayWindow(
        Configuration config,
        CombatState state,
        RotationAdvisor rotation,
        ReactionAdvisor reactions,
        MitigationAdvisor mitigation,
        CaptureTracker capture,
        LevelingAdvisor leveling)
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
        Size = new Vector2(380, 0);
        SizeCondition = ImGuiCond.FirstUseEver;
    }


    public override void PreDraw()
    {
        var flags = ImGuiWindowFlags.NoCollapse;
        if (config.LockOverlay)
            flags |= ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize;


        Flags = flags;


        ImGui.SetNextWindowBgAlpha(0.90f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 8f);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 4f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(12, 10));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(6, 6));
        ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.08f, 0.08f, 0.10f, 0.94f));
        ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0.30f, 0.30f, 0.35f, 0.80f));
    }


    public override void PostDraw()
    {
        ImGui.PopStyleColor(2);
        ImGui.PopStyleVar(4);
    }


    public override void Draw()
    {
        UiText.Sync(config);
        RefreshSnapshot();


        var scale = Math.Clamp(config.OverlayScale, 0.60f, 1.80f);
        ImGui.SetWindowFontScale(scale);


        if (config.CompactOverlay)
            DrawCompactMode();
        else
            DrawClassicMode();


        ImGui.SetWindowFontScale(1f);
    }


    // ================= TRYB KLASYCZNY (PELNY, ORYGINALNY) =================
    private void DrawClassicMode()
    {
        DrawClassicHeader();
        DrawClassicGauges();


        if (config.ShowRotation && rotationSnapshot.Count > 0)
        {
            DrawClassicSection(UiText.Rotation, new Vector4(1f, 0.8f, 0.3f, 1f), () =>
            {
                foreach (var x in rotationSnapshot.Take(3))
                {
                    ImGui.BulletText(x.Action);
                    ImGui.SameLine();
                    ImGui.TextDisabled(x.Reason);
                }
            });
        }


        if (config.ShowReactions && reactionSnapshot.Count > 0)
        {
            DrawClassicSection(UiText.Reactions, new Vector4(1f, 0.45f, 0.4f, 1f), () =>
            {
                foreach (var x in reactionSnapshot.Take(2))
                {
                    ImGui.BulletText(x.Action);
                    ImGui.SameLine();
                    ImGui.TextDisabled(x.Reason);
                }
            });
        }


        if (config.ShowMitigation && mitigationSnapshot.Count > 0)
        {
            DrawClassicSection(UiText.Mitigation, new Vector4(0.4f, 0.75f, 1f, 1f), () =>
            {
                foreach (var x in mitigationSnapshot.Take(2))
                {
                    ImGui.BulletText(x.Action);
                    ImGui.SameLine();
                    ImGui.TextDisabled(x.Reason);
                }
            });
        }


        if (levelingSnapshot.Count > 0)
        {
            DrawClassicSection(UiText.Leveling, new Vector4(0.75f, 0.55f, 1f, 1f), () =>
            {
                for (var i = 0; i < levelingSnapshot.Count; i++)
                {
                    var s = levelingSnapshot[i];
                    var tag = s.Captured ? UiText.Captured : UiText.Missing;
                    ImGui.TextColored(i == 0 ? new Vector4(0.85f, 0.70f, 1f, 1f) : new Vector4(0.9f, 0.9f, 0.9f, 1f),
                        i == 0 ? $"-> Lv{s.Level} {s.Name} ({tag})" : $" - Lv{s.Level} {s.Name} ({tag})");
                    ImGui.Indent(12);
                    ImGui.TextWrapped(s.Location);
                    ImGui.Unindent(12);
                }
            });
        }


        if (config.ShowCaptureHud && !string.IsNullOrEmpty(captureText))
        {
            DrawClassicSection(UiText.Capture, new Vector4(0.55f, 1f, 0.55f, 1f), () =>
            {
                ImGui.TextWrapped(captureText);
            });
        }
    }


    private void DrawClassicHeader()
    {
        var accent = state.InCombat ? new Vector4(1f, 0.35f, 0.3f, 1f) : new Vector4(0.35f, 0.85f, 0.45f, 1f);
        var p = ImGui.GetCursorScreenPos();
        ImGui.GetWindowDrawList().AddCircleFilled(p + new Vector2(6, 8), 5f, ImGui.ColorConvertFloat4ToU32(accent));
        ImGui.Dummy(new Vector2(14, 0));
        ImGui.SameLine();
        ImGui.TextColored(new Vector4(1f, 0.85f, 0.35f, 1f), "BEASTMASTER ASSIST");
        ImGui.SameLine();
        ImGui.TextDisabled(state.Player is not null ? $"Lv{state.Level}" : UiText.NoPlayer);


        ImGui.SameLine();
        if (ImGui.SmallButton("Compact"))
        {
            config.CompactOverlay = true;
            config.Save();
        }


        ImGui.SameLine();
        if (ImGui.SmallButton(UiText.HideOverlay))
        {
            UiNavigator.ToggleOverlay?.Invoke();
        }


        ImGui.Separator();
    }


    private void DrawClassicGauges()
    {
        var h = MathF.Max(18f, ImGui.GetTextLineHeight() + ImGui.GetStyle().FramePadding.Y * 2f);
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new Vector4(1f, 0.72f, 0.2f, 1f));
        ImGui.ProgressBar(state.PlayerTp / 250f, new Vector2(-1, h), $"TP {state.PlayerTp}/250");
        ImGui.PopStyleColor();


        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new Vector4(0.35f, 0.70f, 1f, 1f));
        ImGui.ProgressBar(state.FamiliarTp / 250f, new Vector2(-1, h), $"{UiText.FamiliarLabel} {state.FamiliarTp}/250");
        ImGui.PopStyleColor();


        ImGui.TextDisabled($"{state.ActiveKinship}   {BestiaryCatalog.BeastModeName(state.ActiveKinship)}");
        ImGui.Spacing();
    }


    private static void DrawClassicSection(string t, Vector4 c, Action body)
    {
        ImGui.Spacing();
        ImGui.TextColored(c, t.ToUpperInvariant());
        ImGui.PushStyleColor(ImGuiCol.Separator, c);
        ImGui.Separator();
        ImGui.PopStyleColor();
        ImGui.Indent(6);
        body();
        ImGui.Unindent(6);
    }


    // ================= TRYB KOMPAKTOWY =================
    private void DrawCompactMode()
    {
        DrawCompactHeader();
        DrawCompactResourceBars();


        if (config.ShowReactions && reactionSnapshot.Count > 0)
        {
            var r = reactionSnapshot[0];
            ImGui.TextColored(new Vector4(1f, 0.4f, 0.4f, 1f), "[REACTION]");
            ImGui.SameLine();
            ImGui.Text(r.Action);
            ImGui.SameLine();
            ImGui.TextDisabled($"({r.Reason})");
        }


        if (config.ShowMitigation && mitigationSnapshot.Count > 0)
        {
            var m = mitigationSnapshot[0];
            ImGui.TextColored(new Vector4(0.4f, 0.75f, 1f, 1f), "[MITIGATION]");
            ImGui.SameLine();
            ImGui.Text(m.Action);
            ImGui.SameLine();
            ImGui.TextDisabled($"({m.Reason})");
        }


        if (config.ShowCaptureHud && !string.IsNullOrEmpty(captureText))
        {
            ImGui.Spacing();
            ImGui.TextColored(new Vector4(0.45f, 0.95f, 0.45f, 1f), "CAPTURE");
            ImGui.TextWrapped(captureText);
        }


        if (config.ShowRotation && rotationSnapshot.Count > 0)
        {
            var prime = rotationSnapshot[0];
            ImGui.TextDisabled("NEXT");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(1f, 0.95f, 0.6f, 1f), prime.Action);
            ImGui.SameLine();
            ImGui.TextDisabled($"({prime.Reason})");


            if (state.InCombat && rotationSnapshot.Count > 1)
            {
                for (var i = 1; i < Math.Min(rotationSnapshot.Count, 3); i++)
                {
                    var sub = rotationSnapshot[i];
                    ImGui.BulletText(sub.Action);
                    ImGui.SameLine();
                    ImGui.TextDisabled(sub.Reason);
                }
            }
        }


        if (!state.InCombat && levelingSnapshot.Count > 0)
        {
            ImGui.Spacing();
            ImGui.Separator();
            var best = levelingSnapshot[0];
            var tag = best.Captured ? UiText.Captured : UiText.Missing;


            ImGui.TextColored(new Vector4(0.85f, 0.7f, 1f, 1f), "LEVELING");
            ImGui.Text($"Lv{best.Level} {best.Name} ({tag})");
            ImGui.TextDisabled(best.Location);


            if (levelingSnapshot.Count > 1)
            {
                ImGui.SameLine();
                if (ImGui.SmallButton(showMoreLeveling ? "[-]" : $"+{levelingSnapshot.Count - 1}"))
                {
                    showMoreLeveling = !showMoreLeveling;
                }


                if (showMoreLeveling)
                {
                    for (var i = 1; i < levelingSnapshot.Count; i++)
                    {
                        var alt = levelingSnapshot[i];
                        var altTag = alt.Captured ? UiText.Captured : UiText.Missing;
                        ImGui.TextDisabled($"- Lv{alt.Level} {alt.Name} ({altTag}) - {alt.Location}");
                    }
                }
            }
        }
    }


    private void DrawCompactHeader()
    {
        var accent = state.InCombat ? new Vector4(1f, 0.3f, 0.3f, 1f) : new Vector4(0.3f, 0.9f, 0.4f, 1f);
        var p = ImGui.GetCursorScreenPos();
        ImGui.GetWindowDrawList().AddCircleFilled(p + new Vector2(5, 7), 4f, ImGui.ColorConvertFloat4ToU32(accent));
        ImGui.Dummy(new Vector2(10, 0));
        ImGui.SameLine();
        ImGui.TextColored(new Vector4(1f, 0.82f, 0.3f, 1f), "BST");
        ImGui.SameLine();
        ImGui.TextDisabled(state.Player is not null ? $"Lv{state.Level}" : UiText.NoPlayer);


        if (state.ActiveKinship != Kinship.None)
        {
            ImGui.SameLine();
            ImGui.TextDisabled($"- {state.ActiveKinship}");
        }


        // Dynamiczne wyliczenie pozycji przyciskow zamiast sztywnego "- 75f",
        // ktore nachodzilo na tekst naglowka przy waskim oknie lub wiekszej skali czcionki.
        var style = ImGui.GetStyle();
        var classicWidth = ImGui.CalcTextSize("Classic").X + style.FramePadding.X * 2f;
        var closeWidth = ImGui.CalcTextSize("x").X + style.FramePadding.X * 2f;
        var reserved = classicWidth + closeWidth + style.ItemSpacing.X * 2f;
        var offset = MathF.Max(ImGui.GetCursorPosX(), ImGui.GetWindowWidth() - reserved);


        ImGui.SameLine(offset);
        if (ImGui.SmallButton("Classic"))
        {
            config.CompactOverlay = false;
            config.Save();
        }
        ImGui.SameLine();
        if (ImGui.SmallButton("x"))
        {
            UiNavigator.ToggleOverlay?.Invoke();
        }


        ImGui.Separator();
    }


    private void DrawCompactResourceBars()
    {
        var barHeight = MathF.Max(18f, ImGui.GetTextLineHeight() + ImGui.GetStyle().FramePadding.Y * 2f);
        var spacing = ImGui.GetStyle().ItemSpacing.X;


        // Mierzymy rzeczywista szerokosc etykiet zamiast zakladac stala rezerwe (85f),
        // ktora przy innej skali/szerokosci okna powodowala nachodzenie paskow TP/Fam.
        var tpLabelWidth = ImGui.CalcTextSize("TP").X;
        var famLabelWidth = ImGui.CalcTextSize("Fam").X;
        var reserved = tpLabelWidth + famLabelWidth + spacing * 4f + 10f;
        var availWidth = ImGui.GetContentRegionAvail().X;
        var barWidth = MathF.Max(40f, (availWidth - reserved) * 0.5f);


        ImGui.AlignTextToFramePadding();
        ImGui.TextDisabled("TP");
        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new Vector4(0.95f, 0.65f, 0.15f, 1f));
        ImGui.ProgressBar(state.PlayerTp / 250f, new Vector2(barWidth, barHeight), $"{state.PlayerTp}");
        ImGui.PopStyleColor();


        ImGui.SameLine(0, 10f);
        ImGui.AlignTextToFramePadding();
        ImGui.TextDisabled("Fam");
        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new Vector4(0.30f, 0.65f, 0.95f, 1f));
        ImGui.ProgressBar(state.FamiliarTp / 250f, new Vector2(barWidth, barHeight), $"{state.FamiliarTp}");
        ImGui.PopStyleColor();


        ImGui.Spacing();
    }


    private void RefreshSnapshot()
    {
        if (DateTime.UtcNow < nextSnapshotAt) return;
        nextSnapshotAt = DateTime.UtcNow.AddMilliseconds(500);


        captureText = capture.CapturePrompt;


        rotationSnapshot.Clear();
        rotationSnapshot.AddRange(rotation.Advise(state));


        reactionSnapshot.Clear();
        reactionSnapshot.AddRange(reactions.Advise(state));


        mitigationSnapshot.Clear();
        mitigationSnapshot.AddRange(mitigation.Advise(state));


        levelingSnapshot.Clear();
        levelingSnapshot.AddRange(leveling.Suggest(state.Level, capture.Has));
    }
}
