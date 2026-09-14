namespace BeastmasterAssist.Data;

public static class ProgressCatalog
{
    public static readonly IReadOnlyList<AchievementDef> Achievements =
    [
        A("hand1", "A Beast in the Hand I", "Forge pacts with 10 beasts.", null),
        A("hand2", "A Beast in the Hand II", "Forge pacts with 20 beasts.", null),
        A("hand3", "A Beast in the Hand III", "Forge pacts with 30 beasts.", null),
        A("hand4", "A Beast in the Hand IV", "Forge pacts with 40 beasts.", null),
        A("hand5", "A Beast in the Hand V", "Fill the Master's Bestiary (50).", "Reward at Kornago Merchant — Central Shroud (21.9, 22.7)"),
        A("crucible_contender", "Crucible Contender", "Top 300 DC in Season One Third Degree (from 24.09.2026).", "Tourmaline Golem Horn"),
        A("menagerie", "Master of the Menagerie", "Legendary rank on the first three boards and both Master's Boards.", "Guttler Unleashed + Augmented Kornago Hoplon"),
        A("rank25", "Beast Rank 25", "Any familiar at rank 25 (alternate appearance outside Crucible).", "Alternate appearance"),
    ];

    public static readonly IReadOnlyList<QuestDef> Quests =
    [
        Q("unlock", "Strangers in the Wood", 50, "Excited Adventurer, New Gridania (11.8, 13.6)", "Soul of the Beastmaster, Beast Herder's set, Cu Sith Gourd"),
        Q("wilds", "The Wilds Call", 1, "J'yhuh Tia", "Beast Tender's Hand Axe / Hoplon"),
        Q("hearts", "Hearts Aligned", 8, "J'yhuh Tia", "Beast Handler's Tabar / Pelta"),
        Q("lhu", "Master of the Lhu", 18, "J'yhuh Tia", "Beast Whisperer's Hand Axe / Round Shield"),
        Q("crucible", "Into the Crucible", 30, "J'yhuh Tia", "Beast Tamer's Coffer IL30"),
        Q("wit", "Wit as Weaponry", 30, "J'yhuh Tia", "Requires a Crucible board"),
        Q("gob", "Gobsmacked", 40, "J'yhuh Tia", "Beastwarden's Coffer IL40"),
        Q("blood", "Sin of Blood", 40, "Lauda", "Requires a Crucible board"),
        Q("path", "A Beastmaster's Path", 50, "J'yhuh Tia", "Beastmaster's Coffer IL90"),
        Q("bonds", "Bonds Unbroken", 50, "Lauda", "Cu Sith Horn"),
        Q("ffa", "Free for All", 50, "Sylmond", "Crucible Degrees"),
        Q("rematch", "Mastery Rematch", 50, "Sylmond", "Crucible Degrees"),
        Q("trails", "Blazing Trails", 50, "Sylmond", "Crucible Degrees / rankings"),
    ];

    public static readonly IReadOnlyList<GearPiece> ExclusiveGear =
    [
        G("herder_axe", "Beast Herder's Hand Axe", "MH", "Strangers in the Wood", 1, false, 0),
        G("herder_shield", "Beast Herder's Hoplon", "OH", "Strangers in the Wood", 1, false, 0),
        G("tender_axe", "Beast Tender's Hand Axe", "MH", "The Wilds Call", 1, false, 0),
        G("handler_axe", "Beast Handler's Tabar", "MH", "Hearts Aligned", 8, false, 0),
        G("whisper_axe", "Beast Whisperer's Hand Axe", "MH", "Master of the Lhu", 18, false, 0),
        G("tamer_set", "Beast Tamer's Coffer", "Set", "Into the Crucible", 30, false, 0),
        G("warden_set", "Beastwarden's Coffer", "Set", "Gobsmacked", 40, false, 0),
        G("bst_axe", "Beastmaster's Hand Axe", "MH", "A Beastmaster's Path", 90, true, 0),
        G("bst_hoplon", "Beastmaster's Hoplon", "OH", "A Beastmaster's Path", 90, true, 0),
        G("bst_horns", "Beastmaster's Horns", "Head", "IL90 coffer + remnants", 90, true, 4),
        G("bst_furs", "Beastmaster's Furs", "Body", "IL90 coffer + remnants", 90, true, 4),
        G("bst_boots", "Beastmaster's Boots", "Feet", "IL90 coffer + remnants", 90, true, 4),
        G("bst_earrings", "Beastmaster's Earrings", "Ears", "Kornago Merchant / remnants", 90, true, 0),
        G("guttler", "Guttler", "MH", "Crucible BiS base", 90, true, 0),
        G("guttler_unleashed", "Guttler Unleashed", "MH", "Master of the Menagerie", 90, false, 0),
        G("kornago_hoplon", "Kornago Hoplon", "OH", "Kornago Merchant", 90, true, 0),
        G("kornago_hoplon_aug", "Augmented Kornago Hoplon", "OH", "Master of the Menagerie", 90, false, 0),
        G("cu_sith_horn", "Cu Sith Horn", "Item", "Bonds Unbroken", 1, false, 0),
        G("tourmaline", "Tourmaline Golem Horn", "Mount", "Crucible Contender (top 300 DC)", 1, false, 0),
    ];

    private static AchievementDef A(string key, string name, string desc, string? reward) => new() { Key = key, Name = name, Description = desc, Reward = reward };
    private static QuestDef Q(string key, string name, int level, string npc, string rewards) => new() { Key = key, Name = name, Level = level, Npc = npc, Rewards = rewards };
    private static GearPiece G(string key, string name, string slot, string source, int il, bool up, int tier) => new() { Key = key, Name = name, Slot = slot, Source = source, ItemLevel = il, Upgradeable = up, UpgradeTier = tier };
}
