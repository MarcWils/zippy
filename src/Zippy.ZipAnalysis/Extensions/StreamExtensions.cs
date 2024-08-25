using System.Buffers.Binary;

namespace Zippy.ZipAnalysis.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte> ReadByteAsync(this Stream source)
        {
            var buffer = new byte[1];
            var nrOfBytesRead = await source.ReadAsync(buffer.AsMemory(0, 1));
            return nrOfBytesRead == 0 ? throw new EndOfStreamException() : buffer[0];
        }


        public static async Task<ushort> ReadUInt16Async(this Stream source) => 
            BinaryPrimitives.ReadUInt16LittleEndian((await InternalReadAsync(source, new Memory<byte>(new byte[sizeof(ushort)]))).Span);
        public static async Task<uint> ReadUInt32Async(this Stream source) => 
            BinaryPrimitives.ReadUInt32LittleEndian((await InternalReadAsync(source, new Memory<byte>(new byte[sizeof(uint)]))).Span);
        public static async Task<ulong> ReadUInt64Async(this Stream source) => 
            BinaryPrimitives.ReadUInt64LittleEndian((await InternalReadAsync(source, new Memory<byte>(new byte[sizeof(ulong)]))).Span);


        public static async Task<byte[]> ReadBytesAsync(this Stream source, int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(count);

            if (count == 0)
            {
                return [];
            }

            byte[] result = new byte[count];
            int numRead = await source.ReadAtLeastAsync(result, result.Length, throwOnEndOfStream: false);

            if (numRead != result.Length)
            {
                // Trim array. This should happen on EOF & possibly net streams.
                result = result[..numRead];
            }

            return result;
        }



        private static async Task<Memory<byte>> InternalReadAsync(Stream source, Memory<byte> buffer)
        {
            await source.ReadExactlyAsync(buffer);
            return buffer;
        }
    }
}
