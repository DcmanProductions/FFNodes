// LFInteractive LLC. - All Rights Reserved
using System.Text;

namespace FFNodes.Core.Utilities;

public static class Formatters
{
    public static string GetFormattedTime(TimeSpan span)
    {
        StringBuilder builder = new();

        if (span.TotalDays >= 1)
            builder.Append($" {span.Days} days ");
        if (span.TotalHours >= 1)
            builder.Append($" {span.Hours} hours ");
        if (span.TotalMinutes >= 1)
            builder.Append($" {span.Minutes} minutes ");
        if (span.TotalSeconds >= 1)
            builder.Append($" {span.Seconds} seconds ");
        if (span.TotalMilliseconds >= 1)
            builder.Append($" {span.Milliseconds} ms ");

        return builder.ToString().Replace("  ", " ").Trim();
    }
}