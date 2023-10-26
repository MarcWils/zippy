using System.Diagnostics.CodeAnalysis;
using System.Text;
using BinaryReader = Zippy.ZipAnalysis.IO.BinaryReader;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class Zip64EndOfCentralDirectoryRecordHeader : ZipHeaderBase, IEndOfCentralDirectoryHeader
    {
        public const uint Signature = 0x06064b50;

        public static uint MinimumLength => 56;

        public ulong SizeOfZip64EndOfCentralDirectoryRecord { get; set; }

        public ushort VersionMadeBy { get; set; }

        public ushort VersionNeededToExtract { get; set; }

        public uint NumberOfThisDisk { get; set; }

        public uint NumberOfDiskWithStartOfCentralDirectory { get; set; }

        public ulong NumberOfEntriesInCentralDirectoryOnThisDisk { get; set; }

        public ulong TotalNumberOfEntriesInCentralDirectories { get; set; }

        public ulong SizeOfCentralDirectory { get; set; }

        public ulong OffsetOfCentralDirectory { get; set; }

        public byte[] Zip64ExtensibleDataSector { get; set; } = Array.Empty<byte>();

        public ulong Zip64ExtensibleDataSectorLength { get { return (ulong)((Zip64ExtensibleDataSector != null) ? Zip64ExtensibleDataSector.LongLength : 0); } }

        public override ulong Length => MinimumLength + Zip64ExtensibleDataSectorLength;


        public long CentralDirectoryOffset { get => (long)OffsetOfCentralDirectory; }

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
                SizeOfZip64EndOfCentralDirectoryRecord = await reader.ReadUInt64Async();
                VersionMadeBy = await reader.ReadUInt16Async();
                VersionNeededToExtract = await reader.ReadUInt16Async();
                NumberOfThisDisk = await reader.ReadUInt32Async();
                NumberOfDiskWithStartOfCentralDirectory = await reader.ReadUInt32Async();
                NumberOfEntriesInCentralDirectoryOnThisDisk = await reader.ReadUInt64Async();
                TotalNumberOfEntriesInCentralDirectories = await reader.ReadUInt64Async();
                SizeOfCentralDirectory = await reader.ReadUInt64Async();
                OffsetOfCentralDirectory = await reader.ReadUInt64Async();
                var zip64ExtensibleDataSectorLength = (int)SizeOfZip64EndOfCentralDirectoryRecord - 44; // beperkte lengte (int ipv ulong)
                Zip64ExtensibleDataSector = await reader.ReadBytesAsync(zip64ExtensibleDataSectorLength);
                return Zip64ExtensibleDataSector.Length == zip64ExtensibleDataSectorLength;
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
                writer.Write(SizeOfZip64EndOfCentralDirectoryRecord);
                writer.Write(VersionMadeBy);
                writer.Write(VersionNeededToExtract);
                writer.Write(NumberOfThisDisk);
                writer.Write(NumberOfDiskWithStartOfCentralDirectory);
                writer.Write(NumberOfEntriesInCentralDirectoryOnThisDisk);
                writer.Write(TotalNumberOfEntriesInCentralDirectories);
                writer.Write(SizeOfCentralDirectory);
                writer.Write(OffsetOfCentralDirectory);
                if (Zip64ExtensibleDataSector != null)
                {
                    writer.Write(Zip64ExtensibleDataSector);
                }
            }
            return result;
        }



        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Header: Zip64 end of central directory record header");
            builder.AppendLine($"Position first byte: {PositionFirstByte}");
            builder.AppendLine($"Length: {Length}");
            builder.AppendLine($"Signature: {Signature:x}");
            builder.AppendLine($"SizeOfZip64EndOfCentralDirectoryRecord: {SizeOfZip64EndOfCentralDirectoryRecord:x}");
            builder.AppendLine($"VersionMadeBy: {VersionMadeBy}");
            builder.AppendLine($"VersionNeededToExtract: {VersionNeededToExtract}");
            builder.AppendLine($"NumberOfThisDisk: {NumberOfThisDisk}");
            builder.AppendLine($"NumberOfDiskWithStartOfCentralDirectory: {NumberOfDiskWithStartOfCentralDirectory}");
            builder.AppendLine($"NumberOfEntriesInCentralDirectoryOnThisDisk: {NumberOfEntriesInCentralDirectoryOnThisDisk}");
            builder.AppendLine($"TotalNumberOfEntriesInCentralDirectories: {TotalNumberOfEntriesInCentralDirectories}");
            builder.AppendLine($"SizeOfCentralDirectory: {SizeOfCentralDirectory}");
            builder.AppendLine($"OffsetOfCentralDirectory: {OffsetOfCentralDirectory}");
            builder.AppendLine($"Zip64ExtensibleDataSector: {(Zip64ExtensibleDataSector != null ? BitConverter.ToString(Zip64ExtensibleDataSector) : "")}");

            return builder.ToString();

        }
    }
}
