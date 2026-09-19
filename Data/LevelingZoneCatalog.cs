namespace BeastmasterAssist.Data;

/// <summary>Static, level-bracket XP zone guide for actual character/job
/// leveling (FATE grinding grounds) - distinct from Master's Bestiary
/// beast-catch suggestions in LevelingAdvisor.Suggest. Brackets follow the
/// well-documented ARR FATE grinding zones (Prima Games / community guides).
/// Beastmaster caps at Lv50, so only ARR-range brackets are relevant here.</summary>
public static class LevelingZoneCatalog
{
    public sealed record ZoneBracket(int MinLevel, int MaxLevel, string Zone, string ZonePl);

    public static readonly IReadOnlyList<ZoneBracket> Brackets =
    [
        new(1, 12, "Western Thanalan / Central Shroud / Middle La Noscea (starting zone)", "Western Thanalan / Central Shroud / Middle La Noscea (strefa startowa)"),
        new(12, 21, "Western La Noscea (Aleport)", "Western La Noscea (Aleport)"),
        new(21, 30, "South Shroud (Quarrymill)", "South Shroud (Quarrymill)"),
        new(30, 35, "Eastern La Noscea", "Eastern La Noscea"),
        new(35, 40, "Coerthas Central Highlands", "Coerthas Central Highlands"),
        new(40, 45, "Mor Dhona", "Mor Dhona"),
        new(45, 51, "Northern Thanalan", "Northern Thanalan"),
    ];

    public static ZoneBracket? ForLevel(int level) =>
        Brackets.FirstOrDefault(b => level >= b.MinLevel && level < b.MaxLevel);
}
