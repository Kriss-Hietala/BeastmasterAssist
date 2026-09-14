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
    public string? LocationPl { get; init; }
    public required int Level { get; init; }
    public required string Trick { get; init; }
    public required string TemperedRelease { get; init; }
    public required string Notes { get; init; }
    public string? NotesPl { get; init; }
    public bool Duty { get; init; }
    public string? GourdHint { get; init; }
    public string? GourdHintPl { get; init; }

    public string LocalizedLocation(bool polish) => polish ? (LocationPl ?? Location) : Location;
    public string LocalizedNotes(bool polish) => polish ? (NotesPl ?? Notes) : Notes;
    public string? LocalizedGourdHint(bool polish) => polish ? (GourdHintPl ?? GourdHint) : GourdHint;
}

public sealed class GearPiece
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required string Slot { get; init; }
    public required string Source { get; init; }
    public string? SourcePl { get; init; }
    public int ItemLevel { get; init; }
    public bool Upgradeable { get; init; }
    public int UpgradeTier { get; init; }

    public string LocalizedSource(bool polish) => polish ? (SourcePl ?? Source) : Source;
}

public sealed class AchievementDef
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public string? DescriptionPl { get; init; }
    public string? Reward { get; init; }
    public string? RewardPl { get; init; }
    public uint? ResolvedId { get; set; }

    public string LocalizedDescription(bool polish) => polish ? (DescriptionPl ?? Description) : Description;
    public string? LocalizedReward(bool polish) => polish ? (RewardPl ?? Reward) : Reward;
}

public sealed class QuestDef
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required int Level { get; init; }
    public required string Npc { get; init; }
    public required string Rewards { get; init; }
    public string? RewardsPl { get; init; }

    public string LocalizedRewards(bool polish) => polish ? (RewardsPl ?? Rewards) : Rewards;
}
