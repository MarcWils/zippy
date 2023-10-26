using Zippy.ZipAnalysis.Extensions;
using Zippy.ZipAnalysis.ZipFormat;

namespace Zippy.ZipAnalysis
{
    public static class ZipInspector
    {
        public static readonly long MaxSupportedSize = 25 * 1024 * 1024;

        private static readonly uint _commonHeaderMask = 0xFFFF;
        private static readonly uint _commonHeader = 0x4b50;
        
        public static async IAsyncEnumerable<ZipHeaderBase> GetZipHeadersAsync(Stream source)
        {
            uint possibleHeader = 0;
            int lastByte;

            while ((lastByte = await source.ReadByteAsync()) != -1)
            {
                possibleHeader >>= 8;
                possibleHeader |= (uint)(lastByte << 24);

                if ((possibleHeader & _commonHeaderMask) == _commonHeader)
                {
                    var header = possibleHeader switch
                    {
                        LocalFileHeader.Signature => (ZipHeaderBase)(await CreateFromStreamAsync<LocalFileHeader>(source)),
                        CentralDirectoryHeader.Signature => await CreateFromStreamAsync<CentralDirectoryHeader>(source),
                        EndOfCentralDirectoryHeader.Signature => await CreateFromStreamAsync<EndOfCentralDirectoryHeader>(source),
                        Zip64EndOfCentralDirectoryLocatorHeader.Signature => await CreateFromStreamAsync<Zip64EndOfCentralDirectoryLocatorHeader>(source),
                        Zip64EndOfCentralDirectoryRecordHeader.Signature => await CreateFromStreamAsync<Zip64EndOfCentralDirectoryRecordHeader>(source),
                        _ => null
                    };

                    if (header != null)
                    {
                        yield return header;
                    }
                }

            }
        }


        public static async Task<T> CreateFromStreamAsync<T>(Stream stream) where T : ZipHeaderBase, new()
        {
            var zipHeader = new T();
            await zipHeader.LoadFromStreamAsync(stream);
            return zipHeader;
        }
    }
}