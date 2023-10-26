using System.Diagnostics.CodeAnalysis;
using System.Text;
using Zippy.ZipAnalysis.Extensions;
using BinaryReader = Zippy.ZipAnalysis.IO.BinaryReader;

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
                var reader = new BinaryReader(source);

                if (includeSignature)
                {
                    var signature = await reader.ReadUInt32Async();
                    if (signature != Signature)
                    {
                        throw new ArgumentException("Wrong signature");
                    }
                }
                PositionFirstByte = source.Position - 4;
                VersionNeededToExtract = await reader.ReadUInt16Async();
                GeneralPurposeBitFlag = await reader.ReadUInt16Async();
                CompressionMethod = await reader.ReadUInt16Async();
                LastModificationFileTime = await reader.ReadUInt16Async();
                LastModificationFileDate = await reader.ReadUInt16Async();
                Crc32 = await reader.ReadUInt32Async();
                CompressedSize = await reader.ReadUInt32Async();
                UncompressedSize = await reader.ReadUInt32Async();
                var fileNameLength = await reader.ReadUInt16Async();
                var extraFieldLength = await reader.ReadUInt16Async();

                FileNameBytes = await reader.ReadBytesAsync(fileNameLength);
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


        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Header: Local file header");
            builder.AppendLine($"Position first byte: {PositionFirstByte}");
            builder.AppendLine($"Signature: {Signature:x}");
            builder.AppendLine($"Length: {Length}");
            builder.AppendLine($"VersionNeededToExtract: {VersionNeededToExtract}");
            builder.AppendLine($"GeneralPurposeBitFlag: {GeneralPurposeBitFlag}");
            builder.AppendLine($"CompressionMethod: {CompressionMethod}");
            builder.AppendLine($"LastFileModificationTime: {LastModificationFileTime.ToTime()}");
            builder.AppendLine($"LastFileModificationDate: {LastModificationFileDate.ToDate().ToShortDateString()}");
            builder.AppendLine($"Crc32: {Crc32}");
            builder.AppendLine($"CompressedSize: {CompressedSize}");
            builder.AppendLine($"UncompressedSize: {UncompressedSize}");
            builder.AppendLine($"FileNameLength: {FileNameLength}");
            builder.AppendLine($"ExtraFieldLength: {ExtraFieldLength}");
            builder.AppendLine($"FileName: {FileName}");
            builder.AppendLine($"{string.Concat(ExtraFields?.Select(e => e.ToString()) ?? Array.Empty<string>())}");
            return builder.ToString();
        }
    }
}