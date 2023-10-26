using Zippy.ZipAnalysis.Extensions;
using Zippy.ZipAnalysis.ZipFormat;

namespace Zippy.ZipAnalysis
{
    public static class ZipInspector
    {
        public static readonly long MaxSupportedSize = 25 * 1024 * 1024;
        
        public static async IAsyncEnumerable<ZipHeaderBase> GetZipHeadersAsync(Stream source)
        {
            uint possibleHeader = 0;
            int lastByte;

            while ((lastByte = await source.ReadByteAsync()) != -1)
            {
                possibleHeader >>= 8;
                possibleHeader |= (uint)(lastByte << 24);

                if ((possibleHeader & 0x000000FF) == 0x00000050)
                {
                    var header = possibleHeader switch
                    {
                        LocalFileHeader.Signature => (ZipHeaderBase)new LocalFileHeader(source),
                        CentralDirectoryHeader.Signature => new CentralDirectoryHeader(source),
                        EndOfCentralDirectoryHeader.Signature => new EndOfCentralDirectoryHeader(source),
                        _ => null
                    };

                    if (header != null)
                    {
                        yield return header;
                    }
                }

            }
        }
    }
}