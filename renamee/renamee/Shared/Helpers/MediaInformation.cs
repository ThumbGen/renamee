
namespace renamee.Shared.Helpers
{
    public class MediaInformation
    {
        public static readonly string[] VideoExtensions = { ".MOV", ".AVI", ".MP4", ".DIVX", ".WMV", ".LVIX" };
        public static readonly string[] PhotoExtensions = { ".JPG", ".JPEG", ".HEIC", ".DIVX", ".WMV", ".LVIX" };
        public static readonly List<string> MediaExtensions = VideoExtensions.Concat(PhotoExtensions).ToList();
    }
}
