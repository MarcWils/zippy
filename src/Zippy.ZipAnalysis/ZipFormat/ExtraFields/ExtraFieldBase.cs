using System.IO;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public abstract class ExtraFieldBase
    {
        public abstract byte[] ToByteArray();

        public abstract ushort Length { get; }

        public void WriteToStream(Stream stream)
        {
            var buffer = ToByteArray();
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
