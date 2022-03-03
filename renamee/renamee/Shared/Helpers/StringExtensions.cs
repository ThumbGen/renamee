
namespace renamee.Shared.Helpers
{
    public static class StringExtensions
    {
        public static string EnsureDirectoryEnding(this string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath)) return directoryPath;
            return directoryPath.Replace('\\', '/').TrimEnd('/') + '/';
        }
    }
}
