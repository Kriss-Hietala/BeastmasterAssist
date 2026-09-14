using BeastmasterAssist.Data;

namespace BeastmasterAssist.Combat;

public sealed class ReactionAdvisor
{
    public ReactionAdvisor(GameData data) { }

    public List<Advice> Advise(CombatState s)
    {
        var list = new List<Advice>();
        if (s.Player is null) return list;
        if (s.ActiveKinship == Kinship.Soulkin && s.TargetCasting)
            list.Add(new Advice(AdviceKind.Reaction, "Soul Crush", Loc.T($"Interrupt {s.TargetCastName}.", $"Interrupt {s.TargetCastName}."), 100));
        if (s.ActiveKinship == Kinship.Ashkin)
            list.Add(new Advice(AdviceKind.Reaction, "Scouring Ash", Loc.T("Cleanse debuffa.", "Cleanse a debuff."), 95));
        if (s.ActiveKinship == Kinship.Vilekin)
            list.Add(new Advice(AdviceKind.Reaction, "Vileskin", Loc.T("Anti-KB + block.", "Anti-KB + block."), 70));
        if (s.ActiveKinship == Kinship.Scalekin)
            list.Add(new Advice(AdviceKind.Reaction, "Scaleskin", Loc.T("Bariera magiczna.", "Magic barrier."), 80));
        if (s.ActiveKinship == Kinship.Beastkin && !s.Beastskin)
            list.Add(new Advice(AdviceKind.Reaction, "Beastskin", Loc.T("-25% phys.", "-25% phys."), 55));
        if (s.ActiveKinship == Kinship.Cloudkin)
            list.Add(new Advice(AdviceKind.Reaction, "Cloud Skim", Loc.T("Dash / evasion.", "Dash / evasion."), 40));
        if (s.ActiveKinship == Kinship.Seedkin)
            list.Add(new Advice(AdviceKind.Reaction, "Seedsower", Loc.T("-10% dmg wrogow.", "-10% enemy damage."), 60));
        if (s.ActiveKinship == Kinship.Wavekin)
            list.Add(new Advice(AdviceKind.Reaction, "Quelling Wave", Loc.T("Tag 30 y / strip.", "30y tag / strip."), 50));
        return list.OrderByDescending(a => a.Priority).ToList();
    }
}
