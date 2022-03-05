
namespace renamee.Shared.Helpers
{
    public static class StringExtensions
    {
        public static string EnsureDirectoryEnding(this string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath)) return directoryPath;
            return directoryPath.Replace('\\', '/').TrimEnd('/') + '/';
        }

        static char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

        public static bool IsValidFileName(this string fileName)
        {
            return (fileName.IndexOfAny(invalidFileNameChars) >= 0);
        }
    }
}
