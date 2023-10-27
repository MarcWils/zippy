using Zippy.ZipAnalysis.ZipFormat;

namespace Zippy.ZipAnalysis.Validations
{
    public interface IZipHeadersValidator
    {
        string GetValidationError(IEnumerable<ZipHeaderBase> headers);
    }
}
