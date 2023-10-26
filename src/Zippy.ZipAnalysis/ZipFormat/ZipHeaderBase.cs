using System.Text;
using Zippy.ZipAnalysis.ZipFormat.ExtraFields;
using BinaryReader = Zippy.ZipAnalysis.IO.BinaryReader;

namespace Zippy.ZipAnalysis.ZipFormat
{
    public abstract class ZipHeaderBase
    {
        protected static Encoding DefaultEncoding => Encoding.GetEncoding("IBM437");

        static ZipHeaderBase()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public abstract byte[] ToByteArray();

        public abstract ulong Length { get; }

        public abstract Task<bool> LoadFromStreamAsync(Stream source, bool includeSignature);

        public long PositionFirstByte { get; set; }

        public async Task<bool> LoadFromStreamAsync(Stream source) => await LoadFromStreamAsync(source, false);
        
        

        public void WriteToStream(Stream stream)
        {
            var buffer = ToByteArray();
            stream.Write(buffer, 0, buffer.Length);
        }


        protected static async Task<IEnumerable<ExtraFieldBase>> ReadExtraFieldsAsync(Stream source, int extraFieldLength)
        {
            var leftToRead = extraFieldLength;
            var extraFields = new List<ExtraFieldBase>();
            var reader = new BinaryReader(source);
            while (leftToRead >= 2)
            {
                var tag = await reader.ReadUInt16Async();
                switch (tag)
                {
                    case Zip64ExtraField.Tag:
                        extraFields.Add(await CreateFromStreamAsync<Zip64ExtraField>(source)); 
                        break;
                    case NtfsExtraField.Tag: 
                        extraFields.Add(await CreateFromStreamAsync<NtfsExtraField>(source));
                        break;
                    default:
                        var notImplementedExtraField = await CreateFromStreamAsync<NotImplementedExtraField>(source);
                        notImplementedExtraField.Tag = tag;
                        extraFields.Add(notImplementedExtraField);
                        break;
                }
                leftToRead -= extraFields[^1].Length;
            }

            var reallyRead = extraFields.Sum(e => e.Length);
            return extraFieldLength != reallyRead
                ? throw new EndOfStreamException($"Extra fields incorrectly read. Expected {extraFieldLength}, but read {reallyRead}.")
                : extraFields;
        }


        public static async Task<T> CreateFromStreamAsync<T>(Stream stream) where T : ExtraFieldBase, new()
        {
            var zipHeader = new T();
            await zipHeader.LoadFromStreamAsync(stream);
            return zipHeader;
        }
    }
}
