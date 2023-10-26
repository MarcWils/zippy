using System.Text;
using Zippy.ZipAnalysis.ZipFormat.ExtraFields;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public abstract class ZipEntryHeaderBase : ZipHeaderBase
    {
        public abstract ulong OffsetLocalFileHeader { get; }

        public ushort VersionNeededToExtract { get; set; }

        public ushort GeneralPurposeBitFlag { get; set; }

        public ushort CompressionMethod { get; set; }

        public ushort LastModificationFileTime { get; set; }

        public ushort LastModificationFileDate { get; set; }

        public uint Crc32 { get; set; }

        public uint CompressedSize { get; set; }

        public uint UncompressedSize { get; set; }

        public ushort FileNameLength { get { return (ushort)((FileNameBytes != null) ? FileNameBytes.Length : 0); } }

        public ushort ExtraFieldLength { get { return (ushort)((ExtraFields != null) ? ExtraFields.Sum(e => e.Length) : 0); } }


        /// <summary>
        /// Let op, bij schrijven van de filename wordt de huidige encoding gebruikt
        /// Als die later wijzigt worden de bytes van de filename niet gewijzigd
        /// </summary>
        public string FileName
        {
            get { return FileNameLength > 0 ? Encoding.GetString(FileNameBytes) : string.Empty; }
            set
            {
                if (value != null)
                {
                    FileNameBytes = Encoding.GetBytes(value);
                }
            }
        }

        public IEnumerable<ExtraFieldBase> ExtraFields { get; set; } = Enumerable.Empty<ExtraFieldBase>();

        protected byte[] FileNameBytes { get; set; } = Array.Empty<byte>();


        public Encoding Encoding
        {
            get
            {
                return (GeneralPurposeBitFlag & 0x0800) > 0 ? Encoding.UTF8 : DefaultEncoding;
            }
        }


        public bool IsZip64
        {
            get
            {
                return ExtraFields.Any(ef => ef is Zip64ExtraField);
            }
        }

        public bool IsDirectory
        {
            get
            {
                return FileNameLength > 0 && (FileName.EndsWith('/') || FileName.EndsWith('\\'));
            }
        }

        /// <summary>
        ///  Houdt rekening met zip64
        /// </summary>
        public ulong FinalUncompressedSize
        {
            get
            {
                var zip64ExtraField = ExtraFields.OfType<Zip64ExtraField>().FirstOrDefault();
                if (zip64ExtraField != null && zip64ExtraField.UncompressedSize > 0)
                {
                    return zip64ExtraField.UncompressedSize;
                }

                return UncompressedSize;
            }
        }


        public ulong FinalCompressedSize
        {
            get
            {
                var zip64ExtraField = ExtraFields.OfType<Zip64ExtraField>().FirstOrDefault();
                if (zip64ExtraField != null && zip64ExtraField.CompressedSize > 0)
                {
                    return zip64ExtraField.CompressedSize;
                }

                return CompressedSize;
            }
        }


        public ulong FinalOffsetLocalFileHeader
        {
            get
            {
                var zip64ExtraField = ExtraFields.OfType<Zip64ExtraField>().FirstOrDefault();
                if (zip64ExtraField != null && zip64ExtraField.RelativeHeaderOffset > 0)
                {
                    return zip64ExtraField.RelativeHeaderOffset;
                }

                return OffsetLocalFileHeader;
            }
        }
    }
}
