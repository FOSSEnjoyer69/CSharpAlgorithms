namespace CSharpAlgorithms.Data.NRL;

public struct Score(ushort home, ushort away)
{
    public ushort Home { get; set; } = home;
    public ushort Away { get; set; } = away;
}