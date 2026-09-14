using BeastmasterAssist.Data;
using Dalamud.Game.Gui.NamePlate;
using Dalamud.Plugin.Services;

namespace BeastmasterAssist.Tracking;

/// <summary>
/// Draws a small marker icon (like a hunt-train mark) above battle NPCs whose
/// name matches an uncaptured Master's Bestiary beast. Purely visual: it never
/// selects a target, moves the camera, or presses any action.
///
/// Dalamud only invokes <see cref="INamePlateGui.OnNamePlateUpdate"/> when the
/// game itself marks a nameplate's data dirty, not on every frame. Without a
/// forced redraw the marker we set can be silently overwritten by the game's
/// own nameplate refresh, making it flash on and off. Calling RequestRedraw on
/// a short interval keeps the marker visible continuously.
/// </summary>
public sealed class NameplateMarker
{
    // FFXIV hunt-mark style icon (bronze marker). Adjust here if you prefer a different icon id.
    private const int MarkerIconId = 60092;
    private static readonly TimeSpan RedrawInterval = TimeSpan.FromMilliseconds(250);

    private readonly Configuration config;
    private readonly INamePlateGui namePlateGui;
    private readonly CaptureTracker capture;
    private DateTime nextRedrawAt = DateTime.MinValue;

    public NameplateMarker(Configuration config, INamePlateGui namePlateGui, CaptureTracker capture)
    {
        this.config = config;
        this.namePlateGui = namePlateGui;
        this.capture = capture;
    }

    public void Attach() => namePlateGui.OnNamePlateUpdate += OnNamePlateUpdate;
    public void Detach() => namePlateGui.OnNamePlateUpdate -= OnNamePlateUpdate;

    /// <summary>Call this once per framework tick to keep markers from flickering.</summary>
    public void Tick()
    {
        if (!config.ShowNameplateMarkers) return;
        if (DateTime.UtcNow < nextRedrawAt) return;
        nextRedrawAt = DateTime.UtcNow.Add(RedrawInterval);
        namePlateGui.RequestRedraw();
    }

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
