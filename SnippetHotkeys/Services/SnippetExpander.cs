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

                // Friendly style: uses "tomorrow" when applicable
                ["NEXT_BUSINESS_DAY"] = () => GetNextBusinessDayLabel(DateTime.Today),
                ["NEXT_BUSINESS_DAY_DATE"] = () => GetNextBusinessDayDateLabel(DateTime.Today),

                // Weekday / proper style: always uses the actual weekday name
                ["NEXT_BUSINESS_DAY_PROPER"] = () => GetNextBusinessDayProperLabel(DateTime.Today),
                ["NEXT_BUSINESS_DAY_DATE_PROPER"] = () => GetNextBusinessDayProperDateLabel(DateTime.Today),

                // User-friendly aliases for the "proper" tokens
                ["NEXT_BUSINESS_WEEKDAY"] = () => GetNextBusinessDayProperLabel(DateTime.Today),
                ["NEXT_BUSINESS_WEEKDAY_DATE"] = () => GetNextBusinessDayProperDateLabel(DateTime.Today),

                ["TODAY"] = () => DateTime.Today.ToString("M/d", CultureInfo.InvariantCulture),
                ["NOW"] = () => DateTime.Now.ToString("M/d h:mm tt", CultureInfo.InvariantCulture),

                // Control tokens (TypeService handles these)
                ["TAB"] = () => "\t",
                ["ENTER"] = () => "\n",
                ["LINEBREAK"] = () => "\uE000",     // marker for Shift+Enter
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

        // Calculates the next business date (weekends only for now)
        private static DateTime GetNextBusinessDate(DateTime fromDate)
        {
            var d = fromDate.AddDays(1);

            while (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                d = d.AddDays(1);

            return d;
        }

        // Friendly label: "tomorrow" if literally tomorrow, otherwise weekday name
        private static string GetNextBusinessDayLabel(DateTime today)
        {
            var next = GetNextBusinessDate(today);

            if (next.Date == today.Date.AddDays(1))
                return "tomorrow";

            return next.DayOfWeek.ToString();
        }

        // Friendly label + date: "tomorrow 3/4" or "Monday 3/4"
        private static string GetNextBusinessDayDateLabel(DateTime today)
        {
            var next = GetNextBusinessDate(today);
            string label = GetNextBusinessDayLabel(today);
            return $"{label} {next:M/d}";
        }

        // Proper / weekday label: always actual weekday name (never "tomorrow")
        private static string GetNextBusinessDayProperLabel(DateTime today)
        {
            var next = GetNextBusinessDate(today);
            return next.DayOfWeek.ToString();
        }

        // Proper / weekday label + date: always "Monday 3/4" style
        private static string GetNextBusinessDayProperDateLabel(DateTime today)
        {
            var next = GetNextBusinessDate(today);
            return $"{next.DayOfWeek} {next:M/d}";
        }

        // For Help UI
        public IReadOnlyList<(string Token, string Description, string Example)> GetTokenHelp()
        {
            return new List<(string, string, string)>
            {
                // Date / time tokens
                ("{NEXT_BUSINESS_DATE}", "Next business date only.", "Scheduled for {NEXT_BUSINESS_DATE}."),
                ("{NEXT_BUSINESS_DAY}", "Next business day label. Uses 'tomorrow' when applicable.", "Scheduled {NEXT_BUSINESS_DAY}."),
                ("{NEXT_BUSINESS_DAY_DATE}", "Next business day label plus date. Uses 'tomorrow' when applicable.", "Scheduled for {NEXT_BUSINESS_DAY_DATE}."),
                ("{NEXT_BUSINESS_WEEKDAY}", "Next business day as the weekday name.", "Scheduled for {NEXT_BUSINESS_WEEKDAY}."),
                ("{NEXT_BUSINESS_WEEKDAY_DATE}", "Next business day as weekday plus date.", "Scheduled for {NEXT_BUSINESS_WEEKDAY_DATE}."),
                ("{TODAY}", "Today's date.", "Updated {TODAY}."),
                ("{NOW}", "Current date and time.", "Sent {NOW}."),

                // Legacy aliases kept for backwards compatibility
                ("{NEXT_BUSINESS_DAY_PROPER}", "Same as {NEXT_BUSINESS_WEEKDAY}.", "Scheduled for {NEXT_BUSINESS_DAY_PROPER}."),
                ("{NEXT_BUSINESS_DAY_DATE_PROPER}", "Same as {NEXT_BUSINESS_WEEKDAY_DATE}.", "Scheduled for {NEXT_BUSINESS_DAY_DATE_PROPER}."),

                // Formatting tokens
                ("{TAB}", "Insert a tab.", "Name:{TAB}Value"),
                ("{ENTER}", "Insert a normal Enter/new paragraph. You can also press Enter directly in the snippet editor.", "Line 1{ENTER}Line 2"),
                ("{LINEBREAK}", "Insert a soft line break (Shift+Enter). Best for Gmail formatting.", "Line 1{LINEBREAK}Line 2"),
            };
        }
    }
}