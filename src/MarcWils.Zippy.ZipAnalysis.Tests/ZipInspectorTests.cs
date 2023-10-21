using System.Text;

namespace MarcWils.Zippy.ZipAnalysis.Tests
{
    [TestClass]
    public class ZipInspectorTests
    {
        [TestMethod]
        public async Task Given_A_Valid_ZipArchive_HasZipSignatureAsync_Should_Return_True()
        {
            using var fs = File.OpenRead(@"TestFiles\ValidZip.zip");
            var sut = new ZipInspector();
            Assert.IsTrue(await sut.HasZipSignatureAsync(fs));
        }

        [TestMethod]
        public async Task Given_A_Random_String_HasZipSignatureAsync_Should_Return_False()
        {
            using var fs = new MemoryStream(Encoding.UTF8.GetBytes("Zippy"));
            var sut = new ZipInspector();
            Assert.IsFalse(await sut.HasZipSignatureAsync(fs));
        }
    }
}