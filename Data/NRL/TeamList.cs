namespace CSharpAlgorithms.Data.NRL;

public struct TeamList(string[] backs, string[] fowards, string[] interchanges, string[] reserves, string coach)
{
    public string[] Back { get; set; } = backs;
    public string[] Forwards { get; set; } = fowards;
    public string[] Interchanges { get; set; } = interchanges;
    public string[] Reservess { get; set; } = reserves;
    public string Coach { get; set; } = coach;   
}