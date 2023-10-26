using System.Diagnostics.CodeAnalysis;
using System.Text;
using BinaryReader = Zippy.ZipAnalysis.IO.BinaryReader;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class Zip64ExtraField : ExtraFieldBase
    {
        public Zip64ExtraField()
        {
        }

        public const ushort Tag = 1;

        public ushort ExtraBlockSize { get; set; }
        public ulong UncompressedSize { get; set; }

        public ulong CompressedSize { get; set; }

        public ulong RelativeHeaderOffset { get; set; }

        public uint DiskStartNumber { get; set; }


        public override ushort Length { get => ((ushort)(ExtraBlockSize + 4)); }

        public long PositionFirstByte { get; set; }


        public override async Task<bool> LoadFromStreamAsync(Stream source, bool includeTag)
        {
            try
            {
                var reader = new BinaryReader(source);

                if (includeTag)
                {
                    var tag = await reader.ReadUInt16Async();
                    if (tag != Tag)
                    {
                        throw new ArgumentException("Wrong tag for Zip64");
                    }
                }
                PositionFirstByte = source.Position - 2;
                ExtraBlockSize = await reader.ReadUInt16Async();

                if (ExtraBlockSize >= 8)
                {
                    UncompressedSize = await reader.ReadUInt64Async();
                }

                if (ExtraBlockSize >= 16)
                {
                    CompressedSize = await reader.ReadUInt64Async();
                }

                if (ExtraBlockSize >= 24)
                {
                    RelativeHeaderOffset = await reader.ReadUInt64Async();
                }

                if (ExtraBlockSize == 28)
                {
                    DiskStartNumber = await reader.ReadUInt32Async();
                }

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
                writer.Write(Tag);
                writer.Write(ExtraBlockSize);

                if (ExtraBlockSize >= 8)
                {
                    writer.Write(UncompressedSize);
                }

                if (ExtraBlockSize >= 16)
                {
                    writer.Write(CompressedSize);
                }

                if (ExtraBlockSize >= 24)
                {
                    writer.Write(RelativeHeaderOffset);
                }

                if (ExtraBlockSize >= 28)
                {
                    writer.Write(DiskStartNumber);
                }
            }
            return result;
        }



        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Extra field: Zip 64 extra field");
            builder.AppendLine($"Position first byte: {PositionFirstByte}");
            builder.AppendLine($"Tag: {Tag}");
            builder.AppendLine($"Length: {Length}");

            builder.AppendLine($"ExtraBlockSize: {ExtraBlockSize}");
            builder.AppendLine($"UncompressedSize: {UncompressedSize}");
            builder.AppendLine($"CompressedSize: {CompressedSize}");
            builder.AppendLine($"RelativeHeaderOffset: {RelativeHeaderOffset}");
            builder.AppendLine($"DiskStartNumber: {DiskStartNumber}");
            return builder.ToString();
        }

    }
}
