using BeastmasterAssist.Combat;
using BeastmasterAssist.Data;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Gui.NamePlate;
using Dalamud.Plugin.Services;

namespace BeastmasterAssist.Tracking;

/// <summary>
/// Draws a small marker icon (like a hunt-train mark) above battle NPCs whose
/// name matches an uncaptured Master's Bestiary beast. Purely visual: it never
/// selects a target, moves the camera, or presses any action.
///
/// The game appears to reset the marker icon slot more often than a short
/// throttle interval can compensate for, so RequestRedraw is called every
/// framework tick instead of on a timer.
/// </summary>
public sealed class NameplateMarker
{
    // FFXIV hunt-mark style icon (bronze marker). Adjust here if you prefer a different icon id.
    private const int MarkerIconId = 60092;

    private readonly Configuration config;
    private readonly INamePlateGui namePlateGui;
    private readonly CaptureTracker capture;
    private readonly GameData gameData;
    private readonly CombatState state;

    public NameplateMarker(Configuration config, INamePlateGui namePlateGui, CaptureTracker capture, GameData gameData, CombatState state)
    {
        this.config = config;
        this.namePlateGui = namePlateGui;
        this.capture = capture;
        this.gameData = gameData;
        this.state = state;
    }

    public void Attach() => namePlateGui.OnNamePlateUpdate += OnNamePlateUpdate;
    public void Detach() => namePlateGui.OnNamePlateUpdate -= OnNamePlateUpdate;

    // Markery maja sens tylko na Beastmasterze - na innych jobach lapanie bestii
    // jest niedostepne, wiec sugerowanie "mozesz to zlapac" byloby mylace.
    // Respektujemy tez ShowOnlyOnBeastmaster, tak jak robi to overlay.
    private bool ShouldShow =>
        config.ShowNameplateMarkers &&
        (!config.ShowOnlyOnBeastmaster || gameData.IsBeastmaster(state.Player));

    private void OnNamePlateUpdate(INamePlateUpdateContext context, IReadOnlyList<INamePlateUpdateHandler> handlers)
    {
        if (!ShouldShow) return;

        foreach (var handler in handlers)
        {
            if (handler.NamePlateKind != NamePlateKind.BattleNpc) continue;

            var name = handler.Name.TextValue;
            if (string.IsNullOrWhiteSpace(name)) continue;

            var beast = capture.MatchByMonster(name);
            if (beast is null || capture.Has(beast.Id)) continue;

            handler.MarkerIconId = MarkerIconId;
        }
    }

    public void Tick()
    {
        if (!ShouldShow) return;
        namePlateGui.RequestRedraw();
    }
}
