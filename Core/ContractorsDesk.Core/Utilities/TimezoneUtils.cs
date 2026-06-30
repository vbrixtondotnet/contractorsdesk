using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Utilities
{
    public static class TimezoneUtils
    {
        private static readonly TimeZoneInfo _californiaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");

        public static TimeZoneInfo GetDefaultCaliforniaTimezoneInfo()
        {
            return _californiaTimeZone;
        }

        public static DateTime GetDefaultCaliforniaTimezoneUtc()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _californiaTimeZone);
        }

        public static string CalculateTimeSinceEmailSent(DateTime californiaLocalTime)
        {
            // Get current time in California time zone
            var californiaZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            var nowInCalifornia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, californiaZone);

            // Difference between now and the stored local time
            var diff = nowInCalifornia - californiaLocalTime;

            if (diff.TotalSeconds < 60)
                return "Just now";
            if (diff.TotalMinutes < 60)
                return $"{(int)diff.TotalMinutes} min{(diff.TotalMinutes >= 2 ? "s" : "")} ago";
            if (diff.TotalHours < 24)
                return $"{(int)diff.TotalHours} hour{(diff.TotalHours >= 2 ? "s" : "")} ago";

            return $"{(int)diff.TotalDays} day{(diff.TotalDays >= 2 ? "s" : "")} ago";
        }
    }
}
