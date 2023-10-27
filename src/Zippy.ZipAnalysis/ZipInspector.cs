using Zippy.ZipAnalysis.Extensions;
using Zippy.ZipAnalysis.Validations;
using Zippy.ZipAnalysis.ZipFormat;

namespace Zippy.ZipAnalysis
{
    public class ZipInspector
    {
        public static readonly long MaxSupportedSize = 25 * 1024 * 1024;

        private static readonly uint _commonHeaderMask = 0x0000FFFFu;
        private static readonly uint _commonHeader = 0x00004b50u;


        private readonly Stream _source;

        public ZipInspector(Stream source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        private static readonly Dictionary<uint, Func<Stream, Task<ZipHeaderBase>>> _supportedZipHeaders = new()
        {
            { LocalFileHeader.Signature, CreateFromStreamAsync<LocalFileHeader> },
            { CentralDirectoryHeader.Signature, CreateFromStreamAsync<CentralDirectoryHeader> },
            { EndOfCentralDirectoryHeader.Signature, CreateFromStreamAsync<EndOfCentralDirectoryHeader> },
            { Zip64EndOfCentralDirectoryLocatorHeader.Signature, CreateFromStreamAsync<Zip64EndOfCentralDirectoryLocatorHeader> },
            { Zip64EndOfCentralDirectoryRecordHeader.Signature, CreateFromStreamAsync<Zip64EndOfCentralDirectoryRecordHeader> },
            { DataDescriptorOrSplitArchiveHeader.Signature, DataDescriptorOrSplitArchiveHeader.GetCorrectZipHeader },
        };



        public async IAsyncEnumerable<ZipHeaderBase> GetZipHeadersAsync()
        {
            ZipHeaderBase? header;
            while ((header = await GetNextZipHeaderAsync(_source)) != null)
            {
                yield return header;
            }
        }


        private static async Task<ZipHeaderBase?> GetNextZipHeaderAsync(Stream source)
        {
            uint header = 0;

            try
            {
                while (!_supportedZipHeaders.ContainsKey(header))
                {
                    if ((header & (_commonHeaderMask << 24)) == (_commonHeader << 24) ||
                        (header & (_commonHeaderMask << 16)) == (_commonHeader << 16) ||
                        (header & (_commonHeaderMask << 8)) == (_commonHeader << 8))
                    {
                        var b = await source.ReadByteAsync();
                        header >>= 8;
                        header |= (uint)(b << 24);
                    }
                    else
                    {
                        header = await source.ReadUInt32Async();
                    }
                }

                return await _supportedZipHeaders[header](source);
            }
            catch (EndOfStreamException)
            {
                return null;
            }
        }



        private static async Task<ZipHeaderBase> CreateFromStreamAsync<T>(Stream stream) where T : ZipHeaderBase, new()
        {
            var zipHeader = new T();
            await zipHeader.LoadFromStreamAsync(stream);
            return zipHeader;
        }

        public static IEnumerable<ValidationResult> GetValdationResults(IEnumerable<ZipHeaderBase> _)
        {
            return Array.Empty<ValidationResult>();
        }
    }
}