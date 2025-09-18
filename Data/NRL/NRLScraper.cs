#pragma warning disable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpAlgorithms.Time;
using HtmlAgilityPack;

namespace CSharpAlgorithms.Data.NRL;

public static class NRLScraper
{
    public const string STAT_LABEL_NODES_SELECTOR = "//span[@class='match-centre-summary-group__name']";

    public static (string home, string away) GetMatchTeamNames(HtmlDocument page)
    {
        HtmlNode homeTeamNode = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[2]/div/div[1]/div[1]/div/div[2]/div/div[2]/div[1]/div/p[2]");
        HtmlNode awayTeamNode = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[2]/div/div[1]/div[1]/div/div[2]/div/div[3]/div[1]/div/p[2]");

        string homeTeamName = homeTeamNode.InnerText.Trim();
        string awayTeamName = awayTeamNode.InnerText.Trim();

        return (homeTeamName, awayTeamName);
    }

    public static Score GetFullTimeMatchScores(HtmlDocument page)
    {
        HtmlNode homeScoreNode = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[2]/div/div[1]/div[1]/div/div[2]/div/div[2]/div[2]");
        HtmlNode awayScoreNode = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[2]/div/div[1]/div[1]/div/div[2]/div/div[3]/div[2]");

        ushort homeScore = Convert.ToUInt16(homeScoreNode.InnerText.Replace("Scored", "").Replace("points", "").Trim());
        ushort awayScore = Convert.ToUInt16(awayScoreNode.InnerText.Replace("Scored", "").Replace("points", "").Trim());

        return new Score(homeScore, awayScore);
    }

    public static Score GetHalfTimeScore(HtmlDocument page)
    {
        HtmlNode halfTimeLabelNode = HTMLUtils.SearchForNodes(page, new SearchForNodeParams()
        {
            NodeType = "span",
            InnerText = "Half Time"
        })[0];

        HtmlNode parentNode = halfTimeLabelNode.ParentNode;
        HtmlNode homeScoreNode = parentNode.SelectSingleNode(".//span[2]").ChildNodes.First();
        HtmlNode awayScoreNode = parentNode.SelectSingleNode(".//span[4]").ChildNodes.First();

        ushort homeScore = Convert.ToUInt16(homeScoreNode.InnerText.Trim());
        ushort awayScore = Convert.ToUInt16(awayScoreNode.InnerText.Trim());

        return new Score(homeScore, awayScore);
    }

    public static string GetPlace(HtmlDocument page)
    {
        HtmlNode placeNode = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[2]/div/div[1]/div[2]/p");
        return placeNode.InnerText.Replace("Venue:", string.Empty).Trim();
    }

    public static (int home, int away) GetTriesCount(HtmlDocument page)
    {
        HtmlNode homeTriesNode = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[3]/div[1]/h3/span[2]/span");
        HtmlNode awayTriesNode = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[3]/div[1]/h3/span[4]/span");

        int homeTries = Convert.ToInt32(homeTriesNode.InnerText.Trim());
        int awayTries = Convert.ToInt32(awayTriesNode.InnerText.Trim());

        return (homeTries, awayTries);
    }

    public static TryInfo[] GetTries(HtmlDocument page)
    {
        (string home, string away) teamNames = GetMatchTeamNames(page);
        (int homeTriesCount, int awayTriesCount) = GetTriesCount(page);
        List<TryInfo> tries = new(capacity: homeTriesCount + awayTriesCount);

        HtmlNodeCollection homeTriesNodesList = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[3]/div[1]/div/ul[1]").ChildNodes;
        HtmlNodeCollection awayTriesNodesList = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[3]/div[1]/div/ul[2]").ChildNodes;

        for (int i = 0; i < homeTriesNodesList.Count; i++)
        {
            HtmlNode tryNode = homeTriesNodesList[i];
            string innerText = tryNode.InnerText.Trim();
            int lastSpaceIndex = innerText.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                Console.WriteLine($"[CSharpAlgorithms.Data.NRL.NRLScraper.GetTries] could not get info for home try");
                continue;
            }

            string minutesInText = innerText[(lastSpaceIndex + 1)..].Replace("'", string.Empty).Trim();
            byte minutesIn = Convert.ToByte(minutesInText);
            string playerName = innerText[..lastSpaceIndex].Trim();

            TryInfo tryInfo = new(teamNames.home, playerName, minutesIn);
            tries.Add(tryInfo);
        }

