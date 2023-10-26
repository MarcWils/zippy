namespace Zippy.ZipAnalysis.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<int> ReadByteAsync(this Stream source)
        {
            var buffer = new byte[1];
            var nrOfBytesRead = await source.ReadAsync(buffer, 0, 1);
            return nrOfBytesRead == 0 ? -1 : buffer[0];
        }
    }
}
