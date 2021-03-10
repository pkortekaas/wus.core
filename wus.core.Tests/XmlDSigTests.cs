using System;
using Xunit;
using Moq;
using CoreWUS;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;

// https://www.patrickschadler.com/c-unit-tests-mocks/
// https://www.patrickschadler.com/c-unit-tests-mocks/
// https://xunit.net/docs/getting-started/netcore/cmdline

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
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);

            string responseFile = Path.Combine(TestDataPath, "valid-deliver-response.xml");
            string data = Encoding.UTF8.GetString(File.ReadAllBytes(responseFile));

            // Act
            bool actual = xmlDSig.VerifySignature(data);

            // Assert
            Assert.True(actual, "Verify valid XmlDSig signature");
        }

        [Fact]
        public void VerifySignature_TamperedData_Fail()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);

            string responseFile = Path.Combine(TestDataPath, "tampered-deliver-response.xml");
            string data = Encoding.UTF8.GetString(File.ReadAllBytes(responseFile));

            // Act
            bool actual = xmlDSig.VerifySignature(data);

            // Assert
            Assert.False(actual, "Verify tampered XmlDSig signature");
        }
    }
}
