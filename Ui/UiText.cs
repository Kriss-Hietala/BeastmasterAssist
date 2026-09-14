namespace BeastmasterAssist.Ui;

/// <summary>Centralny katalog tekstow UI. Nazwy akcji i bestii pozostaja takie jak w kliencie FFXIV.</summary>
public static class UiText
{
    public static bool Polish { get; set; } = true;
    public static string T(string pl, string en) => Polish ? pl : en;

    public static string Bestiary => T("BESTIARIUSZ", "BESTIARY");
    public static string Progress => T("PROGRES", "PROGRESS");
    public static string Settings => T("USTAWIENIA", "SETTINGS");
    public static string Close => T("ZAMKNIJ", "CLOSE");
    public static string HideOverlay => T("UKRYJ OVERLAY", "HIDE OVERLAY");
    public static string All => T("WSZYSTKIE", "ALL");
    public static string Missing => T("BRAKUJACE", "MISSING");
    public static string Captured => T("ZLAPANE", "CAPTURED");
    public static string Duty => T("DUTY", "DUTY");
    public static string Search => T("Szukaj", "Search");
    public static string Filters => T("FILTRY", "FILTERS");
    public static string Details => T("SZCZEGOLY", "DETAILS");
    public static string Location => T("LOKACJA", "LOCATION");
    public static string Level => T("POZIOM", "LEVEL");
    public static string Kinship => T("KINSHIP", "KINSHIP");
    public static string Affinity => T("AFFINITY", "AFFINITY");
    public static string Trick => "TRICK";
    public static string TemperedRelease => "TEMPERED RELEASE";
    public static string Capture => T("LAPANIE", "CAPTURE");
    public static string CapturedCount(int count) => T($"Pakty: {count}/50", $"Pacts: {count}/50");
    public static string Achievement => T("ACHIEVEMENTY", "ACHIEVEMENTS");
    public static string Gear => T("EKWIPUNEK", "GEAR");
    public static string Quests => T("QUESTY", "QUESTS");
    public static string Complete => T("UKONCZONE", "COMPLETE");
    public static string Incomplete => T("NIEUKONCZONE", "INCOMPLETE");
    public static string Language => T("Jezyk interfejsu", "Interface language");
    public static string PolishUi => T("Polski", "Polish");
    public static string EnglishUi => T("Angielski", "English");
    public static string Overlay => "Overlay";
    public static string OverlayScale => T("Skala overlay", "Overlay scale");
    public static string LockOverlay => T("Zablokuj overlay", "Lock overlay");
    public static string Rotation => T("ROTACJA", "ROTATION");
    public static string Reactions => T("REAKCJE", "REACTIONS");
    public static string Mitigation => T("MITYGACJA", "MITIGATION");
    public static string CommandHelp => T("POMOC I KOMENDY", "HELP AND COMMANDS");
    public static string Commands => T(
        "/bstassist lub /bestia — otwiera Bestiariusz\n/bstassist overlay — wlacza lub ukrywa overlay\n/bstassist progress — otwiera Progres\n/bstassist config — otwiera Ustawienia\n/bstassist debug — zapisuje ID akcji do /xllog",
        "/bstassist or /bestia — opens Bestiary\n/bstassist overlay — toggles the overlay\n/bstassist progress — opens Progress\n/bstassist config — opens Settings\n/bstassist debug — writes action IDs to /xllog");

    public static void Sync(Configuration config) => Polish = config.PreferPolishUi;
}
