namespace MarcWils.Zippy.ZipAnalysis
{
    public class ZipInspector
    {
        private static readonly byte[] ZIP_LEAD_BYTES = new byte[] { 80, 75, 3, 4 };

        public async Task<bool> HasZipSignatureAsync(Stream stream)
        {
            var buffer = new byte[4];
            var bytesRead = await stream.ReadAsync(buffer, 0, 4);
            return bytesRead == ZIP_LEAD_BYTES.Length &&
                buffer.SequenceEqual(ZIP_LEAD_BYTES);
        }
    }
}