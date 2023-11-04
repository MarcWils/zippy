using System.Globalization;

namespace Zippy.Site.Extensions
{
    public static class FileSizeExtensions
    {
        const int _precision = 2;

        public static string ToReadableFileSize(this long fileSize)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (fileSize == 0)
            {
                return "0" + suf[0];
            }

            long bytes = Math.Abs(fileSize);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), _precision);
            return (Math.Sign(fileSize) * num).ToString(CultureInfo.InvariantCulture) + suf[place];
        }

        public static string ToReadableFileSize(this uint fileSize)
        {
            return ToReadableFileSize((long)fileSize);
        }

        public static string ToReadableFileSize(this ulong fileSize)
        {
            return ToReadableFileSize((long)fileSize);
        }
    }
}