        for (int i = 0; i < awayTriesNodesList.Count; i++)
        {
            HtmlNode tryNode = awayTriesNodesList[i];
            string innerText = tryNode.InnerText.Trim();
            int lastSpaceIndex = innerText.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                Console.WriteLine($"[CSharpAlgorithms.Data.NRL.NRLScraper.GetTries] could not get info for away try");
                continue;
            }

            string minutesInText = innerText[(lastSpaceIndex + 1)..].Replace("'", string.Empty).Trim();
            byte minutesIn = Convert.ToByte(minutesInText);
            string playerName = innerText[..lastSpaceIndex].Trim();

            TryInfo tryInfo = new(teamNames.away, playerName, minutesIn);
            tries.Add(tryInfo);
        }

        return [.. tries];
    }

    public static ((int chances, int completed) home, (int chances, int completed) away) GetConversionCount(HtmlDocument page)
    {
        HtmlNode homeConversionsNode = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[3]/div[2]/h3/span[2]/span");
        HtmlNode awayConversionsNode = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[3]/div[2]/h3/span[4]/span");

        int homeCompletedConversions = Convert.ToInt32(homeConversionsNode.InnerText.Trim().First().ToString());
        int awayCompletedConversions = Convert.ToInt32(awayConversionsNode.InnerText.Trim().First().ToString());

        int homeConversionChances = Convert.ToInt32(homeConversionsNode.InnerText.Trim().Last().ToString());
        int awayConversionChances = Convert.ToInt32(awayConversionsNode.InnerText.Trim().Last().ToString());

        return ((homeConversionChances, homeCompletedConversions), (awayConversionChances, awayCompletedConversions));
    }

    public static ConversionInfo[] GetConversions(HtmlDocument page)
    {
        (string home, string away) teamNames = GetMatchTeamNames(page);
        ((int _, int homeCompleted), (int _, int awayCompleted)) = GetConversionCount(page);

        List<ConversionInfo> conversions = new List<ConversionInfo>(capacity: homeCompleted + awayCompleted);

        HtmlNodeCollection homeConversionsNodesList = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[3]/div[2]/div/ul[1]").ChildNodes;
        HtmlNodeCollection awayConversionsNodesList = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[3]/div[2]/div/ul[2]").ChildNodes;

        for (int i = 0; i < homeConversionsNodesList.Count; i++)
        {
            HtmlNode conversionNode = homeConversionsNodesList[i];
            string innerText = conversionNode.InnerText.Trim();
            int lastSpaceIndex = innerText.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                Console.WriteLine($"[CSharpAlgorithms.Data.NRL.NRLScraper.GetConversions] could not get info for home conversion");
                continue;
            }

            string minutesInText = innerText[(lastSpaceIndex + 1)..].Replace("'", string.Empty).Trim();
            byte minutesIn = Convert.ToByte(minutesInText);
            string playerName = innerText[..lastSpaceIndex].Trim();

            ConversionInfo conversion = new(teamNames.home, playerName, minutesIn);
            conversions.Add(conversion);
        }

        for (int i = 0; i < awayConversionsNodesList.Count; i++)
        {
            HtmlNode conversionNode = awayConversionsNodesList[i];
            string innerText = conversionNode.InnerText.Trim();
            int lastSpaceIndex = innerText.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                Console.WriteLine($"[CSharpAlgorithms.Data.NRL.NRLScraper.GetConversions] could not get info for away conversion");
                continue;
            }

            string minutesInText = innerText[(lastSpaceIndex + 1)..].Replace("'", string.Empty).Trim();
            byte minutesIn = Convert.ToByte(minutesInText);
            string playerName = innerText[..lastSpaceIndex].Trim();

            ConversionInfo conversion = new(teamNames.away, playerName, minutesIn);
            conversions.Add(conversion);
        }

        return [.. conversions];
    }


    public static FieldGoalInfo[] GetFieldGoals(HtmlDocument page)
    {
        var _1pointGoals = FieldGoalInfo.Get1PointFieldGoalsFromTupleArray(GetMatchPlayerStatSummary(page, "1 Point Field Goals"));
        var _2pointGoals = FieldGoalInfo.Get2PointFieldGoalsFromTupleArray(GetMatchPlayerStatSummary(page, "2 Point Field Goals"));

        List<FieldGoalInfo> fieldGoalInfos = [.. _1pointGoals];
        fieldGoalInfos.AddRange(_2pointGoals);

        return [.. fieldGoalInfos];
    }

    public static PenaltyGoalInfo[] GetPenaltyGoals(HtmlDocument page)
    {
        return PenaltyGoalInfo.GetPenaltyGoalsFromTupleArray(GetMatchPlayerStatSummary(page, "Penalty Goals"));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="statType">'Tries' or 'Conversions' or 'Penalty goals' or 'field goals'</param>
    /// <returns></returns>
    public static (string team, string player, byte minutesIn)[] GetMatchPlayerStatSummary(HtmlDocument page, string statType)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Data.NRL.NRLScraper.GetMatchPlayerStatSummary]";

        statType = statType.ToLower();
        Console.WriteLine($"{CALL_PATH} getting {statType}");

        const string STAT_LABEL_NODES_SELECTOR = "//span[contains(@class, 'match-centre-summary-group__name')]";
        HtmlNodeCollection stateLabelNodes = page.DocumentNode.SelectNodes(STAT_LABEL_NODES_SELECTOR);
        if (stateLabelNodes is null)
        {
            Console.WriteLine($"{CALL_PATH} could not find stat label nodes with {STAT_LABEL_NODES_SELECTOR}");
            return [];
        }

        HtmlNode stateLabelNode = stateLabelNodes.FirstOrDefault(p => p.InnerText.Trim().Equals(statType, StringComparison.OrdinalIgnoreCase));

        if (stateLabelNode is null)
        {
            Console.WriteLine($"{CALL_PATH} could not get match player stat summary");
            return [];
        }

        List<(string team, string player, byte minutesIn)> summary = [];

        (string home, string away) teamNames = GetMatchTeamNames(page);
        HtmlNode groupNode = stateLabelNode.ParentNode.ParentNode;

        HtmlNode homeStatsParentNode = groupNode.SelectSingleNode(".//div/ul[1]");
        foreach (var statNode in homeStatsParentNode.ChildNodes)
        {
            string innerText = statNode.InnerText.Trim();
            int lastSpaceIndex = innerText.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                Console.WriteLine($"{CALL_PATH} could not get home stat");
                continue;
            }

            string minutesInText = innerText[(lastSpaceIndex + 1)..].Replace("'", string.Empty).Trim();
            byte minutesIn = Convert.ToByte(minutesInText);
            string playerName = innerText[..lastSpaceIndex].Trim();
            summary.Add((teamNames.home, playerName, minutesIn));
        }

        HtmlNode awayStatsParentNode = groupNode.SelectSingleNode(".//div/ul[2]");
        foreach (var statNode in awayStatsParentNode.ChildNodes)
        {
            string innerText = statNode.InnerText.Trim();
            int lastSpaceIndex = innerText.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                Console.WriteLine($"{CALL_PATH} could not get away stat");
                continue;
            }

            string minutesInText = innerText[(lastSpaceIndex + 1)..].Replace("'", string.Empty).Trim();
            byte minutesIn = Convert.ToByte(minutesInText);
            string playerName = innerText[..lastSpaceIndex].Trim();
            summary.Add((teamNames.away, playerName, minutesIn));
        }

        return [.. summary];
    }

    public static ushort GetRound(HtmlDocument page)
    {
        HtmlNode node = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[2]/div/div[1]/div[1]/div/div[2]/p");
        ushort round = Convert.ToUInt16(node.InnerText.Split("-")[0].Trim().Replace("Round ", string.Empty));

        return round;
    }

    public static DateTime GetDateTime(string url, HtmlDocument page)
    {
        HtmlNode dateNode = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[1]/div/div/div[2]/div/div[1]/div[1]/div/div[2]/p");

        string trimmedInnerText = dateNode.InnerText.Trim();
        string[] splittedTrimmedInnerText = trimmedInnerText.Split(" ");

        ushort year = Convert.ToUInt16(url.Split("/")[5]);
        ushort month = TimeUtils.GetMonthNumber(splittedTrimmedInnerText[^1].Trim().ToLower());
        ushort day = Convert.ToUInt16(Text.RemoveOrdinalSuffix(splittedTrimmedInnerText
        [^2].Trim()));

        DateTime matchDate = new(year, month, day);
        return matchDate;
    }

    public static async Task<(int year, int round, string homeTeam, string awayTeam, string matchDataUrl)[]> GetMatchOverviews(int yearStart, int yearEnd, int roundStart, int roundEnd)
    {
        List<(int year, int round, string homeTeam, string awayTeam, string matchDataUrl)> matchOverviews = new();

        for (int year = yearStart; year <= yearEnd; year++)
        {
            for (int round = roundStart; round <= roundEnd; round++)
            {
                string pageUrl = $"https://www.nrl.com/draw/?competition=111&round={round}&season={year}";
                HtmlDocument page = await Internet.GetPageWithHeadlessBrowser(pageUrl);

                HtmlNode matchesParentNode = page.DocumentNode.SelectSingleNode("//div[@id='draw-content']");
                HtmlNodeCollection matchNodes = matchesParentNode.ChildNodes;

                //Match node xpath example:  /html/body/div[3]/main/div[2]/div[2]/div[2]/div/div[2]/section[x]
                //match link node xpath:     /html/body/div[3]/main/div[2]/div[2]/div[2]/div/div[2]/section[x]/ul/li/div/div[1]/a
                //Home Team Name node xpath: /html/body/div[3]/main/div[2]/div[2]/div[2]/div/div[2]/section[x]/ul/li/div/div[1]/a/div/div/div/div[2]/div[1]/div/p[2]
                //Away Team Name node xpath: /html/body/div[3]/main/div[2]/div[2]/div[2]/div/div[2]/section[x]/ul/li/div/div[1]/a/div/div/div/div[3]/div[1]/div/p[2]

                foreach (HtmlNode matchNode in matchNodes)
                {
                    if (matchNode.XPath.Contains("#comment") || matchNode.XPath.Contains("#text"))
                        continue; // Skip usless nodes

                    string matchNodeXPath = matchNode.XPath;

                    HtmlNode homeTeamNode = page.DocumentNode.SelectSingleNode($"{matchNodeXPath}/ul/li/div/div[1]/a/div/div/div/div[2]/div[1]/div/p[2]");
                    if (homeTeamNode is null)
                        continue; // Skip if home team node is not found, as this probally means this is a bye

                    string homeTeamName = homeTeamNode.InnerText.Trim();


                    HtmlNode awayTeamNode = page.DocumentNode.SelectSingleNode($"{matchNodeXPath}/ul/li/div/div[1]/a/div/div/div/div[3]/div[1]/div/p[2]");
                    string awayTeamName = awayTeamNode.InnerText.Trim();

                    HtmlNode linkNode = page.DocumentNode.SelectSingleNode($"{matchNodeXPath}/ul/li/div/div[1]/a");
                    string matchDataHref = linkNode.GetAttributeValue("href", string.Empty);
                    string matchDataUrl = $"https://www.nrl.com{matchDataHref}";
                }
            }
        }

        return [.. matchOverviews];
    }

