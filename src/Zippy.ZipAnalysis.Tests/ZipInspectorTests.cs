using System.Reflection.PortableExecutable;
using System.Text;
using Zippy.ZipAnalysis.ZipFormat;

namespace Zippy.ZipAnalysis.Tests
{
    [TestClass]
    public class ZipInspectorTests
    {
        [TestMethod]
        public void Given_A_Valid_ZipArchive_GetZipHeaders_Should_Return_Zip_Headers()
        {
            using var fs = File.OpenRead(@"TestFiles/ValidZip.zip");
            var headers = ZipInspector.GetZipHeaders(fs);

            Assert.AreNotEqual(0, headers.Count());
            Assert.AreEqual(1, headers.OfType<LocalFileHeader>().Count());
            Assert.AreEqual(1, headers.OfType<CentralDirectoryHeader>().Count());
            Assert.AreEqual(1, headers.OfType<EndOfCentralDirectoryHeader>().Count());
        }

        [TestMethod]
        public void Given_An_Empty_ZipArchive_GetZipHeaders_Should_Return_Zip_Headers()
        {
            using var fs = File.OpenRead(@"TestFiles/EmptyZip.zip");
            var headers = ZipInspector.GetZipHeaders(fs);

            Assert.AreNotEqual(0, headers.Count());
            Assert.AreEqual(0, headers.OfType<LocalFileHeader>().Count());
            Assert.AreEqual(0, headers.OfType<CentralDirectoryHeader>().Count());
            Assert.AreEqual(1, headers.OfType<EndOfCentralDirectoryHeader>().Count());
        }

        [TestMethod]
        public void Given_A_Random_String_GetZipHeaders_Should_Return_No_Headers()
        {
            using var fs = new MemoryStream(Encoding.UTF8.GetBytes("Zippy"));
            var headers = ZipInspector.GetZipHeaders(fs);

            Assert.AreEqual(0, headers.Count());
        }
    }
}