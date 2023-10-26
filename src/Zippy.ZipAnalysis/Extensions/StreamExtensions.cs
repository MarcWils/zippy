namespace Zippy.ZipAnalysis.Extensions
{
    public static class StreamExtensions
    {
        public static uint ReadSignature(this Stream source)
        {
            uint header = 0;
            header |= (uint)source.ReadByte();
            header |= ((uint)source.ReadByte() << 8);
            header |= ((uint)source.ReadByte() << 16);
            header |= ((uint)source.ReadByte() << 24);
            return header;
        }


        public static async Task<int> ReadByteAsync(this Stream source)
        {
            var buffer = new byte[1];
            var nrOfBytesRead = await source.ReadAsync(buffer, 0, 1);
            return nrOfBytesRead == 0 ? -1 : buffer[0];
        }
    }
}
