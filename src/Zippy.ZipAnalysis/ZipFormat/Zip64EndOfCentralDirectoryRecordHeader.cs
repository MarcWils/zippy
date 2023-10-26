using System.Diagnostics.CodeAnalysis;
using System.Text;

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

        public byte[] Zip64ExtensibleDataSector { get; set; }

        public ulong Zip64ExtensibleDataSectorLength { get { return (ulong)((Zip64ExtensibleDataSector != null) ? Zip64ExtensibleDataSector.LongLength : 0); } }

        public override ulong Length => MinimumLength + Zip64ExtensibleDataSectorLength;

        public override long PositionFirstByte { get; set; }


        public long CentralDirectoryOffset { get => (long)OffsetOfCentralDirectory; }

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
                    SizeOfZip64EndOfCentralDirectoryRecord = reader.ReadUInt64();
                    VersionMadeBy = reader.ReadUInt16();
                    VersionNeededToExtract = reader.ReadUInt16();
                    NumberOfThisDisk = reader.ReadUInt32();
                    NumberOfDiskWithStartOfCentralDirectory = reader.ReadUInt32();
                    NumberOfEntriesInCentralDirectoryOnThisDisk = reader.ReadUInt64();
                    TotalNumberOfEntriesInCentralDirectories = reader.ReadUInt64();
                    SizeOfCentralDirectory = reader.ReadUInt64();
                    OffsetOfCentralDirectory = reader.ReadUInt64();
                    var zip64ExtensibleDataSectorLength = (int)SizeOfZip64EndOfCentralDirectoryRecord - 44; // beperkte lengte (int ipv ulong)
                    Zip64ExtensibleDataSector = reader.ReadBytes(zip64ExtensibleDataSectorLength); 
                    return Zip64ExtensibleDataSector.Length == zip64ExtensibleDataSectorLength;
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
            StringBuilder builder = new StringBuilder();
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
