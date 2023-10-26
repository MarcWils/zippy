using System.Text;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public class ZipFileHeaders
    {


        public ZipFileHeaders(Stream source)
        {
            LoadFromStream(source);
        }



        public IEnumerable<LocalFileHeader> LocalFileHeaders { get; private set; }

        public IEnumerable<CentralDirectoryHeader> CentralDirectoryHeaders { get; private set; }

        public Zip64EndOfCentralDirectoryLocatorHeader Zip64EndOfCentralDirectoryLocatorHeader { get; private set; }

        public Zip64EndOfCentralDirectoryRecordHeader Zip64EndOfCentralDirectoryRecordHeader { get; private set; }

        public EndOfCentralDirectoryHeader EndOfCentralDirectoryHeader { get; private set; }


        /// <summary>
        /// Load a whole zip file, stream need to support seeking
        /// </summary>
        public void LoadFromStream(Stream source)
        {
            EndOfCentralDirectoryHeader = EndOfCentralDirectoryHeader.GetEndOfCentralDirectoryHeader(source);
            Zip64EndOfCentralDirectoryLocatorHeader = Zip64EndOfCentralDirectoryLocatorHeader.GetZip64EndOfCentralDirectoryLocatorHeader(source, EndOfCentralDirectoryHeader);

            if (Zip64EndOfCentralDirectoryLocatorHeader != null)
            {
                Zip64EndOfCentralDirectoryRecordHeader = new Zip64EndOfCentralDirectoryRecordHeader();
                source.Position = (long)Zip64EndOfCentralDirectoryLocatorHeader.OffsetOfZip64EndOfCentralDirectory;
                Zip64EndOfCentralDirectoryRecordHeader.LoadFromStream(source, true);

                CentralDirectoryHeaders = CentralDirectoryHeader.GetCentralDirectoryHeaders(source, Zip64EndOfCentralDirectoryRecordHeader);
            }
            else if (EndOfCentralDirectoryHeader != null)
            {
                CentralDirectoryHeaders = CentralDirectoryHeader.GetCentralDirectoryHeaders(source, EndOfCentralDirectoryHeader);
            }

            if (CentralDirectoryHeaders != null)
            {
                LocalFileHeaders = LocalFileHeader.GetLocalFileHeaders(source, CentralDirectoryHeaders);
            }
        }


       

    }
}
