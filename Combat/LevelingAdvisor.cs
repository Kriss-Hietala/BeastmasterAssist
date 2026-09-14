using BeastmasterAssist.Data;

namespace BeastmasterAssist.Combat;

/// <summary>Suggests nearby-level overworld beasts as grinding/leveling spots.
/// Purely informational: it never moves the player or targets anything.</summary>
public sealed class LevelingAdvisor
{
    public sealed record Spot(string Name, string Location, int Level, bool Captured);

    public List<Spot> Suggest(int playerLevel, Func<int, bool> isCaptured, int count = 3)
    {
        if (playerLevel <= 0) return [];

        return BestiaryCatalog.All
            .Where(b => !b.Duty)
            .Where(b => b.Level <= playerLevel + 3)
            .OrderBy(b => Math.Abs(b.Level - playerLevel))
            .ThenByDescending(b => b.Level)
            .Take(count)
            .Select(b => new Spot(b.Name, b.LocalizedLocation(Data.Loc.Polish), b.Level, isCaptured(b.Id)))
            .ToList();
    }
}
