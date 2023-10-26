using System.Diagnostics.CodeAnalysis;
using System.Text;
using Zippy.ZipAnalysis.Extensions;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class LocalFileHeader : ZipEntryHeaderBase
    {
        public LocalFileHeader(Stream source)
        {
            LoadFromStream(source);
        }

        public LocalFileHeader()
        {

        }

        public const uint Signature = 0x04034b50;

        public static uint MinimumLength => 30;

        public override ulong Length { get { return (ulong)MinimumLength + FileNameLength + ExtraFieldLength; } }

        public override long PositionFirstByte { get; set; }

        public override ulong OffsetLocalFileHeader => (ulong)PositionFirstByte;

        public override bool LoadFromStream(Stream source, bool includeSignature = false)
        {
            try
            {
                using (var reader = new BinaryReader(source, Encoding.UTF8, true))
                {
                    if (includeSignature)
                    {
                        var signature = reader.ReadUInt32();
                        if (signature != Signature)
                        {
                            throw new ArgumentException("Wrong signature");
                        }
                    }
                    PositionFirstByte = source.Position - 4;
                    VersionNeededToExtract = reader.ReadUInt16();
                    GeneralPurposeBitFlag = reader.ReadUInt16();
                    CompressionMethod = reader.ReadUInt16();
                    LastModificationFileTime = reader.ReadUInt16();
                    LastModificationFileDate = reader.ReadUInt16();
                    Crc32 = reader.ReadUInt32();
                    CompressedSize = reader.ReadUInt32();
                    UncompressedSize = reader.ReadUInt32();
                    var fileNameLength = reader.ReadUInt16();
                    var extraFieldLength = reader.ReadUInt16();

                    FileNameBytes = reader.ReadBytes(fileNameLength);
                    ExtraFields = ReadExtraFields(reader, extraFieldLength);

                    return FileNameBytes.Length == fileNameLength;
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

        public static IEnumerable<LocalFileHeader> GetLocalFileHeaders(Stream source, IEnumerable<CentralDirectoryHeader> centralDirectoryHeaders)
        {
            List<LocalFileHeader> localFileHeaders = new List<LocalFileHeader>();

            foreach (var centralDirectoryHeader in centralDirectoryHeaders)
            {
                source.Seek(centralDirectoryHeader.RelativeOffsetOfLocalHeader, SeekOrigin.Begin);

                if (source.ReadSignature() == Signature)
                {
                    var localFileHeader = new LocalFileHeader();
                    if (localFileHeader.LoadFromStream(source))
                    {
                        localFileHeaders.Add(localFileHeader);
                    }
                }
            }

            return localFileHeaders;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
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
            builder.AppendLine($"{String.Concat(ExtraFields?.Select(e => e.ToString()) ?? new string[0])}");
            return builder.ToString();
        }
    }
}