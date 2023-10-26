using System.Buffers.Binary;

namespace Zippy.ZipAnalysis.IO
{
    internal class BinaryReader
    {
        private readonly Stream _stream;

        public BinaryReader(Stream input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (!input.CanRead)
            {
                throw new ArgumentException("Stream not readable");
            }
            _stream = input;
        }



        public virtual async Task<ushort> ReadUInt16Async() => BinaryPrimitives.ReadUInt16LittleEndian((await InternalReadAsync(new Memory<byte>(new byte[sizeof(ushort)]))).Span);
        public virtual async Task<uint> ReadUInt32Async() => BinaryPrimitives.ReadUInt32LittleEndian((await InternalReadAsync(new Memory<byte>(new byte[sizeof(uint)]))).Span);
        public virtual async Task<ulong> ReadUInt64Async() => BinaryPrimitives.ReadUInt64LittleEndian((await InternalReadAsync(new Memory<byte>(new byte[sizeof(ulong)]))).Span);


        public virtual async Task<byte[]> ReadBytesAsync(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (count == 0)
            {
                return Array.Empty<byte>();
            }

            byte[] result = new byte[count];
            int numRead = await _stream.ReadAtLeastAsync(result, result.Length, throwOnEndOfStream: false);

            if (numRead != result.Length)
            {
                // Trim array. This should happen on EOF & possibly net streams.
                result = result[..numRead];
            }

            return result;
        }


        private async Task<Memory<byte>> InternalReadAsync(Memory<byte> buffer)
        {
            await _stream.ReadExactlyAsync(buffer);
            return buffer;
        }


    }
}
