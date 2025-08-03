using HtmlAgilityPack;
namespace CSharpAlgorithms.Data.NRL;

public class Game
{
    public string HomeTeam { get; set; } = "";
    public string AwayTeam { get; set; } = "";
    public Score HalfTmeScore { get; set; }
    public Score FullTimeScore { get; set; }
    public DateTime MatchDate { get; set; }
    public int Round { get; set; }
    public string Winner => FullTimeScore.Home > FullTimeScore.Away ? HomeTeam : FullTimeScore.Away > FullTimeScore.Home ? AwayTeam : "Draw";

    public string Place { get; set; } = "";

    public string GroundCondition { get; set; } = "";
    public string WeatherCondition { get; set; } = "";

    public TryInfo[] Tries { get; set; } = [];
    public ConversionInfo[] Conversions { get; set; } = [];
    public PenaltyGoalInfo[] PenaltyGoals { get; set; } = [];
    public FieldGoalInfo[] FieldGoals { get; set; } = [];

    public override string ToString()
    {
        string matchSummary = $"{MatchDate.ToShortDateString()} - {HomeTeam} vs {AwayTeam}: {FullTimeScore.Home}-{FullTimeScore.Away} At {Place} \n";

        foreach (var _try in Tries)
        {
            matchSummary += $"{_try.Player} scored a try for {_try.Team} at {_try.MinutesIn} Minutes\n";
        }

        return matchSummary;
    }

    public static async Task<Game> DownloadGame(string url)
    {
        Console.WriteLine($"[CSharpAlgorithms.Data.NRL.Game.DownloadGame] Downloading game from {url}");
        HtmlDocument page = await Internet.GetPageWithHeadlessBrowser(url);

        (string homeTeamName, string awayTeamName) = NRLScraper.GetMatchTeamNames(page);

        Game game = new()
        {
            HomeTeam = homeTeamName,
            AwayTeam = awayTeamName,

            HalfTmeScore = NRLScraper.GetHalfTimeScore(page),
            FullTimeScore = NRLScraper.GetFullTimeMatchScores(page),

            Place = NRLScraper.GetPlace(page),

            Tries = NRLScraper.GetTries(page),
            Conversions = NRLScraper.GetConversions(page),
            PenaltyGoals = NRLScraper.GetPenaltyGoals(page),
            FieldGoals = NRLScraper.GetFieldGoals(page),
            Round = NRLScraper.GetRound(page),
            MatchDate = NRLScraper.GetDateTime(url, page),
            GroundCondition = NRLScraper.GetGroundCondition(page),
            WeatherCondition = NRLScraper.GetWeather(page)
        };

        return game;
    }

    public static void SaveGame(Game[] games)
    {
        foreach (Game game in games)
        {
            SaveGame(game);
        }
    }
    public static void SaveGame(Game game)
    {
        string filePath = $"CSharpAlgorithms/Data/NRL/Games/{game.MatchDate.Year}/Round {game.Round}/{game.HomeTeam} vs {game.AwayTeam}.json";
        SaveSystem.SaveAsJson(filePath, game);
    }

    internal static Game DownloadGameSync(string matchDataUrl)
    {
        throw new NotImplementedException();
    }
}