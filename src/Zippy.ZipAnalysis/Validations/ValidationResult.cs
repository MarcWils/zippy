namespace Zippy.ZipAnalysis.Validations
{
    public readonly record struct ValidationResult
    {
        public string Description { get; init; }

        public bool IsValid { get; init; }
    }
}
