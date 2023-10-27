using System.Diagnostics.CodeAnalysis;
using System.Text;
using Zippy.ZipAnalysis.Extensions;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class Zip64EndOfCentralDirectoryLocatorHeader : ZipHeaderBase
    {
        private static readonly long _length = 20;

        public const uint Signature = 0x07064b50;

        public uint NumberOfDiskWithStartOfZip64EndOfCentralDirectory { get; set; }

        public ulong OffsetOfZip64EndOfCentralDirectory { get; set; }

        public uint TotalNumberOfDisks { get; set; }


        public override ulong Length => (ulong)_length;

        public override async Task<bool> LoadFromStreamAsync(Stream source, bool includeSignature)
        {
            try
            {
                if (includeSignature)
                {
                    var signature = await source.ReadUInt32Async();
                    if (signature != Signature)
                    {
                        throw new ArgumentException("Wrong signature");
                    }
                }

                PositionFirstByte = source.Position - 4;
                NumberOfDiskWithStartOfZip64EndOfCentralDirectory = await source.ReadUInt32Async();
                OffsetOfZip64EndOfCentralDirectory = await source.ReadUInt64Async();
                TotalNumberOfDisks = await source.ReadUInt32Async();
                return true;
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


        


        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            var builder = new StringBuilder();
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
