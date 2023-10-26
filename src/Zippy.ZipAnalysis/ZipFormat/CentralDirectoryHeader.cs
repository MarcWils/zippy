﻿using System.Diagnostics.CodeAnalysis;
using System.Text;
using Zippy.ZipAnalysis.Extensions;
using BinaryReader = Zippy.ZipAnalysis.IO.BinaryReader;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class CentralDirectoryHeader : ZipEntryHeaderBase
    { 
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

        protected byte[] FileCommentBytes { get; set; } = Array.Empty<byte>();

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
                VersionMadeBy = await reader.ReadUInt16Async();
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
                var fileCommentLength = await reader.ReadUInt16Async();
                DiskNumberStart = await reader.ReadUInt16Async();
                InternalFileAttributes = await reader.ReadUInt16Async();
                ExternalFileAttributes = await reader.ReadUInt32Async();
                RelativeOffsetOfLocalHeader = await reader.ReadUInt32Async();

                FileNameBytes = await reader.ReadBytesAsync(fileNameLength);
                ExtraFields = await ReadExtraFieldsAsync(source, extraFieldLength);
                FileCommentBytes = await reader.ReadBytesAsync(fileCommentLength);
                return FileNameBytes.Length == fileNameLength && FileCommentBytes.Length == fileCommentLength;

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


        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            var builder = new StringBuilder();
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
            builder.AppendLine($"{string.Concat(ExtraFields?.Select(e => e.ToString()) ?? Array.Empty<string>())}");

            return builder.ToString();

        }
    }
}