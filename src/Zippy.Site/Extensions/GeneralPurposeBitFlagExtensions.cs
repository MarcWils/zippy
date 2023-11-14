namespace Zippy.Site.Extensions
{
    public static class GeneralPurposeBitFlagExtensions
    {
        public static IEnumerable<string> GetGeneralPurposeBitFlagExplanations(this ushort generalPurposeBitFlag, ushort compressionMethod)
        {
            if ((generalPurposeBitFlag & (1)) > 0)
            {
                yield return "Bit 0: File is encrypted";
            }


            if (compressionMethod == 6) // imploding
            {
                yield return $"Bit 1: Indicates a {((generalPurposeBitFlag & (1 << 1)) > 0 ? 8 : 4)}K sliding window was used for imploding";
                yield return $"Bit 2: Indicates {((generalPurposeBitFlag & (1 << 1)) > 0 ? 3 : 2)} Shannon-Fano trees were used to encode the sliding dictionary output for imploding";
            }

            if (compressionMethod == 8 || compressionMethod == 9) // Deflating
            {
                yield return (generalPurposeBitFlag & 0x0006) switch
                {
                    0 => "Bit 1 & 2: Normal (-en) compression option was used",
                    2 => "Bit 1 & 2: Maximum (-exx/-ex) compression option was used",
                    4 => "Bit 1 & 2: Fast (-ef) compression option was used",
                    6 => "Bit 1 & 2: Super Fast (-es) compression option was used",
                    _ => "Unknown"
                };
            }

            if (compressionMethod == 14) // LZMA
            {
                yield return (generalPurposeBitFlag & 0x0002) switch
                {
                    0 => $"Bit 1: indicates an end-of-stream (EOS) marker is used to mark the end of the compressed data stream",
                    2 => $"Bit 1:EOS marker is not present and the compressed data size must be known to extract",
                    _ => "Unknown"
                };
            }

            if ((generalPurposeBitFlag & (1 << 3)) > 0)
            {
                yield return "Bit 3: crc-32, compressed and uncompressed size are set to zero in local file header. he correct values are put in the data descriptor immediately following the compressed data";
            }

            if ((generalPurposeBitFlag & (1 << 5)) > 0)
            {
                yield return "Bit 5: File is compressed patched data (Note: Requires PKZIP version 2.70 or greater)";
            }

            if ((generalPurposeBitFlag & (1 << 6)) > 0)
            {
                yield return "Bit 6: Strong encryption. If this bit is set, you MUST set the version needed to extract value to at least 50 and you MUST also set bit 0. If AES encryption is used, the version needed to extract value MUST be at least 51";
            }

            if ((generalPurposeBitFlag & (1 << 7)) > 0)
            {
                yield return "Bit 7 is set but is not used";
            }

            if ((generalPurposeBitFlag & (1 << 8)) > 0)
            {
                yield return "Bit 8 is set but is not used";
            }

            if ((generalPurposeBitFlag & (1 << 9)) > 0)
            {
                yield return "Bit 9 is set but is not used";
            }

            if ((generalPurposeBitFlag & (1 << 10)) > 0)
            {
                yield return "Bit 10 is set but is not used";
            }

            if ((generalPurposeBitFlag & (1 << 11)) > 0)
            {
                yield return "Bit 11: the filename and comment fields for this file MUST be encoded using UTF-8";
            }

            if ((generalPurposeBitFlag & (1 << 12)) > 0)
            {
                yield return "Bit 12: Reserved by PKWARE for enhanced compression";
            }

            if ((generalPurposeBitFlag & (1 << 13)) > 0)
            {
                yield return "Bit 13: Selected data values in the Local Header are masked to hide their actual values. (Used for encrypting central directory)";
            }

            if ((generalPurposeBitFlag & (1 << 14)) > 0)
            {
                yield return "Bit 14: Reserved by PKWARE for alternate streams";
            }

            if ((generalPurposeBitFlag & (1 << 15)) > 0)
            {
                yield return "Bit 15: Reserved by PKWARE";
            }
        }


    }
}
