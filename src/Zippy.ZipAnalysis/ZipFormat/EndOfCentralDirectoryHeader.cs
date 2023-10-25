using System.Diagnostics.CodeAnalysis;
using System.Text;
using Zippy.ZipAnalysis.Extensions;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class EndOfCentralDirectoryHeader : ZipHeaderBase, IEndOfCentralDirectoryHeader
    {

        public static uint Signature { get => 0x06054b50; }
        public static uint MinimumLength => 22;
        public static uint MaximumLength => 22 + 65535;



        public ushort NumberOfThisDisk { get; set; }

        public ushort NumberOfDiskWithStartOfCentralDirectory { get; set; }

        public ushort NumberOfEntriesInCentralDirectoryOnThisDisk { get; set; }

        public ushort TotalNumberOfEntriesInCentralDirectory { get; set; }

        public uint SizeOfCentralDirectory { get; set; }

        public uint OffsetOfCentralDirectory { get; set; }

        public ushort ZipFileCommentLength { get { return (ushort)((ZipFileCommentBytes != null) ? ZipFileCommentBytes.Length : 0); } }

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

        public byte[] ZipFileCommentBytes { get; set; }

        public override ulong Length { get { return (ulong)(MinimumLength + ZipFileCommentLength); } }

        public override long PositionFirstByte { get; set; }


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
                    NumberOfThisDisk = reader.ReadUInt16();
                    NumberOfDiskWithStartOfCentralDirectory = reader.ReadUInt16();
                    NumberOfEntriesInCentralDirectoryOnThisDisk = reader.ReadUInt16();
                    TotalNumberOfEntriesInCentralDirectory = reader.ReadUInt16();
                    SizeOfCentralDirectory = reader.ReadUInt32();
                    OffsetOfCentralDirectory = reader.ReadUInt32();
                    var zipFileCommentLength = reader.ReadUInt16();

                    ZipFileCommentBytes = reader.ReadBytes(zipFileCommentLength);
                    return ZipFileCommentBytes.Length == zipFileCommentLength;
                }
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }



        /// Gaat op zoek naar de end-of-central-directory header en controleert dat deze volledig is.
        /// Er wordt achteraan begonnen met zoeken
        /// Er wordt maximaal gezocht naar de maximale lengte van de end of central directory header
        /// Stream moet seekable zijn, en de lengte moet op te vragen zijn
        public static EndOfCentralDirectoryHeader GetEndOfCentralDirectoryHeader(Stream source)
        {
            var startPos = Math.Max(0, source.Length - MinimumLength);
            var endPos = Math.Max(0, source.Length - MaximumLength);

            for (long position = startPos; position >= endPos; position--)
            {
                source.Position = position;

                if (source.ReadSignature() == Signature)
                {
                    var endOfCentralDirectoryHeader = new EndOfCentralDirectoryHeader();
                    if (endOfCentralDirectoryHeader.LoadFromStream(source))
                    {
                        return endOfCentralDirectoryHeader;
                    }
                }
            }
            return null;
        }


        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Header: End of central directory header");
            builder.AppendLine($"Position first byte: {PositionFirstByte}");
            builder.AppendLine($"Signature: {Signature:x}");
            builder.AppendLine($"Length: {Length}");
            builder.AppendLine($"MinimumLength: {MinimumLength}");
            builder.AppendLine($"MaximumLength: {MaximumLength}");
            builder.AppendLine($"NumberOfThisDisk: {NumberOfThisDisk}");
            builder.AppendLine($"NumberOfDiskWithStartOfCentralDirectory: {NumberOfDiskWithStartOfCentralDirectory}");
            builder.AppendLine($"NumberOfEntriesInCentralDirectoryOnThisDisk: {NumberOfEntriesInCentralDirectoryOnThisDisk}");
            builder.AppendLine($"TotalNumberOfEntriesInCentralDirectory: {TotalNumberOfEntriesInCentralDirectory}");
            builder.AppendLine($"SizeOfCentralDirectory: {SizeOfCentralDirectory}");
            builder.AppendLine($"OffsetOfCentralDirectory: {OffsetOfCentralDirectory}");
            builder.AppendLine($"ZipFileCommentLength: {ZipFileCommentLength}");
            builder.AppendLine($"ZipFileComment: {ZipFileComment}");
            return builder.ToString();
        }
    }
}
