﻿using System.Diagnostics.CodeAnalysis;
using System.Text;
using Zippy.ZipAnalysis.Extensions;
using BinaryReader = Zippy.ZipAnalysis.IO.BinaryReader;

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

        public byte[] ZipFileCommentBytes { get; set; } = Array.Empty<byte>();

        public override ulong Length { get { return (MinimumLength + ZipFileCommentLength); } }

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
                NumberOfThisDisk = await reader.ReadUInt16Async();
                NumberOfDiskWithStartOfCentralDirectory = await reader.ReadUInt16Async();
                NumberOfEntriesInCentralDirectoryOnThisDisk = await reader.ReadUInt16Async();
                TotalNumberOfEntriesInCentralDirectory = await reader.ReadUInt16Async();
                SizeOfCentralDirectory = await reader.ReadUInt32Async();
                OffsetOfCentralDirectory = await reader.ReadUInt32Async();
                var zipFileCommentLength = await reader.ReadUInt16Async();

                ZipFileCommentBytes = await reader.ReadBytesAsync(zipFileCommentLength);
                return ZipFileCommentBytes.Length == zipFileCommentLength;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
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
