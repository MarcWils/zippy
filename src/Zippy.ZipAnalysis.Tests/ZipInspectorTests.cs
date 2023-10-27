using System.Text;
using Zippy.ZipAnalysis.ZipFormat;
using Zippy.ZipAnalysis.ZipFormat.ExtraFields;

namespace Zippy.ZipAnalysis.Tests
{
    [TestClass]
    public class ZipInspectorTests
    {
        [TestMethod]
        public async Task Given_A_Valid_ZipArchive_GetZipHeadersAsync_Should_Return_Headers()
        {
            using var fs = File.OpenRead(@"TestFiles/ValidZip.zip");
            var headers = await new ZipInspector(fs).GetZipHeadersAsync().ToArrayAsync();
            Assert.AreEqual(3, headers.Length);
            Assert.IsTrue(headers[0] is LocalFileHeader);
            Assert.IsTrue(headers[1] is CentralDirectoryHeader);
            Assert.IsTrue(headers[2] is EndOfCentralDirectoryHeader);
        }


        [TestMethod]
        public async Task Given_A_Empty_ZipArchive_GetZipHeadersAsync_Should_Return_Header()
        {
            using var fs = File.OpenRead(@"TestFiles/EmptyZip.zip");
            var headers = await new ZipInspector(fs).GetZipHeadersAsync().ToArrayAsync();
            Assert.AreEqual(1, headers.Length);
            Assert.IsTrue(headers[0] is EndOfCentralDirectoryHeader);
        }


        [TestMethod]
        public async Task Given_A_ZipArchive_With_Zip64_Entry_GetZipHeadersAsync_Should_Return_Header()
        {
            using var fs = File.OpenRead(@"TestFiles/Zip64.zip");
            var headers = await new ZipInspector(fs).GetZipHeadersAsync().ToArrayAsync();
            Assert.AreEqual(3, headers.Length);
            Assert.AreEqual(1, headers.OfType<LocalFileHeader>().Single().ExtraFields.OfType<Zip64ExtraField>().Count());
        }

        [TestMethod]
        public async Task Given_A_Random_String_GetZipHeadersAsync_Should_Return_No_Headers()
        {
            using var fs = new MemoryStream(Encoding.UTF8.GetBytes("Zippy"));
            var headers = await new ZipInspector(fs).GetZipHeadersAsync().ToArrayAsync();
            Assert.AreEqual(0, headers.Length);
        }
    }
}