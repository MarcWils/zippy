namespace Zippy.Site.Extensions
{
    public static class NumericExtension
    {
        public static string ToHexString(this ushort value) => $"0x{value:x4}";
        public static string ToHexString(this uint value) => $"0x{value:x8}";
        public static string ToHexString(this ulong value) => $"0x{value:x16}";

        public static string ToBinaryString(this ushort value) => $"0b{Convert.ToString(value, 2).PadLeft(16, '0')}";
        public static string ToBinaryString(this uint value) => $"0b{Convert.ToString(value, 2).PadLeft(32, '0')}";
        public static string ToBinaryString(this ulong value) => $"0b{Convert.ToString((long)value, 2).PadLeft(64, '0')}";

    }
}
