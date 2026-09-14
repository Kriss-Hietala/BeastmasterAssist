namespace BeastmasterAssist.Ui;

/// <summary>Centralny katalog tekstow UI. Nazwy akcji i bestii z klienta FFXIV pozostaja po angielsku.</summary>
public static class UiText
{
    public static bool Polish { get; set; } = true;
    public static string T(string pl, string en) => Polish ? pl : en;

    public static string Bestiary => T("Bestiariusz", "Bestiary");
    public static string Progress => T("Progres", "Progress");
    public static string Settings => T("Ustawienia", "Settings");
    public static string Close => T("Zamknij", "Close");
    public static string HideOverlay => T("Ukryj overlay", "Hide overlay");
    public static string All => T("Wszystkie", "All");
    public static string Missing => T("Brakujace", "Missing");
    public static string Captured => T("Zlapane", "Captured");
    public static string Duty => "Duty";
    public static string Search => T("Szukaj", "Search");
    public static string Filters => T("Filtry", "Filters");
    public static string Details => T("Szczegoly", "Details");
    public static string Location => T("Lokacja", "Location");
    public static string Level => T("Poziom", "Level");
    public static string KinshipLabel => "Kinship";
    public static string AffinityLabel => "Affinity";
    public static string Trick => "Trick";
    public static string TemperedRelease => "Tempered Release";
    public static string Capture => T("Lapanie", "Capture");
    public static string CapturedCount(int count) => T($"Pakty: {count}/50", $"Pacts: {count}/50");
    public static string MissingCount(int count) => T($"{count} brakujacych", $"{count} missing");
    public static string Achievement => T("Achievementy", "Achievements");
    public static string Gear => T("Ekwipunek", "Gear");
    public static string Quests => T("Questy", "Quests");
    public static string Complete => T("Ukonczone", "Complete");
    public static string Incomplete => T("Nieukonczone", "Incomplete");
    public static string Reward => T("Nagroda", "Reward");
    public static string Source => T("Zrodlo", "Source");
    public static string Language => T("Jezyk interfejsu", "Interface language");
    public static string PolishUi => "Polski";
    public static string EnglishUi => "English";
    public static string Overlay => "Overlay";
    public static string OverlayScale => T("Skala overlay", "Overlay scale");
    public static string LockOverlay => T("Zablokuj overlay", "Lock overlay");
    public static string ShowOnlyOnBst => T("Tylko na Beastmasterze", "Beastmaster only");
    public static string ShowRotationOpt => T("Pokazuj rotacje", "Show rotation");
    public static string ShowReactionsOpt => T("Pokazuj reakcje", "Show reactions");
    public static string ShowMitigationOpt => T("Pokazuj mitygacje", "Show mitigation");
    public static string ShowCaptureOpt => T("Pokazuj HUD lapania", "Show capture HUD");
    public static string HoldTpOpt => T("Trzymaj TP na Universality", "Hold TP for Universality");
    public static string ChatNotifyOpt => T("Powiadamiaj na czacie o pakcie", "Announce pacts in chat");
    public static string Rotation => T("Rotacja", "Rotation");
    public static string Reactions => T("Reakcje", "Reactions");
    public static string Mitigation => T("Mitygacja", "Mitigation");
    public static string CommandHelp => T("Pomoc i komendy", "Help and commands");
    public static string NoPlayer => T("Brak gracza", "No player");
    public static string Commands => T(
        "/bstassist lub /bestia - otwiera Bestiariusz\n/bstassist overlay - wlacza lub ukrywa overlay\n/bstassist progress - otwiera Progres\n/bstassist config - otwiera Ustawienia\n/bstassist debug - zapisuje ID akcji do /xllog",
        "/bstassist or /bestia - opens Bestiary\n/bstassist overlay - toggles the overlay\n/bstassist progress - opens Progress\n/bstassist config - opens Settings\n/bstassist debug - writes action IDs to /xllog");
    public static string PluginDisclaimer => T("Plugin nie wciska skilli. To nakladka i tracker.", "The plugin never presses abilities. It is an overlay and tracker only.");

    public static void Sync(Configuration config)
    {
        Polish = config.PreferPolishUi;
        Data.Loc.Polish = config.PreferPolishUi;
    }
}
