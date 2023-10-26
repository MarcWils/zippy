using BinaryReader = Zippy.ZipAnalysis.IO.BinaryReader;


namespace Zippy.ZipAnalysis.ZipFormat.ExtraFields
{

    public class NotImplementedExtraField : ExtraFieldBase
    {

        public ushort Tag { get; set; }

        public override ushort Length { get => (ushort)(ExtraBlockSize + 4); }

        public ushort ExtraBlockSize { get; set; }

        public byte[] Data { get; set; } = Array.Empty<byte>();

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

        /// <summary>
        /// Leest tot het einde van het blok
        /// </summary>
        public override async Task<bool> LoadFromStreamAsync(Stream source, bool includeTag)
        {
            try
            {
                var reader = new BinaryReader(source);

                if (includeTag)
                {
                    Tag = await reader.ReadUInt16Async();
                }

                PositionFirstByte = source.Position - 2;
                ExtraBlockSize = await reader.ReadUInt16Async();
                Data = await reader.ReadBytesAsync(ExtraBlockSize);
                return true;

            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }
    }
}
