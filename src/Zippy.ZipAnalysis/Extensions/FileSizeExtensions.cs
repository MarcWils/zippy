namespace Zippy.ZipAnalysis.Extensions
{
    public static class FileSizeExtensions
    {
        public static string ToReadableFileSize(this long fileSize)
        {
            return $"{fileSize / 1024 / 1024}MB";
        }
    }
}
