using Zippy.ZipAnalysis.Extensions;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class EndOfCentralDirectoryHeader : ZipHeaderBase, IEndOfCentralDirectoryHeader
    { 
        public const uint Signature = 0x06054b50;
        public static uint MinimumLength => 22;
        public static uint MaximumLength => 22 + 65535;



        public ushort NumberOfThisDisk { get; set; }

        public ushort NumberOfDiskWithStartOfCentralDirectory { get; set; }

        public ushort NumberOfEntriesInCentralDirectoryOnThisDisk { get; set; }

        public ushort TotalNumberOfEntriesInCentralDirectory { get; set; }

        public uint SizeOfCentralDirectory { get; set; }

        public uint OffsetOfCentralDirectory { get; set; }

        public ushort ZipFileCommentLength { get; set; }

        public string ZipFileComment
        {
            get { return ZipFileCommentLength > 0 ? DefaultEncoding.GetString(ZipFileCommentBytes) : string.Empty; }
            set
            {
                if (value != null)
                {
                    ZipFileCommentBytes = DefaultEncoding.GetBytes(value);
                }
            }
        }

        public byte[] ZipFileCommentBytes { get; set; } = Array.Empty<byte>();

        public override ulong Length { get { return MinimumLength + (ulong)ZipFileCommentBytes.Length; } }

        public long CentralDirectoryOffset { get => OffsetOfCentralDirectory; }

        public override byte[] ToByteArray()
        {
            var result = new byte[Length];
            using (var helperStream = new MemoryStream(result))
            using (var writer = new BinaryWriter(helperStream))
            {
                writer.Write(Signature);
                writer.Write(NumberOfThisDisk);
                writer.Write(NumberOfDiskWithStartOfCentralDirectory);
                writer.Write(NumberOfEntriesInCentralDirectoryOnThisDisk);
                writer.Write(TotalNumberOfEntriesInCentralDirectory);
                writer.Write(SizeOfCentralDirectory);
                writer.Write(OffsetOfCentralDirectory);
                writer.Write(ZipFileCommentLength);

                if (ZipFileCommentLength > 0)
                {
                    writer.Write(ZipFileCommentBytes);
                }
            }
            return result;
        }

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
                NumberOfThisDisk = await source.ReadUInt16Async();
                NumberOfDiskWithStartOfCentralDirectory = await source.ReadUInt16Async();
                NumberOfEntriesInCentralDirectoryOnThisDisk = await source.ReadUInt16Async();
                TotalNumberOfEntriesInCentralDirectory = await source.ReadUInt16Async();
                SizeOfCentralDirectory = await source.ReadUInt32Async();
                OffsetOfCentralDirectory = await source.ReadUInt32Async();
                ZipFileCommentLength = await source.ReadUInt16Async();

                ZipFileCommentBytes = await source.ReadBytesAsync(ZipFileCommentLength);
                return ZipFileCommentBytes.Length == ZipFileCommentLength;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }


    }
}
