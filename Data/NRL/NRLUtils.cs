#pragma warning disable

using CSharpAlgorithms.Images;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Color = ScottPlot.Color;
using Image = ScottPlot.Image;

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

    public static Dictionary<string, string> TeamLogoFilePathDic = new()
    {
        { "Broncos", "CSharpAlgorithms/Data/NRL/Images/Teams/broncos.svg" },
        { "Bulldogs", "CSharpAlgorithms/Data/NRL/Images/Teams/bulldogs.svg" },
        { "Cowboys", "CSharpAlgorithms/Data/NRL/Images/Teams/cowboys.svg" },
        { "Dolphins", "CSharpAlgorithms/Data/NRL/Images/Teams/dolphins.svg" },
        { "Dragons", "CSharpAlgorithms/Data/NRL/Images/Teams/dragons.svg" },
        { "Eels", "CSharpAlgorithms/Data/NRL/Images/Teams/eels.svg" },
        { "Knights", "CSharpAlgorithms/Data/NRL/Images/Teams/knights.svg" },
        { "Sharks", "CSharpAlgorithms/Data/NRL/Images/Teams/sharks.svg" },
        { "Titans", "CSharpAlgorithms/Data/NRL/Images/Teams/titans.svg" },
        { "Sea Eagles", "CSharpAlgorithms/Data/NRL/Images/Teams/sea eagles.svg" },
        { "Storm", "CSharpAlgorithms/Data/NRL/Images/Teams/storm.svg" },
        { "Panthers", "CSharpAlgorithms/Data/NRL/Images/Teams/panthers.svg" },
        { "Rabbitohs", "CSharpAlgorithms/Data/NRL/Images/Teams/rabbitohs.svg" },
        { "Roosters", "CSharpAlgorithms/Data/NRL/Images/Teams/roosters.svg" },
        { "Wests Tigers", "CSharpAlgorithms/Data/NRL/Images/Teams/tigers.svg" },
        { "Raiders", "CSharpAlgorithms/Data/NRL/Images/Teams/raiders.svg" },
        { "Warriors", "CSharpAlgorithms/Data/NRL/Images/Teams/warriors.svg" },

    };

    public static Dictionary<string, Color> nrlTeamColors = new Dictionary<string, Color>()
    {
        { "Broncos", new Color(153, 0, 51) },
        { "Bulldogs", new Color(0, 83, 159) },
        { "Sharks", new Color(0, 98, 114) },
        { "Dolphins", new Color(255, 111, 0) },
        { "Titans", new Color(0, 43, 92) },
        { "Sea Eagles", new Color(153, 0, 0) },
        { "Storm", new Color(108, 37, 141) },
        { "Knights", new Color(237, 28, 36) },
        { "Warriors", new Color(0, 175, 0) },
        { "Cowboys", new Color(0, 125, 195) },
        { "Eels", new Color(0, 51, 153) },
        { "Panthers", new Color(0, 0, 0) },
        { "Rabbitohs", new Color(0, 102, 51) },
        { "Dragons", new Color(255, 0, 0) },
        { "Roosters", new Color(204, 0, 0) },
        { "Wests Tigers", new Color(247, 148, 29) },
        { "Raiders", new Color(0, 153, 51) }
    };

    public static void GraphRefBias(RefBias[] refBiases)
    {
        var plot = new Plot();
        plot.ScaleFactor = 3;
        plot.YLabel("<--- Disfavours | Favours --->");
        plot.XLabel("Referee");

        Tick[] ticks = new Tick[refBiases.Length];

        for (int i = 0; i < refBiases.Length; i++)
        {
            RefBias refBias = refBiases[i];

            // ----- Favoured Bar -----
            Bar favouredBar = new Bar
            {
                Position = i + 1,
                Value = refBias.FavourScore,
                Label = refBias.FavouredTeam,
                FillColor = nrlTeamColors[refBias.FavouredTeam]
            };

            Image favouredLogo = null;
            CoordinateRect favouredLogoRect = new CoordinateRect();

            Image disfavouredLogo = null;
            CoordinateRect disfavouredLogoRect = new CoordinateRect();
            

            // favoured logo
            if (TeamLogoFilePathDic.TryGetValue(refBias.FavouredTeam, out string favouredLogoPath) && File.Exists(favouredLogoPath))
            {
                favouredLogo = ImageUtils.ToScottPlotImage(ImageUtils.SvgToImage(favouredLogoPath, 256, 256));

                double barX = favouredBar.Position;
                double barY = favouredBar.Value / 2.0;
                double logoWidth = 0.5;
                double logoHeight = 0.5;

                favouredLogoRect = new CoordinateRect
                {
                    Left = barX - logoWidth / 2,
                    Right = barX + logoWidth / 2,
                    Bottom = barY - logoHeight / 2,
                    Top = barY + logoHeight / 2
                };
            }

            // ----- Disfavoured Bar -----
            Bar disfavouredBar = new Bar
            {
                Position = i + 1,
                Value = refBias.DisfavourScore,
                Label = refBias.DisfavouredTeam,
                FillColor = nrlTeamColors[refBias.DisfavouredTeam]
            };

            // disfavoured logo
            if (TeamLogoFilePathDic.TryGetValue(refBias.DisfavouredTeam, out string disfavouredLogoPath) && File.Exists(disfavouredLogoPath))
            {
                //Image disfavouredLogo = new Image(disfavouredLogoPath);
                disfavouredLogo = ImageUtils.ToScottPlotImage(ImageUtils.SvgToImage(disfavouredLogoPath, 256, 256));

                double barX = disfavouredBar.Position;
                double barY = disfavouredBar.Value / 2.0;
                double logoWidth = 0.5;
                double logoHeight = 0.5;

                disfavouredLogoRect = new CoordinateRect
                {
                    Left = barX - logoWidth / 2,
                    Right = barX + logoWidth / 2,
                    Bottom = barY - logoHeight / 2,
                    Top = barY + logoHeight / 2
                };
            }

            // ----- Tick for Ref -----
            ticks[i] = new Tick(i + 1, refBias.RefName);

            // add both bars
            plot.Add.Bars(new[] { favouredBar, disfavouredBar });
            //plot.Add.ImageRect(favouredLogo, favouredLogoRect);
            //plot.Add.ImageRect(disfavouredLogo, disfavouredLogoRect);
        }

        plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);

        // save the graph
        plot.SavePng("ref bias graph.png", 6000, 1500);
    }


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

    public static RefBias[] GetRefBiases(Game[] games, double minDiffThreshold = 0.1)
    {
        var biasList = new List<RefBias>();

        foreach (KeyValuePair<string, Game[]> refGames in GetGamesWithRefAsDict(games))
        {
            string favouredTeam = "";
            string disfavouredTeam = "";
            double highestDiff = double.MinValue;
            double lowestDiff = double.MaxValue;
            string referee = refGames.Key;

            foreach (var team in games.SelectMany(g => new[] { g.HomeTeam, g.AwayTeam }).Distinct())
            {
                var gamesWithRef = games.Where(g =>
                    g.MatchOfficials.Any(mo => mo.Name == referee) &&
                    (g.HomeTeam == team || g.AwayTeam == team)).ToList();

                var gamesWithoutRef = games.Where(g =>
                    !g.MatchOfficials.Any(mo => mo.Name == referee) &&
                    (g.HomeTeam == team || g.AwayTeam == team)).ToList();

                double winRateWith = gamesWithRef.Count(g => g.Winner == team) / (double)gamesWithRef.Count;
                double winRateWithout = gamesWithoutRef.Count(g => g.Winner == team) / (double)gamesWithoutRef.Count;

                double diff = winRateWith - winRateWithout;

                if (diff > highestDiff)
                {
                    highestDiff = diff;
                    favouredTeam = team;
                }

                if (diff < lowestDiff)
                {
                    lowestDiff = diff;
                    disfavouredTeam = team;
                }
            }

            if (!string.IsNullOrEmpty(favouredTeam) && !string.IsNullOrEmpty(disfavouredTeam))
            {
                double favourScore = highestDiff;    // how much ref helps the favoured team
                double disfavourScore = lowestDiff;  // how much ref hurts the disfavoured team

                if (Math.Abs(favourScore) >= minDiffThreshold || Math.Abs(disfavourScore) >= minDiffThreshold)
                {
                    biasList.Add(new RefBias
                    {
                        RefName = referee,
                        FavouredTeam = favouredTeam,
                        DisfavouredTeam = disfavouredTeam,
                        FavourScore = favourScore,
                        DisfavourScore = disfavourScore,
                    });
                }
            }
        }

        return biasList.ToArray();

    }

    public static Dictionary<string, Game[]> GetGamesWithRefAsDict(Game[] games)
    {
        Dictionary<string, Game[]> gamesDict = [];

        for (int i = 0; i < games.Length; i++)
        {
            string referee = "No Ref";
            foreach (MatchOfficial official in games[i].MatchOfficials)
            {
                if (official.Position == "Referee")
                {
                    referee = official.Name;
                }
            }

            Game[] refGames = [];
            if (gamesDict.ContainsKey(referee))
            {
                refGames = gamesDict[referee];
            }

            refGames = [.. refGames, games[i]];
            gamesDict[referee] = refGames;
        }

        return gamesDict;
    }
}

public struct RefBias
{
    public string RefName { get; set; }
    public string FavouredTeam { get; set; }
    public string DisfavouredTeam { get; set; }
    public double FavourScore { get; set; }
    public double DisfavourScore { get; set; }
    public override string ToString() =>
        $"{RefName}: Favours {FavouredTeam} ({FavourScore}), against {DisfavouredTeam} ({DisfavourScore})";
}