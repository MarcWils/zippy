using System.Diagnostics.CodeAnalysis;
using System.Text;
using Zippy.ZipAnalysis.Extensions;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class Zip64EndOfCentralDirectoryLocatorHeader : ZipHeaderBase
    {
        private static long _length = 20;

        public static uint Signature { get => 0x07064b50; }

        public uint NumberOfDiskWithStartOfZip64EndOfCentralDirectory { get; set; }

        public ulong OffsetOfZip64EndOfCentralDirectory { get; set; }

        public uint TotalNumberOfDisks { get; set; }


        public override ulong Length => (ulong)_length;

        public override long PositionFirstByte { get; set; }

        public override bool LoadFromStream(Stream source, bool includeSignature = false)
        {
            try
            {
                using (var reader = new BinaryReader(source, Encoding.UTF8, true))
                {
                    if (includeSignature)
                    {
                        var signature = reader.ReadUInt32();
                        if (signature != Signature)
                        {
                            throw new ArgumentException("Wrong signature");
                        }
                    }

                    PositionFirstByte = source.Position - 4;
                    NumberOfDiskWithStartOfZip64EndOfCentralDirectory = reader.ReadUInt32();
                    OffsetOfZip64EndOfCentralDirectory = reader.ReadUInt64();
                    TotalNumberOfDisks = reader.ReadUInt32();
                    return true;
                }
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }


        public override byte[] ToByteArray()
        {
            var result = new byte[Length];
            using (var helperStream = new MemoryStream(result))
            using (var writer = new BinaryWriter(helperStream))
            {
                writer.Write(Signature);
                writer.Write(NumberOfDiskWithStartOfZip64EndOfCentralDirectory);
                writer.Write(OffsetOfZip64EndOfCentralDirectory);
                writer.Write(TotalNumberOfDisks);
            }
            return result;
        }


        public static Zip64EndOfCentralDirectoryLocatorHeader GetZip64EndOfCentralDirectoryLocatorHeader(Stream source, EndOfCentralDirectoryHeader endOfCentralDirectoryHeader)
        {
            long startPos, endPos;

            if (endOfCentralDirectoryHeader.PositionFirstByte - _length > 0) // endOfCentralDirectoryHeader was al uitgelezen en adh daarvan kan positie van dit record bepaald worden 
            {
                startPos = endOfCentralDirectoryHeader.PositionFirstByte - _length;
                endPos = startPos;
            }
            else
            {
                startPos = Math.Max(0, source.Length - EndOfCentralDirectoryHeader.MinimumLength - _length);
                endPos = Math.Max(0, source.Length - EndOfCentralDirectoryHeader.MaximumLength - _length);
            }

            for (long pos = startPos; pos >= endPos; pos--)
            {
                source.Position = pos;
                if (source.ReadSignature() == Signature)
                {
                    var zip64EndOfCentralDirectoryLocatorHeader = new Zip64EndOfCentralDirectoryLocatorHeader();
                    if (zip64EndOfCentralDirectoryLocatorHeader.LoadFromStream(source))
                    {
                        return zip64EndOfCentralDirectoryLocatorHeader;
                    }
                }
            }

            return null;
        }


        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Header: Zip64 end of central directory locator header");
            builder.AppendLine($"Position first byte: {PositionFirstByte}");
            builder.AppendLine($"Length: {Length}");
            builder.AppendLine($"Signature: {Signature:x}");
            builder.AppendLine($"NumberOfDiskWithStartOfZip64EndOfCentralDirectory: {NumberOfDiskWithStartOfZip64EndOfCentralDirectory:x}");
            builder.AppendLine($"OffsetOfZip64EndOfCentralDirectory: {OffsetOfZip64EndOfCentralDirectory}");
            builder.AppendLine($"TotalNumberOfDisks: {TotalNumberOfDisks}");

            return builder.ToString();

        }
    }
}
