using static BeastmasterAssist.Data.InstinctColor;
using static BeastmasterAssist.Data.Kinship;

namespace BeastmasterAssist.Data;

public static class BestiaryCatalog
{
    public static readonly IReadOnlyList<BeastEntry> All =
    [
        B(1, "Cu Sith", "Cu Sith", Beastkin, Rampant, "Quest", "Strangers in the Wood - Cu Sith Gourd", "Strangers in the Wood - Cu Sith Gourd", 1, "Rake", "Relentless Rake", "Starting familiar from the unlock quest.", "Startowy familiar z questa odblokowujacego.", false, null, null),
        B(2, "Squirrel", "Ground Squirrel", Beastkin, Rampant, "Central Shroud", "Central Shroud (23, 16) Blue Badger Gate", "Central Shroud (23, 16) Blue Badger Gate", 1, "Somersault Slash", "Scamper", "Haste on allies.", "Haste na sojusznikow.", false, null, null),
        B(3, "Lamb", "Lost Lamb", Beastkin, Rampant, "Middle La Noscea", "Middle La Noscea (22, 25) Zephyr Drift", "Middle La Noscea (22, 25) Zephyr Drift", 3, "Fleece Butt", "Lullaby", "Sleep plus ally sleep resist.", "Sleep oraz odpornosc sojusznikow na sen.", false, null, null),
        B(4, "Pugil", "Pugil", Wavekin, Durant, "Middle La Noscea", "Middle La Noscea (21, 22) Summerford", "Middle La Noscea (21, 22) Summerford", 4, "Screwdriver", "Water Wall", "Wavekin for Quelling Wave.", "Wavekin do Quelling Wave.", false, null, null),
        B(5, "Opo-opo", "Opo-opo", Beastkin, Rampant, "North Shroud", "North Shroud (27, 23) Treespeak", "North Shroud (27, 23) Treespeak", 5, "Stone Throw", "Pins and Nails", "ST paralysis.", "Paraliz na jeden cel.", false, null, null),
        B(6, "Dodo", "Wild Dodo", Cloudkin, Eldritch, "Lower La Noscea", "Lower La Noscea (27, 21) Cedarwood", "Lower La Noscea (27, 21) Cedarwood", 4, "Fowl Stench", "Strut", "Blind.", "Oslepienie.", false, null, null),
        B(7, "Coblyn", "Rusty Coblyn", Soulkin, Eldritch, "Western Thanalan", "Western Thanalan (22, 27) Hammerlea", "Western Thanalan (22, 27) Hammerlea", 6, "Bestial Thunder", "Vulcanize", "Soul Crush kinship.", "Kinship Soul Crush.", false, null, null),
        B(8, "Diremite", "Diremite", Vilekin, Rampant, "Central Shroud", "Central Shroud (19, 18)", "Central Shroud (19, 18)", 10, "Deadly Thrust", "Silkscreen", "Knockback plus bind.", "Odrzucenie oraz spowolnienie ruchu.", false, null, null),
        B(9, "Megalocrab", "Megalocrab", Wavekin, Durant, "Middle La Noscea", "Middle La Noscea Three-malm Bend", "Middle La Noscea, Three-malm Bend", 10, "Drenching Blow", "Bubble Shower", "Water resist down.", "Obniza odpornosc na water.", false, null, null),
        B(10, "Wespe", "Huge Hornet", Vilekin, Volant, "Central Thanalan", "Central Thanalan Spineless Basin", "Central Thanalan, Spineless Basin", 1, "Sharp Sting", "Final Sting", "Final Sting dismisses familiars.", "Final Sting odprawia familiary.", false, null, null),
        B(11, "Vulture", "Arbor Buzzard", Cloudkin, Volant, "Western Thanalan", "Western Thanalan The Footfalls", "Western Thanalan, The Footfalls", 13, "Wing Cutter", "Bloodcurdling Caw", "Dispel 1 enemy buff.", "Zdejmuje 1 buff wroga.", false, null, null),
        B(12, "Mandragora", "Tiny Mandragora", Seedkin, Rampant, "Middle La Noscea", "Middle La Noscea Summerford east", "Middle La Noscea, Summerford wschod", 5, "Budbutt", "Heirloom Scream", "Heavy AoE.", "Heavy na obszar.", false, null, null),
        B(13, "Geshunpest", "Geshunpest", Ashkin, Eldritch, "Central Shroud", "Central Shroud near Tam-Tara", "Central Shroud, przy Tam-Tara", 14, "Dark Thunder", "Odium", "Early ashkin.", "Wczesny ashkin.", false, null, null),
        B(14, "Puk", "Puk Hatchling", Scalekin, Rampant, "Middle La Noscea", "Middle La Noscea Summerford west", "Middle La Noscea, Summerford zachod", 4, "Fireball", "Tail Chase", "Knockback.", "Odrzucenie.", false, null, null),
        B(15, "Crab", "Thickshell", Wavekin, Durant, "Western Thanalan", "Western Thanalan The Footfalls", "Western Thanalan, The Footfalls", 13, "Bubble Shower", "Hundred Fists", "Self haste.", "Haste na siebie.", false, null, null),
        B(16, "Mantis", "Killer Mantis", Vilekin, Durant, "Western La Noscea", "Western La Noscea Camp Skull Valley", "Western La Noscea, Camp Skull Valley", 16, "Standing Chine", "Eerie Soundwave", "Phys vuln up.", "Zwieksza phys vuln.", false, null, null),
        B(17, "Slime", "Ichorous Ire", Ashkin, Eldritch, "Copperbell Mines", "Dungeon: Copperbell Mines", "Dungeon: Copperbell Mines", 17, "Digest", "Syrup", "Duty catch.", "Lapanie w duty.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
        B(18, "Dullahan", "Doctore", Soulkin, Durant, "Halatali", "Dungeon: Halatali", "Dungeon: Halatali", 20, "Iron Justice", "King's Will", "Duty catch.", "Lapanie w duty.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
        B(19, "Bat", "Cave Bat", Cloudkin, Volant, "Lower La Noscea", "Lower La Noscea Cedarwood", "Lower La Noscea, Cedarwood", 7, "Blood Drain", "Ultrasonics", "Ally cleanse.", "Cleanse dla sojusznika.", false, null, null),
        B(20, "Flying Trap", "Roselet", Seedkin, Volant, "Central Shroud", "Central Shroud Bentbranch", "Central Shroud, Bentbranch", 10, "Sour Sough", "Necrotic Nectar", "Magic vuln up.", "Zwieksza magic vuln.", false, null, null),
        B(21, "Ziz", "Rothlyt Pelican", Scalekin, Durant, "Western La Noscea", "Western La Noscea south of Camp Skull Valley", "Western La Noscea, na poludnie od Camp Skull Valley", 16, "Ice Breath", "Petribreath", "Petrify.", "Petryfikacja.", false, null, null),
        B(22, "Sabotender", "Cactuar", Seedkin, Rampant, "Western Thanalan", "Western Thanalan Eighty Sins of Sasamo", "Western Thanalan, Eighty Sins of Sasamo", 3, "Natural Needles", "Hypodermic Hustle", "Keen edge on allies.", "Keen edge na sojusznikow.", false, null, null),
        B(23, "Golem", "Sandstone Golem", Soulkin, Eldritch, "Southern Thanalan", "Southern Thanalan near Qarn", "Southern Thanalan, przy Qarn", 29, "Boulder Clap", "Rockslide", "Earth resist down.", "Obniza odpornosc earth.", false, null, null),
        B(24, "Apkallu", "Apkallu", Cloudkin, Durant, "Eastern La Noscea", "Eastern La Noscea Gullperch Tower", "Eastern La Noscea, Gullperch Tower", 30, "Flying Sardine", "Regurgitate", "Slow.", "Spowolnienie.", false, null, null),
        B(25, "Adamantoise", "Giant Tortoise", Scalekin, Eldritch, "Central Thanalan", "Central Thanalan", "Central Thanalan", 12, "Bestial Thunder II", "Harden Shell", "Phys mit.", "Mitygacja fizyczna.", false, null, null),
        B(26, "Buffalo", "Wounded Aurochs", Beastkin, Rampant, "Middle La Noscea", "Middle La Noscea The Cookpot", "Middle La Noscea, The Cookpot", 8, "Heave", "War Cry", "Stun AoE.", "Stun na obszar.", false, null, null),
        B(27, "Uragnite", "Scaphite", Wavekin, Durant, "Western Thanalan", "Western Thanalan The Footfalls", "Western Thanalan, The Footfalls", 14, "Frost Breath", "Gas Shell", "Poison AoE.", "Poison na obszar.", false, null, null),
        B(28, "Worm", "Sandworm", Vilekin, Eldritch, "Southern Thanalan", "Southern Thanalan Sagolii Desert", "Southern Thanalan, Sagolii Desert", 32, "Sand Breath", "Bottomless Desert", "Heavy.", "Heavy.", false, null, null),
        B(29, "Spriggan", "Spriggan Graverobber", Soulkin, Rampant, "Central Thanalan", "Central Thanalan Sil'dih", "Central Thanalan, Sil'dih", 7, "Romp", "Frenetic Flurry", "7-hit blunt.", "Siedem uderzen blunt.", false, null, null),
        B(30, "Goobbue", "Mossless Goobbue", Beastkin, Rampant, "Lower La Noscea", "Lower La Noscea Cedarwood", "Lower La Noscea, Cedarwood", 12, "Beatdown", "Moldy Sneeze", "Sickness.", "Choroba.", false, null, null),
        B(31, "Gigantoad", "Rivertoad", Wavekin, Eldritch, "Lower La Noscea", "Lower La Noscea Moraby Bay", "Lower La Noscea, Moraby Bay", 4, "Bestial Blizzard II", "Sticky Tongue", "Draw-in.", "Przyciaganie.", false, null, null),
        B(32, "Colibri", "Colibri", Cloudkin, Volant, "Eastern La Noscea", "Eastern La Noscea Hidden Falls", "Eastern La Noscea, Hidden Falls", 33, "Loop", "Pecking Flurry", "5-hit piercing.", "Piec uderzen piercing.", false, null, null),
        B(33, "Coeurl", "Coeurl", Beastkin, Eldritch, "Outer La Noscea", "Outer La Noscea Floating City of Nym", "Outer La Noscea, Floating City of Nym", 34, "Blaster", "Charged Whisker", "Paralysis.", "Paraliz.", false, null, null),
        B(34, "Raptor", "Anole", Scalekin, Durant, "Central Shroud", "Central Shroud Naked Rock", "Central Shroud, Naked Rock", 9, "Frost Breath", "Foul Breath", "Fire resist down.", "Obniza odpornosc fire.", false, null, null),
        B(35, "Drake", "Sundrake", Scalekin, Rampant, "Southern Thanalan", "Southern Thanalan Sagolii SE", "Southern Thanalan, Sagolii SE", 32, "Burning Cyclone", "Smoldering Scales", "Magic mit.", "Mitygacja magiczna.", false, null, null),
        B(36, "Treant", "Treant Sapling", Seedkin, Eldritch, "Central Shroud", "Central Shroud Blue Badger Gate", "Central Shroud, Blue Badger Gate", 12, "Acorn Bomb", "Arboreal Storm", "Nightmares.", "Nightmares.", false, null, null),
        B(37, "Antling", "Myrmidon Family", Vilekin, Rampant, "Cutter's Cry", "Dungeon: Cutter's Cry", "Dungeon: Cutter's Cry", 38, "Mandible Bite", "Formic Pheromones", "Duty catch.", "Lapanie w duty.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
        B(38, "Chimera", "Chimera", Beastkin, Rampant, "Cutter's Cry", "Dungeon: Cutter's Cry", "Dungeon: Cutter's Cry", 38, "the Lion's Breath", "the Ram's Voice", "Duty catch.", "Lapanie w duty.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
        B(39, "Morbol", "Stroper", Seedkin, Rampant, "Central Shroud", "Central Shroud Hopeseed Pond", "Central Shroud, Hopeseed Pond", 31, "Vine Probe", "Bad Breath", "Slow, blind, paralysis.", "Spowolnienie, oslepienie, paraliz.", false, null, null),
        B(40, "Ghost", "Bogy", Ashkin, Volant, "Middle La Noscea", "Middle La Noscea Seasong Grotto", "Middle La Noscea, Seasong Grotto", 7, "Fell Gale", "Curse", "Doom. Ashkin.", "Doom. Ashkin.", false, null, null),
        B(41, "Salamander", "Back Eft", Wavekin, Durant, "Central Shroud", "Central Shroud Jadeite Thick", "Central Shroud, Jadeite Thick", 6, "Brackish Rain", "Peculiar Light", "Magic vuln up.", "Zwieksza magic vuln.", false, null, null),
        B(42, "Cobra", "Lake Cobra", Scalekin, Durant, "Mor Dhona", "Mor Dhona south", "Mor Dhona, poludnie", 45, "Dripping Fang", "Stone Gaze", "Petrify.", "Petryfikacja.", false, null, null),
        B(43, "Hydra", "Hydra", Scalekin, Durant, "A Relic Reborn: the Hydra", "Trial: A Relic Reborn: the Hydra", "Trial: A Relic Reborn: the Hydra", 50, "Main Trap", "White Breath", "Duty catch.", "Lapanie w duty.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
        B(44, "Damselfly", "Gadfly", Vilekin, Volant, "The Lost City of Amdapor", "Dungeon: Lost City of Amdapor", "Dungeon: Lost City of Amdapor", 50, "Cursed Sphere", "Venom", "Duty catch.", "Lapanie w duty.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
        B(45, "Rotting Goobbue", "Decaying Gourmand", Ashkin, Eldritch, "The Lost City of Amdapor", "Dungeon: Lost City of Amdapor", "Dungeon: Lost City of Amdapor", 50, "Dirty Sneeze", "Inhale", "Duty ashkin.", "Ashkin z duty.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
        B(46, "Zu", "Zu", Cloudkin, Volant, "Pharos Sirius", "Dungeon: Pharos Sirius", "Dungeon: Pharos Sirius", 50, "Flying Frenzy", "Breath Wing", "Duty catch.", "Lapanie w duty.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
        B(47, "Ice Golem", "Wandil", Soulkin, Durant, "Snowcloak", "Dungeon: Snowcloak", "Dungeon: Snowcloak", 50, "Ice Guillotine", "Frozen Heart", "Duty catch.", "Lapanie w duty.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
        B(48, "Karlabos", "Karlabos", Wavekin, Durant, "Sastasha (Hard)", "Dungeon: Sastasha (Hard)", "Dungeon: Sastasha (Hard)", 50, "Impale", "Tail Screw", "Duty catch.", "Lapanie w duty.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
        B(49, "Rafflesia", "Rafflesia", Seedkin, Eldritch, "Second Coil of Bahamut - Turn 1", "Raid: T9", "Raid: T9", 50, "Bloody Caress", "Blighted Bouquet", "Unrestricted Party OK.", "Unrestricted Party jest ok.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
        B(50, "Behemoth", "King Behemoth", Beastkin, Eldritch, "Labyrinth of the Ancients", "Alliance: Labyrinth of the Ancients", "Alliance: Labyrinth of the Ancients", 50, "Thunderbolt", "Meteor", "Stop DPS near 5% HP.", "Zatrzymaj DPS przy okolo 5% HP.", true, "Kornago gourd", "Dynia od Kornago Merchanta"),
    ];

    private static BeastEntry B(int id, string name, string monster, Kinship k, InstinctColor aff, string habitat, string location, string locationPl, int level, string trick, string tempered, string notes, string notesPl, bool duty, string? gourd, string? gourdPl) => new()
    {
        Id = id, Name = name, Monster = monster, Class = k, Affinity = aff, Habitat = habitat,
        Location = location, LocationPl = locationPl, Level = level, Trick = trick, TemperedRelease = tempered,
        Notes = notes, NotesPl = notesPl, Duty = duty, GourdHint = gourd, GourdHintPl = gourdPl
    };

    public static BeastEntry? Get(int id) => All.FirstOrDefault(b => b.Id == id);

    public static string BeastModeName(Kinship k) => k switch
    {
        Kinship.Beastkin => "Beastskin",
        Kinship.Vilekin => "Vileskin",
        Kinship.Cloudkin => "Cloud Skim",
        Kinship.Seedkin => "Seedsower",
        Kinship.Wavekin => "Quelling Wave",
        Kinship.Scalekin => "Scaleskin",
        Kinship.Soulkin => "Soul Crush",
        Kinship.Ashkin => "Scouring Ash",
        _ => "Beast Mode"
    };
}
