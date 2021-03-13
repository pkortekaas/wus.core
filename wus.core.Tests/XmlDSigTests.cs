using Xunit;
using CoreWUS;
using System.IO;
using System.Text;

namespace wus.core.Tests
{
    public class XmlDSigTests : BaseTests
    {
        public XmlDSigTests()
            : base()
        {
        }

        [Fact]
        public void VerifySignature_ValidData_Pass()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);

            string responseFile = Path.Combine(TestDataPath, "valid-deliver-response.xml");
            string data = Encoding.UTF8.GetString(File.ReadAllBytes(responseFile));

            // When
            bool actual = xmlDSig.VerifySignature(data);

            // Then
            Assert.True(actual, "Verify valid XmlDSig signature");
        }

        [Fact]
        public void VerifySignature_TamperedData_Fail()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);

            string responseFile = Path.Combine(TestDataPath, "tampered-deliver-response.xml");
            string data = Encoding.UTF8.GetString(File.ReadAllBytes(responseFile));

            // When
            bool actual = xmlDSig.VerifySignature(data);

            // Then
            Assert.False(actual, "Verify tampered XmlDSig signature");
        }
    }
}
