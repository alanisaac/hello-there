using System;

namespace HelloThere.Core.Utilities
{
    public static class UnixEpochExtensions
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTime(this long unixTime)
        {
            return _epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(this DateTime date)
        {
            return Convert.ToInt64((date.ToUniversalTime() - _epoch).TotalSeconds);
        }
    }
}
