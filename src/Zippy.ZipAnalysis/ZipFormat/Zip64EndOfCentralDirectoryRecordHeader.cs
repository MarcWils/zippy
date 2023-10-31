using System.Diagnostics.CodeAnalysis;
using System.Text;
using Zippy.ZipAnalysis.Extensions;

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
                if (includeSignature)
                {
                    var signature = await source.ReadUInt32Async();
                    if (signature != Signature)
                    {
                        throw new ArgumentException("Wrong signature");
                    }
                }

                PositionFirstByte = source.Position - 4;
                SizeOfZip64EndOfCentralDirectoryRecord = await source.ReadUInt64Async();
                VersionMadeBy = await source.ReadUInt16Async();
                VersionNeededToExtract = await source.ReadUInt16Async();
                NumberOfThisDisk = await source.ReadUInt32Async();
                NumberOfDiskWithStartOfCentralDirectory = await source.ReadUInt32Async();
                NumberOfEntriesInCentralDirectoryOnThisDisk = await source.ReadUInt64Async();
                TotalNumberOfEntriesInCentralDirectories = await source.ReadUInt64Async();
                SizeOfCentralDirectory = await source.ReadUInt64Async();
                OffsetOfCentralDirectory = await source.ReadUInt64Async();
                var zip64ExtensibleDataSectorLength = (int)SizeOfZip64EndOfCentralDirectoryRecord - 44; // beperkte lengte (int ipv ulong)
                Zip64ExtensibleDataSector = await source.ReadBytesAsync(zip64ExtensibleDataSectorLength);
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

    }
}
