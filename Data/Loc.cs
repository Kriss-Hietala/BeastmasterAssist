namespace BeastmasterAssist.Data;

public static class Loc
{
    public static bool Polish = true;
    public static string T(string pl, string en) => Polish ? pl : en;
}
