namespace CSharpAlgorithms.Data.NRL;

public struct PenaltyGoalInfo(string team, string player, byte minutesIn)
{
    public string Team { get; private set; } = team;
    public string Player { get; private set; } = player;
    public byte MinutesIn { get; private set; } = minutesIn;

    public static PenaltyGoalInfo[] GetPenaltyGoalsFromTupleArray((string team, string player, byte minutesIn)[] stats)
    {
        PenaltyGoalInfo[] fieldGoals = new PenaltyGoalInfo[stats.Length];

        for (int i = 0; i < stats.Length; i++)
        {
            fieldGoals[i] = new PenaltyGoalInfo
            (
                team: stats[i].team,
                player: stats[i].player,
                minutesIn: stats[i].minutesIn
            );
        }

        return fieldGoals;

    }
}