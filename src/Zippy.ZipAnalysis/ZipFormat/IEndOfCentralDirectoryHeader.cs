namespace Zippy.ZipAnalysis.ZipFormat
{
    public interface IEndOfCentralDirectoryHeader
    {
        long CentralDirectoryOffset { get; }
    }
}
