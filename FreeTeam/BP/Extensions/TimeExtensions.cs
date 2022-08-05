using System;

namespace FreeTeam.BP.Extensions
{
    public static partial class TimeUtils
    {
        public static int CompareToInUtc(this DateTime time, DateTime other) =>
            time.ToUniversalTime().CompareTo(other.ToUniversalTime());

        public static string ToTimerString(this TimeSpan timespan, TimerFormat format = TimerFormat.HMS) =>
            format switch
            {
                TimerFormat.HMS => $"{timespan.TotalHours:00}:{timespan.Minutes:00}:{timespan.Seconds:00}",
                TimerFormat.MS => $"{timespan.TotalMinutes:00}:{timespan.Seconds:00}",
                _ => string.Empty,
            };
    }
}