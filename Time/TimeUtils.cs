using System;

namespace CSharpAlgorithms.Time;

public static class TimeUtils
{
    public static string[] Months => ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

    public const ushort MILLISECONDS_IN_A_SECOND = 1_000;

    public static ushort GetMonthNumber(string monthName)
    {
        monthName = monthName.Trim().ToLower();

        for (ushort i = 0; i < Months.Length; i++)
            if (Months[i].Equals(monthName, StringComparison.CurrentCultureIgnoreCase))
                return (ushort)(i + 1);

        throw new ArgumentException($"[CSharpAlgorithms.Time.Timeutils.GetMonthNumber] Invalid month name: {monthName}");
    }

    public static double MillieSecondsToSeconds(double milliSeconds) => milliSeconds / MILLISECONDS_IN_A_SECOND;
    public static int MillieSecondsToSeconds(int milliSeconds) => milliSeconds / MILLISECONDS_IN_A_SECOND;
    public static long MillieSecondsToSeconds(long milliSeconds) => milliSeconds / MILLISECONDS_IN_A_SECOND;

    public static double SecondsToMilliSeconds(double seconds) => seconds * MILLISECONDS_IN_A_SECOND;
    public static int SecondsToMilliSeconds(int seconds) => seconds * MILLISECONDS_IN_A_SECOND;
    public static long SecondsToMilliSeconds(long seconds) => seconds * MILLISECONDS_IN_A_SECOND;
}