public static async Task<Game[]> GetCompletedGames(GetMatchOverviewsParams searchParams, bool savePerDownload = true, bool loadFromCache = true)
{
    var gamesBag = new ConcurrentBag<Game>();

    // Create all year/round combinations
    var yearRoundPairs =
        from year in Enumerable.Range(searchParams.YearStart, searchParams.YearEnd - searchParams.YearStart + 1)
        from round in Enumerable.Range(searchParams.RoundStart, searchParams.RoundEnd - searchParams.RoundStart + 1)
        select (year, round);

    // Optional: limit concurrency so you don't overload the server
    var throttler = new SemaphoreSlim(5); // Adjust max concurrency here

    var tasks = yearRoundPairs.Select(async pair =>
    {
        await throttler.WaitAsync();
        try
        {
            int year = pair.year;
            int round = pair.round;

            string pageUrl = $"https://www.nrl.com/draw/?competition=111&round={round}&season={year}";
            HtmlDocument page = await Internet.GetPageWithHeadlessBrowser(pageUrl);

            HtmlNode matchesParentNode = page.DocumentNode.SelectSingleNode("//div[@id='draw-content']");
            if (matchesParentNode == null)
                return;

            HtmlNodeCollection matchNodes = matchesParentNode.ChildNodes;

            foreach (HtmlNode matchNode in matchNodes)
            {
                if (matchNode.XPath.Contains("#comment") || matchNode.XPath.Contains("#text"))
                    continue;

                string matchNodeXPath = matchNode.XPath;

                HtmlNode fulltimeNode = page.DocumentNode.SelectSingleNode($"{matchNodeXPath}/ul/li/div/div[1]/a/div/div/div/div[1]/span/span");
                if (fulltimeNode is null || fulltimeNode.InnerText.Trim() != "Full Time")
                    continue;

                HtmlNode homeTeamNode = page.DocumentNode.SelectSingleNode($"{matchNodeXPath}/ul/li/div/div[1]/a/div/div/div/div[2]/div[1]/div/p[2]");
                if (homeTeamNode is null)
                    continue;

                string homeTeamName = homeTeamNode.InnerText.Trim();
                HtmlNode awayTeamNode = page.DocumentNode.SelectSingleNode($"{matchNodeXPath}/ul/li/div/div[1]/a/div/div/div/div[3]/div[1]/div/p[2]");
                string awayTeamName = awayTeamNode.InnerText.Trim();

                if (!string.IsNullOrEmpty(searchParams.Team) &&
                    !homeTeamName.Contains(searchParams.Team, StringComparison.OrdinalIgnoreCase) &&
                    !awayTeamName.Contains(searchParams.Team, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Game game;
                if (loadFromCache && NRLUtils.TryLoadFromCache(year, round, homeTeamName, awayTeamName, out Game cachedGame))
                {
                    Console.WriteLine("Loading game from cache");
                    game = cachedGame;
                }
                else
                {
                    HtmlNode linkNode = page.DocumentNode.SelectSingleNode($"{matchNodeXPath}/ul/li/div/div[1]/a");
                    string matchDataHref = linkNode.GetAttributeValue("href", string.Empty);
                    string matchDataUrl = $"https://www.nrl.com{matchDataHref}";

                    game = await Game.DownloadGame(matchDataUrl);
                    if (savePerDownload)
                        Game.SaveGame(game);
                }

                gamesBag.Add(game);
            }
        }
        finally
        {
            throttler.Release();
        }
    });

    await Task.WhenAll(tasks);

    return gamesBag.ToArray();
}

    public static Game[] GetCachedCompletedGames()
    {
        List<Game> games = [];

        DirectoryInfo cacheFolder = new DirectoryInfo("CSharpAlgorithms/Data/NRL/Games");
        foreach (var yearFolder in cacheFolder.GetDirectories())
        {
            foreach (var roundFolder in yearFolder.GetDirectories())
            {
                FileInfo[] files = roundFolder.GetFiles();
                foreach (var gameFile in files)
                {
                    games.Add(Game.LoadGame(gameFile.FullName));
                }
            }
        }
        
        return [.. games];
    }

    public static string GetGroundCondition(HtmlDocument page)
    {
        HtmlNode? conditionNode = page.DocumentNode
            .SelectNodes("//p[@class='match-weather__text']")
            ?.FirstOrDefault(p => p.InnerText.Trim().StartsWith("Ground Conditions:", System.StringComparison.OrdinalIgnoreCase));

        if (conditionNode is null)
        {
            Console.WriteLine("[CSharpAlgorithms.Data.NRL.NRLScraper.GetGroundCondition] could not get ground condition");
            return "";
        }

        HtmlNode childNode = conditionNode.ChildNodes.Last();

        return childNode.InnerText;
    }

    public static string GetWeather(HtmlDocument page)
    {
        HtmlNode? conditionNode = page.DocumentNode
            .SelectNodes("//p[@class='match-weather__text']")
            ?.FirstOrDefault(p => p.InnerText.Trim().StartsWith("Weather:", System.StringComparison.OrdinalIgnoreCase));

        if (conditionNode is null)
        {
            Console.WriteLine("[CSharpAlgorithms.Data.NRL.NRLScraper.GetGroundCondition] could not get weather");
            return "";
        }

        HtmlNode childNode = conditionNode.ChildNodes.Last();

        return childNode.InnerText;
    }

    public static MatchOfficial[] GetMatchOfficials(HtmlDocument page)
    {
        HtmlNode officialsNode = page.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div[2]/div/div[2]/div[5]/section/div/div[2]/div[2]");
        List<MatchOfficial> officials = [];

        foreach (HtmlNode node in officialsNode.ChildNodes)
        {
            if (node.XPath.Contains("#text"))
                continue;

            string name = node.SelectSingleNode(".//div/h3").InnerText;
            string position = node.SelectSingleNode(".//div/p").InnerText;

            MatchOfficial official = new MatchOfficial
            {
                Name = name,
                Position = position,
            };
            
            officials.Add(official);
        }

        return [.. officials];
    }
}

public struct GetMatchOverviewsParams
{
    public GetMatchOverviewsParams() { }

    public int YearStart { get; set; } = -1;
    public int YearEnd { get; set; } = -1;
    public int RoundStart { get; set; } = -1;
    public int RoundEnd { get; set; } = -1;
    public string Team { get; set; } = string.Empty;
}