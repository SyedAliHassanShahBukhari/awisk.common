namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static DateTime UtcNow() => DateTime.UtcNow;

        public static long ToUnixTimeSeconds(this DateTime dt)
            => new DateTimeOffset(dt.ToUniversalTime()).ToUnixTimeSeconds();

        public static long ToUnixTimeMilliseconds(this DateTime dt)
            => new DateTimeOffset(dt.ToUniversalTime()).ToUnixTimeMilliseconds();

        public static DateTime FromUnixTimeSeconds(long s)
            => DateTimeOffset.FromUnixTimeSeconds(s).UtcDateTime;

        public static DateTime FromUnixTimeMilliseconds(long ms)
            => DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;

        public static DateTime RoundToSeconds(this DateTime dtUtc)
        {
            var ticks = dtUtc.Ticks - (dtUtc.Ticks % TimeSpan.TicksPerSecond);
            return new DateTime(ticks, DateTimeKind.Utc);
        }

        public static DateTime ToTimeZone(this DateTime dt, string timeZoneId)
            => TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));

        public static bool IsWeekend(this DateTime dt)
            => dt.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        public static System.Collections.Generic.IEnumerable<DateTime> EachDay(this DateTime start, DateTime end)
        {
            for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
            {
                yield return d;
            }
        }
    }
}
