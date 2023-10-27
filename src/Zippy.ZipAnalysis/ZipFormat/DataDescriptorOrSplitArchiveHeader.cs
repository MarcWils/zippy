namespace Zippy.ZipAnalysis.ZipFormat
{
    public static class DataDescriptorOrSplitArchiveHeader
    {
        public const uint Signature = 0x08074b50; // signature is used for Data descriptor and split archives

        public static async Task<ZipHeaderBase> GetCorrectZipHeader(Stream source)
        {
            // 8.5.3 Spanned/Split archives created using PKZIP for Windows
            //       (V2.50 or greater), PKZIP Command Line(V2.50 or greater),
            //       or PKZIP Explorer will include a special spanning
            //       signature as the first 4 bytes of the first segment of
            //       the archive.  This signature(0x08074b50) will be
            //       followed immediately by the local header signature for
            //       the first file in the archive.
            ZipHeaderBase header = (source.Position - 4 == 0) ? new SplitArchiveHeader() : new DataDescriptor();
            await header.LoadFromStreamAsync(source);
            return header;
        }

    }
}
