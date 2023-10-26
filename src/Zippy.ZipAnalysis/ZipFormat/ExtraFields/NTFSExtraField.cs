using System.Diagnostics.CodeAnalysis;
using System.Text;
using BinaryReader = Zippy.ZipAnalysis.IO.BinaryReader;

namespace Zippy.ZipAnalysis.ZipFormat.ExtraFields
{
    public class NtfsExtraField : ExtraFieldBase
    {

        public const ushort Tag = 10;


        public ushort ExtraBlockSize { get; set; }
        public uint Reserved { get; set; }


        public IEnumerable<NtfsAttribute> NtfsAttributes { get; private set; } = Enumerable.Empty<NtfsAttribute>();


        public override ushort Length { get => (ushort)(ExtraBlockSize + 4); }

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
                        throw new ArgumentException("Wrong tag for NTFS");
                    }
                }
                PositionFirstByte = source.Position - 2;
                ExtraBlockSize = await reader.ReadUInt16Async();
                Reserved = await reader.ReadUInt32Async();

                var ntfsAttributes = new List<NtfsAttribute>();
                int _bytesToRead = ExtraBlockSize - 4; // 4 bytes al gelezen bij uitlezen ExtraBlockSize
                while (_bytesToRead > 4)
                {
                    var tag = await reader.ReadUInt16Async();
                    var size = await reader.ReadUInt16Async();
                    if (tag == 1 && size == 24) // het enige dat beschreven staat in de appnote..
                    {
                        ntfsAttributes.Add(new NtfsAttribute
                        {
                            Tag = tag,
                            Size = size,
                            FileLastModificationTime = await reader.ReadUInt64Async(),
                            FileLastAccessTime = await reader.ReadUInt64Async(),
                            FileCreationTime = await reader.ReadUInt64Async(),
                        });
                    }
                    _bytesToRead -= 28;
                }

                NtfsAttributes = ntfsAttributes;
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
                writer.Write(Reserved);
                foreach (var attribute in NtfsAttributes)
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
            var builder = new StringBuilder();
            builder.AppendLine($"Extra field: NTFS extra field");
            builder.AppendLine($"Position first byte: {PositionFirstByte}");
            builder.AppendLine($"Tag: {Tag}");
            builder.AppendLine($"Length: {Length}");

            builder.AppendLine($"ExtraBlockSize: {ExtraBlockSize}");
            builder.AppendLine($"Reserved: {Reserved}");
            foreach (var attribute in NtfsAttributes)
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
