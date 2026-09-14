using BeastmasterAssist.Data;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace BeastmasterAssist.Combat;

public sealed class CombatState
{
    private readonly IClientState clientState;
    private readonly ITargetManager targets;
    private readonly ICondition condition;
    private readonly GameData data;

    public IPlayerCharacter? Player { get; private set; }
    public IBattleChara? Target { get; private set; }
    public bool InCombat { get; private set; }
    public byte Level { get; private set; }
    public float HpPct { get; private set; }
    public float TargetHpPct { get; private set; }
    public float TargetDistance { get; private set; }
    public bool TargetCasting { get; private set; }
    public string TargetCastName { get; private set; } = "";
    public bool HasOneWithNature { get; private set; }
    public bool HasLingeringVantage { get; private set; }
    public bool HasInterestCaptured { get; private set; }
    public float InterestCapturedRemain { get; private set; }
    public InstinctColor LastHeart { get; private set; }
    public float HeartRemain { get; private set; }
    public bool Sunstrider { get; private set; }
    public bool Moonstalker { get; private set; }
    public int MasteredInstinct { get; private set; }
    public int NaturalInstinct { get; private set; }
    public Kinship ActiveKinship { get; private set; }
    public bool Beastskin { get; private set; }
    public bool Vileskin { get; private set; }
    public bool Scaleskin { get; private set; }
    public int ComboStep { get; private set; }
    public bool FamiliarOut { get; private set; }
    public int PlayerTp { get; private set; }
    public int FamiliarTp { get; private set; }
    public List<string> PlayerStatusNames { get; } = [];

    public CombatState(IClientState clientState, ITargetManager targets, ICondition condition, GameData data, IPluginLog log)
    {
        this.clientState = clientState;
        this.targets = targets;
        this.condition = condition;
        this.data = data;
    }

    public void Tick()
    {
        Player = clientState.LocalPlayer;
        Target = targets.Target as IBattleChara;
        InCombat = condition[ConditionFlag.InCombat];
        PlayerStatusNames.Clear();
        if (Player is null) return;

        Level = Player.Level;
        HpPct = Player.MaxHp == 0 ? 0 : Player.CurrentHp / (float)Player.MaxHp;
        if (Target is not null)
        {
            TargetHpPct = Target.MaxHp == 0 ? 0 : Target.CurrentHp / (float)Target.MaxHp;
            var dx = Player.Position.X - Target.Position.X;
            var dz = Player.Position.Z - Target.Position.Z;
            TargetDistance = MathF.Sqrt(dx * dx + dz * dz);
            TargetCasting = Target.IsCasting;
            TargetCastName = Target.IsCasting ? $"#{Target.CastActionId}" : "";
        }
        else
        {
            TargetHpPct = 0;
            TargetDistance = 999;
            TargetCasting = false;
            TargetCastName = "";
        }

        HasOneWithNature = Has(Player, "One with Nature");
        HasLingeringVantage = Has(Player, "Lingering Vantage");
        Sunstrider = Has(Player, "Sunstrider");
        Moonstalker = Has(Player, "Moonstalker");
        Beastskin = Has(Player, "Beastskin");
        Vileskin = Has(Player, "Vileskin");
        Scaleskin = Has(Player, "Scaleskin");
        MasteredInstinct = Stacks(Player, "Mastered Instinct");
        NaturalInstinct = Stacks(Player, "Natural Instinct");
        ActiveKinship = ReadKinship(Player);
        LastHeart = ReadHeart(Player, out var hr);
        HeartRemain = hr;
        FamiliarOut = HasOneWithNature || HasLingeringVantage || Has(Player, "Cover");
        HasInterestCaptured = Target is not null && HasRemain(Target, "Interest Captured", out var ic);
        InterestCapturedRemain = ic;

        foreach (var st in Player.StatusList)
        {
            var n = st.GameData.ValueNullable?.Name.ToString();
            if (!string.IsNullOrEmpty(n)) PlayerStatusNames.Add(n);
        }

        ReadGaugeFallback();
        ReadCombo();
    }

