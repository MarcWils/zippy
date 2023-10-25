using System.Text;

namespace Zippy.ZipAnalysis.ZipFormat
{

    public class NotImplementedExtraField : ExtraFieldBase
    {

        public NotImplementedExtraField(ushort tag, Stream source)
        {
            Tag = tag;
            LoadFromStream(source);
        }

        public ushort Tag { get; set; }

        public override ushort Length { get => (ushort)(ExtraBlockSize + 4); }

        public ushort ExtraBlockSize { get; set; }

        public byte[] Data { get; set; }

        public long PositionFirstByte { get; set; }

        public override byte[] ToByteArray()
        {
            var result = new byte[Length];
            using (var helperStream = new MemoryStream(result))
            using (var writer = new BinaryWriter(helperStream))
            {
                writer.Write(Tag);
                writer.Write(ExtraBlockSize);
                writer.Write(Data);
            }
            return result;
        }

        public bool LoadFromStream(Stream source) => LoadFromStream(source, false);

        /// <summary>
        /// Leest tot het einde van het blok
        /// </summary>
        public bool LoadFromStream(Stream source, bool includeTag)
        {
            try
            {
                using (var reader = new BinaryReader(source, Encoding.UTF8, true))
                {
                    if (includeTag)
                    {
                        Tag = reader.ReadUInt16();
                    }

                    PositionFirstByte = source.Position - 2;
                    ExtraBlockSize = reader.ReadUInt16();
                    Data = reader.ReadBytes(ExtraBlockSize);
                    return true;
                }
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }
    }
}
