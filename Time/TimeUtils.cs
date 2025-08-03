namespace CSharpAlgorithms.Time;

public static class TimeUtils
{
    public static string[] Months =>
    [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];

    public static ushort GetMonthNumber(string monthName)
    {
        monthName = monthName.Trim().ToLower();
        for (ushort i = 0; i < Months.Length; i++)
        {
            if (Months[i].ToLower() == monthName)
            {
                return (ushort)(i + 1);
            }
        }
        throw new ArgumentException($"[CSharpAlgorithms.Time.Timeutils.GetMonthNumber] Invalid month name: {monthName}");
    }
}