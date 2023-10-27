namespace Zippy.ZipAnalysis.ZipFormat.ExtraFields
{
    public abstract class ExtraFieldBase
    {
        public abstract byte[] ToByteArray();

        public abstract ushort Length { get; }


        public async Task<bool> LoadFromStreamAsync(Stream source) => await LoadFromStreamAsync(source, false);
        public abstract Task<bool> LoadFromStreamAsync(Stream source, bool includeTag);

        public void WriteToStream(Stream stream)
        {
            var buffer = ToByteArray();
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
