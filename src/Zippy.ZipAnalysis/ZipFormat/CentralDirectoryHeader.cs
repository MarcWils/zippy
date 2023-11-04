using System.Text;
using Zippy.ZipAnalysis.Extensions;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class CentralDirectoryHeader : ZipEntryHeaderBase
    { 
        public const uint Signature = 0x02014b50;


        public ushort VersionMadeBy { get; set; }


        public ushort FileCommentLength { get; set; }

        public ushort DiskNumberStart { get; set; }

        public ushort InternalFileAttributes { get; set; }

        public uint ExternalFileAttributes { get; set; }

        public uint RelativeOffsetOfLocalHeader { get; set; }


        public override ulong Length { get { return (ulong)46 + (ulong)FileNameBytes.Length + (ulong)ExtraFields.Sum(e => e.Length) + (ulong)FileCommentBytes.Length; } }


        public override ulong OffsetLocalFileHeader => RelativeOffsetOfLocalHeader;


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

        protected byte[] FileCommentBytes { get; set; } = Array.Empty<byte>();

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
                VersionMadeBy = await source.ReadUInt16Async();
                VersionNeededToExtract = await source.ReadUInt16Async();
                GeneralPurposeBitFlag = await source.ReadUInt16Async();
                CompressionMethod = await source.ReadUInt16Async();
                LastModificationFileTime = await source.ReadUInt16Async();
                LastModificationFileDate = await source.ReadUInt16Async();
                Crc32 = await source.ReadUInt32Async();
                CompressedSize = await source.ReadUInt32Async();
                UncompressedSize = await source.ReadUInt32Async();
                FileNameLength = await source.ReadUInt16Async();
                ExtraFieldLength = await source.ReadUInt16Async();
                FileCommentLength = await source.ReadUInt16Async();
                DiskNumberStart = await source.ReadUInt16Async();
                InternalFileAttributes = await source.ReadUInt16Async();
                ExternalFileAttributes = await source.ReadUInt32Async();
                RelativeOffsetOfLocalHeader = await source.ReadUInt32Async();

                FileNameBytes = await source.ReadBytesAsync(FileNameLength);
                ExtraFields = await ReadExtraFieldsAsync(source, ExtraFieldLength);
                FileCommentBytes = await source.ReadBytesAsync(FileCommentLength);
                return FileNameBytes.Length == FileNameLength && FileCommentBytes.Length == FileCommentLength;

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

    }
}
