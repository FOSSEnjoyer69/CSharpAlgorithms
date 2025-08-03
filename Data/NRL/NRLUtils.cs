namespace CSharpAlgorithms.Data.NRL;

public static class NRLUtils
{
    public const string BULLDOGS_TEAM_NAME = "Canterbury Bulldogs";
    public const string SHARKS_TEAM_NAME = "Cronulla Sharks";
    public const string WARRIORS_TEAM_NAME = "Warriors";
    public const string SEA_EAGLES_TEAM_NAME = "Manly Sea Eagles";
    public const string BRONCOS_TEAM_NAME = "Brisbane Broncos";
    public const string COWBOYS_TEAM_NAME = "North Queensland Cowboys";
    public const string DOLPHINS_TEAM_NAME = "Dolphins";
    public const string RABBITOHS_TEAM_NAME = "South Sydney Rabbitohs";
    public const string KNIGHTS_TEAM_NAME = "Newcastle Knights";
    public const string STORM_TEAM_NAME = "Melbourne Storm";
    public const string TIGERS_TEAM_NAME = "Wests Tigers";
    public const string TITANS_TEAM_NAME = "Gold Coast Titans";
    public const string PANTHERS_TEAM_NAME = "Penrith Panthers";
    public const string EELS_TEAM_NAME = "Parramatta Eels";
    public const string RAIDERS_TEAM_NAME = "Canberra Raiders";
    public const string ROOSTERS_TEAM_NAME = "Sydney Roosters";

    public static string[] TeamNames =
    [
        BRONCOS_TEAM_NAME,
        RAIDERS_TEAM_NAME,
        BULLDOGS_TEAM_NAME,
        SHARKS_TEAM_NAME,
        DOLPHINS_TEAM_NAME,
        TITANS_TEAM_NAME,
        SEA_EAGLES_TEAM_NAME,
        STORM_TEAM_NAME,
        KNIGHTS_TEAM_NAME,
        COWBOYS_TEAM_NAME,
        EELS_TEAM_NAME,
        PANTHERS_TEAM_NAME,
        RABBITOHS_TEAM_NAME,
        ROOSTERS_TEAM_NAME,
        WARRIORS_TEAM_NAME,
        TIGERS_TEAM_NAME,
    ];

    public static bool TryLoadFromCache(int year, int round, string homeTeam, string awayTeam, out Game game)
    {
        string path = $"CSharpAlgorithms/Data/NRL/Games/{year}/Round {round}/{homeTeam} vs {awayTeam}.json";
        if (File.Exists(path))
        {
            game = SaveSystem.LoadJson<Game>(path);
            return true;
        }
        else
        {
            game = null;
            return false;
        }
    }
}