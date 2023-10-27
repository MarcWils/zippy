using System.Text;

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


        public abstract bool LoadFromStream(Stream source, bool includeSignature = false);


        public abstract long PositionFirstByte { get; set; }

        public void WriteToStream(Stream stream)
        {
            var buffer = ToByteArray();
            stream.Write(buffer, 0, buffer.Length);
        }


        protected IEnumerable<ExtraFieldBase> ReadExtraFields(BinaryReader reader, int extraFieldLength)
        {
            var leftToRead = extraFieldLength;
            var extraFields = new List<ExtraFieldBase>();
            while (leftToRead >= 2)
            {
                var tag = reader.ReadUInt16();
                switch (tag)
                {
                    case Zip64ExtraField.Tag:
                        extraFields.Add(new Zip64ExtraField(reader.BaseStream));
                        break;
                    case NtfsExtraField.Tag:
                        extraFields.Add(new NtfsExtraField(reader.BaseStream));
                        break;
                    default:
                        extraFields.Add(new NotImplementedExtraField(tag, reader.BaseStream));
                        break;
                }
                leftToRead -= extraFields[^1].Length;
            }

            var reallyRead = extraFields.Sum(e => e.Length);
            return extraFieldLength != reallyRead
                ? throw new EndOfStreamException($"Extra fields incorrectly read. Expected {extraFieldLength}, but read {reallyRead}.")
                : extraFields;
        }
    }
}
