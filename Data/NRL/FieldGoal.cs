using System.Linq;

namespace CSharpAlgorithms.Data.NRL;

public struct FieldGoalInfo(string team, string player, byte minutesIn, byte points)
{
    public string Team { get; private set; } = team;
    public string Player { get; private set; } = player;
    public byte MinutesIn { get; private set; } = minutesIn;
    public byte Points { get; private set; } = points;

    public static FieldGoalInfo[] Get1PointFieldGoalsFromTupleArray((string team, string player, byte minutesIn)[] stats)
    {
        return GetFieldGoalsFromTupleArray(
            [.. stats.Select(s => (s.team, s.player, s.minutesIn, (byte)1))]
        );
    }

    public static FieldGoalInfo[] Get2PointFieldGoalsFromTupleArray((string team, string player, byte minutesIn)[] stats)
    {
        return GetFieldGoalsFromTupleArray(
            [.. stats.Select(s => (s.team, s.player, s.minutesIn, (byte)2))]
        );
    }

    public static FieldGoalInfo[] GetFieldGoalsFromTupleArray((string team, string player, byte minutesIn, byte points)[] stats)
    {
        FieldGoalInfo[] fieldGoals = new FieldGoalInfo[stats.Length];

        for (int i = 0; i < stats.Length; i++)
        {
            fieldGoals[i] = new FieldGoalInfo
            (
                team: stats[i].team,
                player: stats[i].player,
                minutesIn: stats[i].minutesIn,
                points: stats[i].points
            );
        }

        return fieldGoals;
    }
}