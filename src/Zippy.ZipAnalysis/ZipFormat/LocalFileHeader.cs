using System.Diagnostics.CodeAnalysis;
using System.Text;
using Zippy.ZipAnalysis.Extensions;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class LocalFileHeader : ZipEntryHeaderBase
    {
        public const uint Signature = 0x04034b50;

        public static uint MinimumLength => 30;

        public override ulong Length { get { return (ulong)MinimumLength + FileNameLength + ExtraFieldLength; } }

        public override ulong OffsetLocalFileHeader => (ulong)PositionFirstByte;

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
                VersionNeededToExtract = await source.ReadUInt16Async();
                GeneralPurposeBitFlag = await source.ReadUInt16Async();
                CompressionMethod = await source.ReadUInt16Async();
                LastModificationFileTime = await source.ReadUInt16Async();
                LastModificationFileDate = await source.ReadUInt16Async();
                Crc32 = await source.ReadUInt32Async();
                CompressedSize = await source.ReadUInt32Async();
                UncompressedSize = await source.ReadUInt32Async();
                var fileNameLength = await source.ReadUInt16Async();
                var extraFieldLength = await source.ReadUInt16Async();

                FileNameBytes = await source.ReadBytesAsync(fileNameLength);
                ExtraFields = await ReadExtraFieldsAsync(source, extraFieldLength);

                return FileNameBytes.Length == fileNameLength;
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
                writer.Write(VersionNeededToExtract);
                writer.Write(GeneralPurposeBitFlag);
                writer.Write(CompressionMethod);
                writer.Write(LastModificationFileTime);
                writer.Write(LastModificationFileDate);
                writer.Write(Crc32);
                writer.Write(CompressedSize);
                writer.Write(UncompressedSize);
                writer.Write(FileNameLength);
                writer.Write(ExtraFieldLength);
                writer.Write(FileNameBytes);

                if (ExtraFields != null)
                {
                    foreach (var extraField in ExtraFields)
                    {
                        extraField.WriteToStream(helperStream);
                    }
                }
            }
            return result;
        }

    }
}