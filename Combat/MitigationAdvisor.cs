using BeastmasterAssist.Data;

namespace BeastmasterAssist.Combat;

public sealed class MitigationAdvisor
{
    public MitigationAdvisor(GameData data) { }

    public List<Advice> Advise(CombatState s)
    {
        var list = new List<Advice>();
        if (s.Player is null) return list;
        if (s.HpPct < 0.35f)
            list.Add(new Advice(AdviceKind.Mitigation, "Emergency", Loc.T("HP < 35%.", "HP < 35%."), 100));
        if (s.ActiveKinship == Kinship.None && s.Level >= 22)
            list.Add(new Advice(AdviceKind.Mitigation, "Borrow", Loc.T("Brak Kinship.", "No Kinship."), 90));
        if (!s.Beastskin && s.ActiveKinship == Kinship.Beastkin)
            list.Add(new Advice(AdviceKind.Mitigation, "Beastskin", Loc.T("Mitygacja fizyczna.", "Physical mit."), 80));
        if (!s.Vileskin && s.ActiveKinship == Kinship.Vilekin)
            list.Add(new Advice(AdviceKind.Mitigation, "Vileskin", Loc.T("Block + anti-KB.", "Block + anti-KB."), 85));
        if (!s.Scaleskin && s.ActiveKinship == Kinship.Scalekin)
            list.Add(new Advice(AdviceKind.Mitigation, "Scaleskin", Loc.T("Bariera magiczna.", "Magic barrier."), 82));
        return list.OrderByDescending(a => a.Priority).ToList();
    }
}
