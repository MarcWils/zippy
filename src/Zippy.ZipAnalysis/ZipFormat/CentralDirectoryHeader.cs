using System.Diagnostics.CodeAnalysis;
using System.Text;
using Zippy.ZipAnalysis.Extensions;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class CentralDirectoryHeader : ZipEntryHeaderBase
    {
        public CentralDirectoryHeader(Stream source)
        {
            LoadFromStream(source);
        }

        public CentralDirectoryHeader()
        {
        }

        public const uint Signature = 0x02014b50;


        public ushort VersionMadeBy { get; set; }


        public ushort FileCommentLength { get { return (ushort)((FileCommentBytes != null) ? FileCommentBytes.Length : 0); } }

        public ushort DiskNumberStart { get; set; }

        public ushort InternalFileAttributes { get; set; }

        public uint ExternalFileAttributes { get; set; }

        public uint RelativeOffsetOfLocalHeader { get; set; }


        public override ulong Length { get { return (ulong)46 + FileNameLength + ExtraFieldLength + FileCommentLength; } }


        public override ulong OffsetLocalFileHeader => RelativeOffsetOfLocalHeader;



        /// <summary>
        /// Let op, bij schrijven van de filecomment wordt de huidige encoding gebruikt
        /// Als die later wijzigt worden de bytes van de filecomment niet gewijzigd
        /// </summary>
        public string FileComment
        {
            get { return Encoding.GetString(FileCommentBytes); }
            set
            {
                if (value != null)
                {
                    FileCommentBytes = Encoding.GetBytes(value);
                }
            }
        }

        protected byte[] FileCommentBytes { get; set; }

        public override long PositionFirstByte { get; set; }









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
                    VersionMadeBy = reader.ReadUInt16();
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
                    var fileCommentLength = reader.ReadUInt16();
                    DiskNumberStart = reader.ReadUInt16();
                    InternalFileAttributes = reader.ReadUInt16();
                    ExternalFileAttributes = reader.ReadUInt32();
                    RelativeOffsetOfLocalHeader = reader.ReadUInt32();

                    FileNameBytes = reader.ReadBytes(fileNameLength);
                    ExtraFields = ReadExtraFields(reader, extraFieldLength);
                    FileCommentBytes = reader.ReadBytes(fileCommentLength);
                    return FileNameBytes.Length == fileNameLength && FileCommentBytes.Length == fileCommentLength;
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
                writer.Write(VersionMadeBy);
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
                writer.Write(FileCommentLength);
                writer.Write(DiskNumberStart);
                writer.Write(InternalFileAttributes);
                writer.Write(ExternalFileAttributes);
                writer.Write(RelativeOffsetOfLocalHeader);
                writer.Write(FileNameBytes);
                if (ExtraFields != null)
                {
                    foreach (var extraField in ExtraFields)
                    {
                        extraField.WriteToStream(helperStream);
                    }
                }

                if (FileCommentLength > 0)
                {
                    writer.Write(FileCommentBytes);
                }
            }
            return result;
        }


        public static IEnumerable<CentralDirectoryHeader> GetCentralDirectoryHeaders(Stream source, IEndOfCentralDirectoryHeader endOfCentralDirectoryHeader)
        {
            List<CentralDirectoryHeader> centralDirectoryHeaders = new List<CentralDirectoryHeader>();

            source.Seek(endOfCentralDirectoryHeader.CentralDirectoryOffset, SeekOrigin.Begin);
            while (source.ReadSignature() == Signature)
            {
                var centralDirectoryHeader = new CentralDirectoryHeader();
                if (centralDirectoryHeader.LoadFromStream(source))
                {
                    centralDirectoryHeaders.Add(centralDirectoryHeader);
                }
            }

            return centralDirectoryHeaders;
        }


        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Header: Central directory header");
            builder.AppendLine($"Position first byte: {PositionFirstByte}");
            builder.AppendLine($"Signature: {Signature:x}");
            builder.AppendLine($"Length: {Length}");
            builder.AppendLine($"VersionMadeBy: {VersionMadeBy}");
            builder.AppendLine($"VersionNeededToExtract: {VersionNeededToExtract}");
            builder.AppendLine($"GeneralPurposeBitFlag: {GeneralPurposeBitFlag}");
            builder.AppendLine($"CompressionMethod: {CompressionMethod}");
            builder.AppendLine($"LastModificationFileTime: {LastModificationFileTime.ToTime()}");
            builder.AppendLine($"LastModificationFileDate: {LastModificationFileDate.ToDate().ToShortDateString()}");
            builder.AppendLine($"Crc32: {Crc32}");
            builder.AppendLine($"CompressedSize: {CompressedSize}");
            builder.AppendLine($"UncompressedSize: {UncompressedSize}");
            builder.AppendLine($"FileNameLength: {FileNameLength}");
            builder.AppendLine($"ExtraFieldLength: {ExtraFieldLength}");
            builder.AppendLine($"FileCommentLength: {FileCommentLength}");
            builder.AppendLine($"DiskNumberStart: {DiskNumberStart}");
            builder.AppendLine($"InternalFileAttributes: {InternalFileAttributes}");
            builder.AppendLine($"ExternalFileAttributes: {ExternalFileAttributes}");
            builder.AppendLine($"RelativeOffsetOfLocalHeader: {RelativeOffsetOfLocalHeader}");
            builder.AppendLine($"FileName: {FileName}");
            builder.AppendLine($"FileComment: {FileComment}");
            builder.AppendLine($"{String.Concat(ExtraFields?.Select(e => e.ToString()) ?? new string[0])}");

            return builder.ToString();

        }
    }
}
