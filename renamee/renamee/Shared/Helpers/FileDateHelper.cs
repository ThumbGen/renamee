using MetadataExtractor;
using MetadataExtractor.Formats.Avi;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;
using System.Globalization;

namespace renamee.Shared.Helpers
{
    public static class FileDateHelper
    {
        public static DateTimeOffset? GetFileDate(string filePath)
        {
            try
            {
                DateTimeOffset? date = GetDateFromTags(filePath);

                if (!date.HasValue || date.Value.Equals(default))
                {
                    date = new FileInfo(filePath).LastWriteTime;
                }

                return date;
            }
            catch
            {
                return default;
            }
        }

        private static DateTimeOffset GetDateFromTags(string filePath)
        {
            DateTime result = default;

            var tags = ImageMetadataReader.ReadMetadata(filePath);

            // Generic Exif
            var subIfdDirectory = tags.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            var found = (subIfdDirectory != null) && subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out result);

            if (!found)
            {
                found = (subIfdDirectory != null) && subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTime, out result);
            }

            if (!found)
            {
                var pngDirecectories = tags.OfType<MetadataExtractor.Formats.Png.PngDirectory>().ToList();
                foreach (var pngDirectory in pngDirecectories)
                {
                    // Print all metadata
                    foreach (var tag in pngDirectory.Tags)
                    {
                        // Console.WriteLine($"{pngDirectory.Name} - {tag.Name} = {tag.Description}");
                        if (tag.Description?.Contains("Creation Time: ") ?? false)
                        {
                            result = ParseDateTime(tag.Description.Replace("Creation Time: ", ""));

                            if (result != default(DateTime))
                            {
                                found = true;
                            }
                        }
                    }

                    if (found) continue;
                }
            }

            if (!found)
            {
                found = (subIfdDirectory != null) && subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeDigitized, out result);
            }

            if (!found)
            {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                if (extension == ".mp4" || extension == ".mov")
                {
                    var directories = new List<MetadataExtractor.Directory>();

                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        directories.AddRange(QuickTimeMetadataReader.ReadMetadata(stream));

                    // Mov
                    var movMetadata = directories.OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault();
                    var dateText = movMetadata?.GetDescription(QuickTimeMovieHeaderDirectory.TagCreated);
                    if (string.IsNullOrEmpty(dateText))
                    {
                        dateText = movMetadata?.GetDescription(QuickTimeMetadataHeaderDirectory.TagCreationDate);
                    }

                    if (dateText != null)
                    {
                        dateText = dateText.Substring(4, dateText.Length - 4);
                        result = ParseDateTime(dateText);
                        //found = true;
                    }
                }
                else if (extension == ".avi")
                {
                    // Avi
                    var aviMetadata = tags.OfType<AviDirectory>().FirstOrDefault();
                    var dateText = aviMetadata?.GetDescription(AviDirectory.TagDateTimeOriginal);

                    if (dateText != null)
                    {
                        dateText = dateText.Substring(4, dateText.Length - 4);
                        result = ParseDateTime(dateText);
                        //found = true;
                    }

                }
            }

            result = DateTime.SpecifyKind(result, DateTimeKind.Utc);
            return (DateTimeOffset)result;
        }

        private static DateTime ParseDateTime(string dateText)
        {
            if (string.IsNullOrEmpty(dateText))
            {
                return default(DateTime);
            }

            // TODO make this more generic
            //List<CultureInfo> cultures = new()
            //{
            //    CultureInfo.GetCultureInfo("en-US"),
            //    CultureInfo.GetCultureInfo("es-ES"),
            //    CultureInfo.GetCultureInfo("de-DE")
            //};

            //foreach (var culture in cultures)
            {
                foreach (var pattern in datePatterns)
                {
                    if (DateTime.TryParseExact(dateText, pattern, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime date))
                    {
                        return date;
                    }
                }
            }

            return default;
        }

        private static readonly string[] datePatterns =
        {
            "yyyy:MM:dd HH:mm:ss",
            "MMM dd HH:mm:ss yyyy",
            "MMM d HH:mm:ss yyyy",
            "yyyy:MM:dd HH:mm:ss.fff",
            "yyyy:MM:dd HH:mm:ss.fffzzz",
            "yyyy:MM:dd HH:mm:ss",
            "yyyy:MM:dd HH:mm:sszzz",
            "yyyy:MM:dd HH:mm",
            "yyyy:MM:dd HH:mmzzz",
            "yyyy-MM-dd HH:mm:ss.fff",
            "yyyy-MM-dd HH:mm:ss.fffzzz",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:sszzz",
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd HH:mmzzz",
            "yyyy.MM.dd HH:mm:ss",
            "yyyy.MM.dd HH:mm:sszzz",
            "yyyy.MM.dd HH:mm",
            "yyyy.MM.dd HH:mmzzz",
            "yyyy-MM-ddTHH:mm:ss.fff",
            "yyyy-MM-ddTHH:mm:ss.fffzzz",
            "yyyy-MM-ddTHH:mm:ss.ff",
            "yyyy-MM-ddTHH:mm:ss.f",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm",
            "yyyy-MM-ddTHH:mmzzz",
            "yyyy:MM:dd",
            "yyyy-MM-dd",
            "yyyy-MM",
            "yyyyMMdd", // as used in IPTC data
            "yyyy"
        };
    }
}
