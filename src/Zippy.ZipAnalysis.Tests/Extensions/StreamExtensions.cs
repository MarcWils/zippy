namespace Zippy.ZipAnalysis.Tests.Extensions
{
    internal static class StreamExtensions
    {
        public static byte[] ReadWholeFile(this Stream source)
        {
            using var helperStream = new MemoryStream();
            source.CopyTo(helperStream);
            return helperStream.ToArray();
        }
    }
}
