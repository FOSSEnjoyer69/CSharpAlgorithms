namespace CSharpAlgorithms.Data.NRL;

public struct TryInfo(string team, string player, byte minutesIn)
{
    public string Team { get; private set; } = team;
    public string Player { get; private set; } = player;
    public byte MinutesIn { get; private set; } = minutesIn;
}