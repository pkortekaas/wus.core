
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace wus.core.Tests
{
    public class InitialTests : BaseTests
    {
        public InitialTests()
            : base()
        {
        }

        [Fact]
        public void X509Certificate_FindCertificate_Pass()
        {
            // Then
            WithX509Certificate( cert =>
            {
                Assert.True(cert != null, "X509Certificate cannot be found");
            });
        }

        [Theory]
        [InlineData("valid-deliver-response.xml")]
        [InlineData("tampered-deliver-response.xml")]
        [InlineData("fault-deliver-response.xml")]
        [InlineData("valid-newstatus-response.xml")]
        [InlineData("fault-newstatus-response.xml")]
        [InlineData("valid-instance.xbrl")]
        public void TestDataFiles_CheckExistence_Pass(string fileName)
        {
            // Given
            string filePath = Path.Combine(TestDataPath, fileName);

            // Then
            Assert.True(File.Exists(filePath), $"Verify {fileName} exists");
        }

    }
}