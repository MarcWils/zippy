using System.Diagnostics.CodeAnalysis;
using System.Text;
using Zippy.ZipAnalysis.Extensions;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class DataDescriptor : ZipHeaderBase
    {
        public const uint Signature = DataDescriptorOrSplitArchiveHeader.Signature; // Header is not manadatory for data descriptor. For now we only have support for data descriptors that include a header..

        public override ulong Length => 16;

        public uint Crc32 { get; set; }

        public uint CompressedSize { get; set; }

        public uint UncompressedSize { get; set; }



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
                Crc32 = await source.ReadUInt32Async();
                CompressedSize = await source.ReadUInt32Async();
                UncompressedSize = await source.ReadUInt32Async();
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
                writer.Write(Crc32);
                writer.Write(CompressedSize);
                writer.Write(UncompressedSize);
            }
            return result;
        }

    }
}