using System;

namespace SimpleBlogMVC.Extensions
{
    public static class TimeExtensions
    {
        public static string ToPrettyString(this TimeSpan span)
        {
            if (span.TotalSeconds < 60)
                return $"{span.Seconds} second{(span.Seconds == 1 ? "" : "s")} ago";
            if (span.TotalMinutes < 60)
                return $"{span.Minutes} minute{(span.Minutes == 1 ? "" : "s")} ago";
            if (span.TotalHours < 24)
                return $"{span.Hours} hour{(span.Hours == 1 ? "" : "s")} ago";
            if (span.TotalDays < 30)
                return $"{span.Days} day{(span.Days == 1 ? "" : "s")} ago";
            if (span.TotalDays < 365)
                return $"{span.Days / 30} month{(span.Days / 30 == 1 ? "" : "s")} ago";
            return $"{span.Days / 365} year{(span.Days / 365 == 1 ? "" : "s")} ago";
        }
    }
}