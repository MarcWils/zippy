using System.IO;

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
    }
}
