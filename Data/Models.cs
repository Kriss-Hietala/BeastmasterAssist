namespace BeastmasterAssist.Data;

public enum InstinctColor { None, Rampant, Durant, Eldritch, Volant, Sunstrider, Moonstalker }
public enum Kinship { None, Beastkin, Vilekin, Cloudkin, Seedkin, Wavekin, Scalekin, Soulkin, Ashkin }
public enum AdviceKind { Gcd, Ogcd, Reaction, Mitigation, Capture, Swap }

public sealed record Advice(AdviceKind Kind, string Action, string Reason, int Priority);

public sealed class BeastEntry
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Monster { get; init; }
    public required Kinship Class { get; init; }
    public required InstinctColor Affinity { get; init; }
    public required string Habitat { get; init; }
    public required string Location { get; init; }
    public required int Level { get; init; }
    public required string Trick { get; init; }
    public required string TemperedRelease { get; init; }
    public required string Notes { get; init; }
    public bool Duty { get; init; }
    public string? GourdHint { get; init; }
}

public sealed class GearPiece
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required string Slot { get; init; }
    public required string Source { get; init; }
    public int ItemLevel { get; init; }
    public bool Upgradeable { get; init; }
    public int UpgradeTier { get; init; }
}

public sealed class AchievementDef
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public string? Reward { get; init; }
    public uint? ResolvedId { get; set; }
}

public sealed class QuestDef
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required int Level { get; init; }
    public required string Npc { get; init; }
    public required string Rewards { get; init; }
}
