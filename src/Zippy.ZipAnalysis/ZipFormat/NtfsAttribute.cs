namespace Zippy.ZipAnalysis.ZipFormat
{
    public class NtfsAttribute
    {
        public ushort Tag { get; set; }
        public ushort Size { get; set; }
        public ulong FileLastModificationTime { get; set; }
        public ulong FileLastAccessTime { get; set; }
        public ulong FileCreationTime { get; set; }
    }
}
