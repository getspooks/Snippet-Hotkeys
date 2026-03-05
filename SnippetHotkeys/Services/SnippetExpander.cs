using System;
using System.Collections.Generic;
using System.Globalization;

namespace SnippetHotkeys.Services
{
    /// ***************************************************************** ///
    /// Class:      SnippetExpander
    /// Summary:    Expands special tokens embedded in snippet text.
    /// ***************************************************************** ///
    public sealed class SnippetExpander
    {
        // Token name -> function that returns replacement text
        private readonly Dictionary<string, Func<string>> _tokens;

        // Initializes the token dictionary
        public SnippetExpander()
        {
            _tokens = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
            {
                // Date tokens
                ["NEXT_BUSINESS_DATE"] = () => GetNextBusinessDate(DateTime.Today).ToString("M/d", CultureInfo.InvariantCulture),
                ["NEXT_BUSINESS_DAY_DATE"] = () => GetNextBusinessDayDateLabel(DateTime.Today),
                ["NEXT_BUSINESS_DAY"] = () => GetNextBusinessDayLabel(DateTime.Today),
                ["TODAY"] = () => DateTime.Today.ToString("M/d", CultureInfo.InvariantCulture),
                ["NOW"] = () => DateTime.Now.ToString("M/d h:mm tt", CultureInfo.InvariantCulture),

                // Control tokens (TypeService will type these characters)
                ["TAB"] = () => "\t",
                ["ENTER"] = () => "\n",
            };
        }

        // Expands tokens within a snippet string
        public string Expand(string snippet)
        {
            if (string.IsNullOrEmpty(snippet))
                return "";

            // Replace {TOKEN} occurrences
            foreach (var kvp in _tokens)
            {
                string token = "{" + kvp.Key + "}";
                snippet = snippet.Replace(token, kvp.Value());
            }

            return snippet;
        }

        // This is the logic behind NEXT_BUSINESS_DATE (weekends only)
        private static DateTime GetNextBusinessDate(DateTime fromDate)
        {
            var d = fromDate.AddDays(1);
            while (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                d = d.AddDays(1);
            return d;
        }

        // Returns a friendly label including both the day name and date
        private static string GetNextBusinessDayDateLabel(DateTime today)
        {
            var next = GetNextBusinessDate(today);
            string label = GetNextBusinessDayLabel(today);
            return $"{label} {next:M/d}";
        }

        // Returns a friendly label for day
        private static string GetNextBusinessDayLabel(DateTime today)
        {
            var next = GetNextBusinessDate(today);

            // If it's literally tomorrow, say "Tomorrow"
            if (next.Date == today.Date.AddDays(1))
                return "tomorrow";

            // Otherwise, use the day name
            return next.DayOfWeek.ToString();
        }

        // For “Help” UI
        public IReadOnlyList<(string Token, string Description, string Example)> GetTokenHelp()
        {
            return new List<(string, string, string)>
            {
                ("{NEXT_BUSINESS_DATE}", "Next business date (skips Sat/Sun).", "Scheduled for {NEXT_BUSINESS_DATE}."),
                ("{NEXT_BUSINESS_DAY}", "Friendly label for next business day: 'Tomorrow' or day name.", "Scheduled {NEXT_BUSINESS_DAY}."),
                ("{NEXT_BUSINESS_DAY_DATE}", "Friendly label + date: 'Tomorrow 3/4' or 'Monday 3/4'.", "Scheduled for {NEXT_BUSINESS_DAY_DATE}."),
                ("{TODAY}", "Today’s date (local format).", "Updated {TODAY}."),
                ("{NOW}", "Current date/time (local format).", "Sent {NOW}."),
                ("{TAB}", "Inserts a tab character (useful for forms).", "Name:{TAB}Value"),
                ("{ENTER}", "Inserts a new line.", "Line 1{ENTER}Line 2"),
            };
        }
    }
}
