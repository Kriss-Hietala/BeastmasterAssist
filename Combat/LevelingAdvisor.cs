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

        var pool = BestiaryCatalog.All
            .Where(b => !b.Duty)
            .Where(b => b.Level <= playerLevel + 3)
            .ToList();

        // "Gdzie levelowac" ma sens tylko dla bestii, ktorych jeszcze nie mamy -
        // juz zlapana bestia niczego tu nie wnosi, wiec nie dopelniamy nia listy,
        // nawet jesli w zasiegu brakuje niezlapanych kandydatow (lista wtedy po
        // prostu bedzie krotsza niz `count`, ewentualnie pusta).
        return pool
            .Where(b => !isCaptured(b.Id))
            .OrderBy(b => Math.Abs(b.Level - playerLevel))
            .ThenByDescending(b => b.Level)
            .Take(count)
            .Select(b => new Spot(b.Name, b.LocalizedLocation(Data.Loc.Polish), b.Level, false))
            .ToList();
    }

    // Realna podpowiedz strefy do zdobywania EXP postaci/joba (FATE grinding),
    // calkowicie niezalezna od bestiariusza - odpowiada wprost na "dokad isc,
    // zeby wbic poziom", w odroznieniu od Suggest() powyzej. Beastmaster ma cap
    // na Lv50, wiec brak dopasowanego przedzialu oznacza po prostu max level.
    public string SuggestXpZone(int playerLevel)
    {
        var bracket = LevelingZoneCatalog.ForLevel(playerLevel);
        if (bracket is null)
        {
            return Loc.T(
                "Lv50 to maksymalny poziom Beastmastera - skup sie na Master's Bestiary i Crucible.",
                "Lv50 is Beastmaster's level cap - focus on the Master's Bestiary and the Crucible instead.");
        }

        return Loc.Polish ? bracket.ZonePl : bracket.Zone;
    }
}
