namespace Zippy.Site.Extensions
{
    public static class CompressionMethodExtensions
    {
        public static string ToCompressionMethod(this ushort compressionMethod)
        {
            return compressionMethod switch
            {
                0 => "Stored (no compression)",
                1 => "Shrunk",
                2 => "Reduced with compression factor 1",
                3 => "Reduced with compression factor 2",
                4 => "Reduced with compression factor 3",
                5 => "Reduced with compression factor 4",
                6 => "Imploded",
                7 => "Reserved for Tokenizing compression algorithm",
                8 => "Deflate",
                9 => "Enhanced Deflating using Deflate64(tm)",
                10 => "PKWARE Data Compression Library Imploding(old IBM TERSE)",
                11 => "Reserved by PKWARE",
                12 => "BZIP2",
                13 => "Reserved by PKWARE",
                14 => "LZMA",
                15 => "Reserved by PKWARE",
                16 => "IBM z/OS CMPSC Compression",
                17 => "Reserved by PKWARE",
                18 => "IBM TERSE(new)",
                19 => "IBM LZ77 z Architecture",
                20 => "deprecated (use method 93 for zstd)",
                93 => "Zstandard(zstd) Compression",
                94 => "MP3 Compression",
                95 => "XZ Compression",
                96 => "JPEG variant",
                97 => "WavPack compressed data",
                98 => "PPMd version I, Rev 1",
                99 => "AE-x encryption marker",
                _ => string.Empty
            };
        }
    }
}
