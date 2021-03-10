
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
            // Assert
            using (X509Certificate2 certificate = GetCertificate(_certificateThumbPrint))
            {
                Assert.True(certificate != null, "X509Certificate cannot be found");
            }
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
            // Arrange
            string filePath = Path.Combine(TestDataPath, fileName);

            // Assert
            Assert.True(File.Exists(filePath), $"Verify {fileName} exists");
        }

    }
}