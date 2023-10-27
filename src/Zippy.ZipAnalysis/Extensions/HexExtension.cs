namespace Zippy.ZipAnalysis.Extensions
{
    public static class HexExtension
    {
        public static string ToHexString(this uint value)
        {
            return "0x" + value.ToString("X8").ToLowerInvariant();
        }

        public static string ToHexString(this ushort value)
        {
            return "0x" + value.ToString("X4").ToLowerInvariant();
        }

        public static string ToHexString(this ulong value)
        {
            return "0x" + value.ToString("X8").ToLowerInvariant();
        }
    }
}
