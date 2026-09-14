using BeastmasterAssist.Data;
using Dalamud.Game.Gui.NamePlate;
using Dalamud.Plugin.Services;

namespace BeastmasterAssist.Tracking;

/// <summary>
/// Draws a small marker icon (like a hunt-train mark) above battle NPCs whose
/// name matches an uncaptured Master's Bestiary beast. Purely visual: it never
/// selects a target, moves the camera, or presses any action.
/// </summary>
public sealed class NameplateMarker
{
    // FFXIV hunt-mark style icon (bronze marker). Adjust here if you prefer a different icon id.
    private const int MarkerIconId = 60092;

    private readonly Configuration config;
    private readonly INamePlateGui namePlateGui;
    private readonly CaptureTracker capture;

    public NameplateMarker(Configuration config, INamePlateGui namePlateGui, CaptureTracker capture)
    {
        this.config = config;
        this.namePlateGui = namePlateGui;
        this.capture = capture;
    }

    public void Attach() => namePlateGui.OnNamePlateUpdate += OnNamePlateUpdate;
    public void Detach() => namePlateGui.OnNamePlateUpdate -= OnNamePlateUpdate;

    private void OnNamePlateUpdate(INamePlateUpdateContext context, IReadOnlyList<INamePlateUpdateHandler> handlers)
    {
        if (!config.ShowNameplateMarkers) return;

        foreach (var handler in handlers)
        {
            if (handler.NamePlateKind != NamePlateKind.BattleNpcEnemy && handler.NamePlateKind != NamePlateKind.BattleNpcFriendly)
                continue;

            var name = handler.Name.TextValue;
            if (string.IsNullOrWhiteSpace(name)) continue;

            var beast = capture.MatchByMonster(name);
            if (beast is null || capture.Has(beast.Id)) continue;

            handler.MarkerIconId = MarkerIconId;
        }
    }
}
