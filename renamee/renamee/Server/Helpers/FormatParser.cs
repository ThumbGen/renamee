using System.Text.RegularExpressions;

namespace renamee.Server.Helpers
{
    public class FormatParser
    {
        public const char FolderSeparator = '|';
        private const string FormatRegexPattern = @"YEAR[.\-|]*|MONTH[.\-|]*|DAY[.\-|]*|HOUR[.\-|]*|MIN[.\-|]*|SEC[.\-|]*|ORG[.\-|]*";
        public const string Year = "YEAR";
        public const string Month = "MONTH";
        public const string Day = "DAY";
        public const string Hours = "HOUR";
        public const string Minute = "MIN";
        public const string Seconds = "SEC";
        public const string OriginalName = "ORG";

        public static readonly string[] AcceptedTokens = new[] { Year, Month, Day, Hours, Minute, Seconds, OriginalName };

        public bool Validate(string format)
        {
            if (string.IsNullOrEmpty(format)) throw new ArgumentNullException(nameof(format));

            var foldersParts = format.Split(FolderSeparator);

            if (foldersParts.Any(x => Regex.Replace(format, FormatRegexPattern, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled) != string.Empty))
            {
                return false;
            }

            return true;
        }

        public bool TryParse(DateTimeOffset dateTimeOffset, string format, string originalFilename, out string result)
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
                    .Replace("ORG", Path.GetFileNameWithoutExtension(originalFilename));
            }
        }


    }
}
