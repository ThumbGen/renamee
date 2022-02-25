using renamee.Shared.Models;
using System.Text.RegularExpressions;

namespace renamee.Shared.Helpers
{
    public static class FormatParser
    {
        public const char FolderSeparator = '|';
        private static string FormatRegexPattern;// = @"YEAR[.\-|\s]*|MONTH[.\-|\s]*|DAY[.\-|\s]*|HOUR[.\-|\s]*|MIN[.\-|\s]*|SEC[.\-|\s]*|ORG[.\-|\s]*|CITY[.\-|\s]*|COUNTRY[.\-|\s]*";  
        public const string Year = "YEAR";
        public const string Month = "MONTH";
        public const string Day = "DAY";
        public const string Hours = "HOUR";
        public const string Minute = "MIN";
        public const string Seconds = "SEC";
        public const string OriginalName = "ORG";
        public const string City = "CITY";
        public const string Country = "COUNTRY";

        public static readonly string[] AcceptedTokens = new[] { Year, Month, Day, Hours, Minute, Seconds, OriginalName, City, Country };

        static FormatParser()
        {
            // build the pattern dynamically as it is repetitive
            FormatRegexPattern = string.Empty;
            foreach (var item in AcceptedTokens)
            {
                FormatRegexPattern += $@"{item}[.\-|\s]*|";
            }
            FormatRegexPattern = FormatRegexPattern.TrimEnd('|');
        }

        public static bool Validate(string format)
        {
            if (string.IsNullOrEmpty(format)) throw new ArgumentNullException(nameof(format));

            var foldersParts = format.Split(FolderSeparator);

            if (foldersParts.Any(x => Regex.Replace(format, FormatRegexPattern, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled) != string.Empty))
            {
                return false;
            }

            return true;
        }

        public static bool TryParse(DateTimeOffset dateTimeOffset, string format, string originalFilename, out string result, GeocodingData geocodingData = null)
        {
            if (string.IsNullOrEmpty(format)) throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(originalFilename)) throw new ArgumentNullException(nameof(originalFilename));
            if (dateTimeOffset == DateTimeOffset.MinValue || dateTimeOffset == DateTimeOffset.MaxValue) throw new ArgumentNullException(nameof(dateTimeOffset));
            if (!Validate(format))
            {
                result = string.Empty;
                return false;
            }

            result = string.Empty;
            var foldersParts = format.Split(FolderSeparator);
            foreach (var folderPart in foldersParts)
            {
                result += StoreDateTimeEntry(folderPart, originalFilename) + FolderSeparator;
            }

            result = result.TrimEnd(FolderSeparator);

            return true;

            string StoreDateTimeEntry(string folderPart, string originalFilename)
            {
                return folderPart
                    .Replace("YEAR", dateTimeOffset.Year.ToString())
                    .Replace("MONTH", dateTimeOffset.Month.ToString("00"))
                    .Replace("DAY", dateTimeOffset.Day.ToString("00"))
                    .Replace("HOUR", dateTimeOffset.Hour.ToString("00"))
                    .Replace("MIN", dateTimeOffset.Minute.ToString("00"))
                    .Replace("SEC", dateTimeOffset.Second.ToString("00"))
                    .Replace("ORG", Path.GetFileNameWithoutExtension(originalFilename))
                    .Replace("CITY", geocodingData?.City ?? string.Empty)
                    .Replace("COUNTRY", geocodingData?.Country ?? string.Empty)
                    .TrimEnd();
            }
        }


    }
}
