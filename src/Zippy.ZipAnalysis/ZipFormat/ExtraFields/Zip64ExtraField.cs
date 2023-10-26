using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class Zip64ExtraField : ExtraFieldBase
    {
        public Zip64ExtraField(Stream source)
        {
            LoadFromStream(source);
        }


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


        public bool LoadFromStream(Stream source) => LoadFromStream(source, false);

        public bool LoadFromStream(Stream source, bool includeTag)
        {
            try
            {
                using (var reader = new BinaryReader(source, Encoding.UTF8, true))
                {
                    if (includeTag)
                    {
                        var tag = reader.ReadUInt16();
                        if (tag != Tag)
                        {
                            throw new ArgumentException("Wrong tag for Zip64");
                        }
                    }
                    PositionFirstByte = source.Position - 2;
                    ExtraBlockSize = reader.ReadUInt16();

                    if (ExtraBlockSize >= 8)
                    {
                        UncompressedSize = reader.ReadUInt64();
                    }

                    if (ExtraBlockSize >= 16)
                    {
                        CompressedSize = reader.ReadUInt64();
                    }

                    if (ExtraBlockSize >= 24)
                    {
                        RelativeHeaderOffset = reader.ReadUInt64();
                    }

                    if (ExtraBlockSize == 28)
                    {
                        DiskStartNumber = reader.ReadUInt32();
                    }

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
            StringBuilder builder = new StringBuilder();
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
