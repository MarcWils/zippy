namespace Zippy.ZipAnalysis.IO
{
    public class FakeSeekableStream : Stream
    {

        private readonly Stream _underlyingStream;

        public FakeSeekableStream(Stream underlyingStream)
        {
            _underlyingStream = underlyingStream ?? throw new ArgumentNullException(nameof(underlyingStream));
        }


        public override bool CanRead => _underlyingStream.CanRead;

        public override bool CanSeek => true;

        public override bool CanWrite => _underlyingStream.CanWrite;

        public override long Length => _underlyingStream.Length;

        public override long Position { get => _underlyingStream.Position; set => _underlyingStream.Position = value; }

        public override void Flush()
        {
            _underlyingStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _underlyingStream.Read(buffer, offset, count);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _underlyingStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _underlyingStream.ReadAsync(buffer, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _underlyingStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _underlyingStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _underlyingStream?.Write(buffer, offset, count);
        }
    }
}
