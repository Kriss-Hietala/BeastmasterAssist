using BeastmasterAssist.Data;

namespace BeastmasterAssist.Combat;

public sealed class RotationAdvisor
{
    private readonly Configuration config;
    public RotationAdvisor(GameData data, Configuration config) => this.config = config;

    public List<Advice> Advise(CombatState s)
    {
        var list = new List<Advice>();
        if (s.Player is null) return list;
        var lv = s.Level;

        if (!s.FamiliarOut && lv >= 1)
            list.Add(new Advice(AdviceKind.Ogcd, "Battlehorn", Loc.T("Przywolaj familiary.", "Summon a familiar."), 100));

        if (s.HasOneWithNature && lv >= 18)
        {
            if (lv >= 22 && s.ActiveKinship == Kinship.None)
                list.Add(new Advice(AdviceKind.Ogcd, "Borrow", Loc.T("Wez Kinship.", "Take Kinship."), 90));
            else
                list.Add(new Advice(AdviceKind.Ogcd, "Tempered Release", Loc.T("Skill familiary + Lingering Vantage.", "Familiar skill + Lingering Vantage."), 85));
        }

        if (s.FamiliarOut && s.HasLingeringVantage && lv >= 6)
            list.Add(new Advice(AdviceKind.Swap, "Parting Blow", Loc.T("Burst, potem 90s CD na rog.", "Burst, then 90s horn CD."), 80));

        if (lv >= 8 && s.FamiliarTp >= 100 && s.PlayerTp >= 100)
        {
            var pair = NextClockwise(s);
            list.Add(new Advice(AdviceKind.Ogcd, $"Trick → {pair}", Loc.T("Okno 7s, zgodnie z zegarem.", "7s window, clockwise."), 95));
        }
        else if (lv >= 4 && s.PlayerTp >= 100)
        {
            var axe = InstinctAxe(s);
            if (config.HoldTpForUniversality && s.PlayerTp >= 220 && lv >= 50 && !(s.Sunstrider || s.Moonstalker))
                list.Add(new Advice(AdviceKind.Gcd, axe, Loc.T("Trzymaj TP na Universality.", "Hold TP for Universality."), 70));
            else
                list.Add(new Advice(AdviceKind.Gcd, axe, Loc.T("Instinctual weaponskill.", "Instinctual weaponskill."), 70));
        }

        if (lv >= 50 && s.PlayerTp >= 250)
            list.Add(new Advice(AdviceKind.Gcd, UniversalityFinisher(s), Loc.T("250 TP = Sunstrider/Moonstalker.", "250 TP upgrades the axe."), 98));

        if (lv >= 28 && s.MasteredInstinct >= 2 && s.PlayerTp < 100)
            list.Add(new Advice(AdviceKind.Ogcd, "Rally", Loc.T("Wydaj Mastered Instinct.", "Spend Mastered Instinct."), 75));
        if (lv >= 40 && s.NaturalInstinct >= 2 && s.FamiliarTp < 100)
            list.Add(new Advice(AdviceKind.Ogcd, "Rallying Cheer", Loc.T("Wydaj Natural Instinct.", "Spend Natural Instinct."), 74));

        if (lv >= 24 && s.TargetDistance > 6 && s.ActionReady("Shield Charge"))
            list.Add(new Advice(AdviceKind.Ogcd, "Shield Charge", Loc.T("Gap closer 20 y.", "20y gap closer."), 40));

        list.Add(new Advice(AdviceKind.Gcd, ComboGcd(s), Loc.T("1-2-3 filler TP.", "1-2-3 TP filler."), 20));
        return list.OrderByDescending(a => a.Priority).ToList();
    }

    private static string ComboGcd(CombatState s) => s.ComboStep switch { 1 => "Axeblade Bite", 2 => "Shieldsplitter", _ => "Smash Axe" };

    private static string NextClockwise(CombatState s) => s.LastHeart switch
    {
        InstinctColor.Volant => "Avalanche Axe",
        InstinctColor.Rampant => "Mistral Axe",
        InstinctColor.Durant => "Spinning Axe",
        InstinctColor.Eldritch => "Gale Axe",
        _ => "Trick"
    };

    private static string InstinctAxe(CombatState s)
    {
        if (s.Level >= 50 && s.PlayerTp >= 250) return UniversalityFinisher(s);
        return s.LastHeart switch
        {
            InstinctColor.Volant => "Avalanche Axe",
            InstinctColor.Rampant => "Mistral Axe",
            InstinctColor.Durant => "Spinning Axe",
            InstinctColor.Eldritch => "Gale Axe",
            _ => s.Level >= 16 ? "Gale Axe" : "Avalanche Axe"
        };
    }

    private static string UniversalityFinisher(CombatState s)
    {
        if (s.Sunstrider && !s.Moonstalker) return "Hawkish Talons / Calamity";
        if (s.Moonstalker && !s.Sunstrider) return "Brutal Rage / Risen Fall";
        return "Brutal Rage";
    }
}
