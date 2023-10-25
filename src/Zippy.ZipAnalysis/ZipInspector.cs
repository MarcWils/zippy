
using Zippy.ZipAnalysis.ZipFormat;

namespace Zippy.ZipAnalysis
{
    public class ZipInspector
    {
        public static IEnumerable<ZipHeaderBase> GetZipHeaders(Stream stream)
        {
            var endOfCentralDirectoryHeader = EndOfCentralDirectoryHeader.GetEndOfCentralDirectoryHeader(stream);

            if (endOfCentralDirectoryHeader != null)
            {
                var centralDirectoryHeaders = CentralDirectoryHeader.GetCentralDirectoryHeaders(stream, endOfCentralDirectoryHeader);
                var localFileHeaders = LocalFileHeader.GetLocalFileHeaders(stream, centralDirectoryHeaders);
                return localFileHeaders.Cast<ZipHeaderBase>().Concat(centralDirectoryHeaders).Concat(new ZipHeaderBase[] { endOfCentralDirectoryHeader });
            }
            

            return Array.Empty<ZipHeaderBase>();
            
        }
    }
}