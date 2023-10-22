using Zippy.ZipAnalysis.ZipFormat;

namespace Zippy.ZipAnalysis
{
    public class ZipInspector
    {
        public async Task<bool> HasZipSignatureAsync(Stream stream)
        {
            var buffer = new byte[4];
            var bytesRead = await stream.ReadAsync(buffer, 0, 4);
            return bytesRead == buffer.Length &&
                (buffer.SequenceEqual(LocalFileHeader.Signature) ||
                buffer.SequenceEqual(EndOfCentralDirectoryHeader.Signature));
        }
    }
}