    public bool ActionReady(string name)
    {
        var id = data.Action(name);
        if (id == 0) return true;
        unsafe
        {
            var am = ActionManager.Instance();
            if (am is null) return true;
            return am->GetActionStatus(ActionType.Action, id) == 0;
        }
    }

    private void ReadCombo()
    {
        unsafe
        {
            var am = ActionManager.Instance();
            if (am is null) { ComboStep = 0; return; }
            var combo = am->Combo.Action;
            if (combo == data.Action("Smash Axe")) ComboStep = 1;
            else if (combo == data.Action("Axeblade Bite")) ComboStep = 2;
            else ComboStep = 0;
        }
    }

    private void ReadGaugeFallback()
    {
        if (PlayerTp < 100 && ComboStep >= 2) PlayerTp = Math.Min(250, PlayerTp + 15);
        if (!InCombat)
        {
            PlayerTp = Math.Max(0, PlayerTp - 2);
            FamiliarTp = Math.Max(0, FamiliarTp - 2);
        }
        if (HasOneWithNature && FamiliarTp < 80) FamiliarTp = 80;
        if (MasteredInstinct > 0 && PlayerTp < 100) PlayerTp = 100;
        if (NaturalInstinct > 0 && FamiliarTp < 100) FamiliarTp = 100;
        PlayerTp = Math.Clamp(PlayerTp, 0, 250);
        FamiliarTp = Math.Clamp(FamiliarTp, 0, 250);
    }

    private Kinship ReadKinship(IBattleChara player)
    {
        if (Has(player, "Beast Kinship") || Beastskin) return Kinship.Beastkin;
        if (Has(player, "Vile Kinship") || Vileskin) return Kinship.Vilekin;
        if (Has(player, "Cloud Kinship")) return Kinship.Cloudkin;
        if (Has(player, "Seed Kinship")) return Kinship.Seedkin;
        if (Has(player, "Wave Kinship")) return Kinship.Wavekin;
        if (Has(player, "Scale Kinship") || Scaleskin) return Kinship.Scalekin;
        if (Has(player, "Soul Kinship")) return Kinship.Soulkin;
        if (Has(player, "Ash Kinship")) return Kinship.Ashkin;
        return Kinship.None;
    }

    private InstinctColor ReadHeart(IBattleChara player, out float remain)
    {
        if (HasRemain(player, "Heart of Rampant", out remain) || HasRemain(player, "Rampant", out remain)) return InstinctColor.Rampant;
        if (HasRemain(player, "Heart of Durant", out remain) || HasRemain(player, "Durant", out remain)) return InstinctColor.Durant;
        if (HasRemain(player, "Heart of Eldritch", out remain) || HasRemain(player, "Eldritch", out remain)) return InstinctColor.Eldritch;
        if (HasRemain(player, "Heart of Volant", out remain) || HasRemain(player, "Volant", out remain)) return InstinctColor.Volant;
        remain = 0;
        return InstinctColor.None;
    }

    private static bool Has(IBattleChara chara, string name)
    {
        foreach (var st in chara.StatusList)
        {
            var n = st.GameData.ValueNullable?.Name.ToString();
            if (n is not null && n.Contains(name, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    private static bool HasRemain(IBattleChara chara, string name, out float remain)
    {
        foreach (var st in chara.StatusList)
        {
            var n = st.GameData.ValueNullable?.Name.ToString();
            if (n is not null && n.Contains(name, StringComparison.OrdinalIgnoreCase))
            {
                remain = st.RemainingTime;
                return true;
            }
        }
        remain = 0;
        return false;
    }

    private static int Stacks(IBattleChara chara, string name)
    {
        foreach (var st in chara.StatusList)
        {
            var n = st.GameData.ValueNullable?.Name.ToString();
            if (n is not null && n.Contains(name, StringComparison.OrdinalIgnoreCase))
                return Math.Max(1, st.Param);
        }
        return 0;
    }
}
