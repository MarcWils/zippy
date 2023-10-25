using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class NTFSExtraField : ExtraFieldBase
    {


        public class NTFSAttribute
        {
            public ushort Tag { get; set; }
            public ushort Size { get; set; }
            public ulong FileLastModificationTime { get; set; }
            public ulong FileLastAccessTime { get; set; }
            public ulong FileCreationTime { get; set; }
        }


        public NTFSExtraField(Stream source)
        {
            LoadFromStream(source);
        }

        public NTFSExtraField()
        {
        }


        public const ushort Tag = 10;


        public ushort ExtraBlockSize { get; set; }
        public uint Reserved { get; set; }


        public IEnumerable<NTFSAttribute> NTFSAttributes { get; private set; }


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
                            throw new ArgumentException("Wrong tag for NTFS");
                        }
                    }
                    PositionFirstByte = source.Position - 2;
                    ExtraBlockSize = reader.ReadUInt16();
                    Reserved = reader.ReadUInt32();

                    var ntfsAttributes = new List<NTFSAttribute>();
                    int _bytesToRead = ExtraBlockSize - 4; // 4 bytes al gelezen bij uitlezen ExtraBlockSize
                    while (_bytesToRead > 4)
                    {
                        var tag = reader.ReadUInt16();
                        var size = reader.ReadUInt16();
                        if (tag == 1 && size == 24) // het enige dat beschreven staat in de appnote..
                        {
                            ntfsAttributes.Add(new NTFSAttribute
                            {
                                Tag = tag,
                                Size = size,
                                FileLastModificationTime = reader.ReadUInt64(),
                                FileLastAccessTime = reader.ReadUInt64(),
                                FileCreationTime = reader.ReadUInt64()
                            });
                        }
                        _bytesToRead -= 28;
                    }

                    NTFSAttributes = ntfsAttributes;
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
                writer.Write(Reserved);
                foreach (var attribute in NTFSAttributes)
                {
                    writer.Write(attribute.Tag);
                    writer.Write(attribute.Size);
                    writer.Write(attribute.FileLastModificationTime);
                    writer.Write(attribute.FileLastAccessTime);
                    writer.Write(attribute.FileCreationTime);
                }
            }
            return result;
        }




        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Extra field: NTFS extra field");
            builder.AppendLine($"Position first byte: {PositionFirstByte}");
            builder.AppendLine($"Tag: {Tag}");
            builder.AppendLine($"Length: {Length}");

            builder.AppendLine($"ExtraBlockSize: {ExtraBlockSize}");
            builder.AppendLine($"Reserved: {Reserved}");
            foreach (var attribute in NTFSAttributes)
            {
                builder.AppendLine($"NTFS attribute:");
                builder.AppendLine($"  Tag: {attribute.Tag}");
                builder.AppendLine($"  Size: {attribute.Size}");
                builder.AppendLine($"  File last modification time: {DateTime.FromFileTime((long)attribute.FileLastModificationTime)}");
                builder.AppendLine($"  File last access time: {DateTime.FromFileTime((long)attribute.FileLastAccessTime)}");
                builder.AppendLine($"  File creation time: {DateTime.FromFileTime((long)attribute.FileCreationTime)}");
            }
            return builder.ToString();
        }

    }
}
