using System;

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
            if (Months[i].Equals(monthName, StringComparison.CurrentCultureIgnoreCase))
                return (ushort)(i + 1);

        throw new ArgumentException($"[CSharpAlgorithms.Time.Timeutils.GetMonthNumber] Invalid month name: {monthName}");
    }
    
    public static double SecondsToMilliSeconds(double seconds) => (seconds * 1_000);
}