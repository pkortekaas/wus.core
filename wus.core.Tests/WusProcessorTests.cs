using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CoreWUS;
using CoreWUS.Http;
using Moq;
using Xunit;

namespace wus.core.Tests
{
    public class WusProcessorTests : BaseTests
    {
        private readonly string _deliveryUrl = "https://cs-bedrijven.procesinfrastructuur.nl/cpl/aanleverservice/1.2";
        private readonly string _statusUrl = "https://cs-bedrijven.procesinfrastructuur.nl/cpl/statusinformatieservice/1.2";
        public WusProcessorTests()
            : base()
        {
        }

        [Fact]
        public void WusProcessor_Deliver_Pass()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);


            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "valid-deliver-response.xml");
            Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile));
            aanleverRequest request = CreateAanleverRequest();

            // Act
            aanleverResponse actual;
            using (X509Certificate2 certificate = GetCertificate(_certificateThumbPrint))
            {
                WusProcessor wp = new WusProcessor(httpClient.Object, logger, certificate);
                actual = wp.Deliver(request, new Uri(_deliveryUrl));
            }

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void WusProcessor_DeliverFault_Fail()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            string expected = "ALS100";

            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "fault-deliver-response.xml");
            Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile));
            aanleverRequest request = CreateAanleverRequest();

            // Act
            WusException actual;
            using (X509Certificate2 certificate = GetCertificate(_certificateThumbPrint))
            {
                WusProcessor wp = new WusProcessor(httpClient.Object, logger, certificate);
                actual = Assert.Throws<WusException>( () => wp.Deliver(request, new Uri(_deliveryUrl)));
            }

            // Assert
            Assert.Equal(expected, actual.WusCode);
        }

        [Fact]
        public void WusProcessor_DeliverNullArgument_Fail()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            string expected = "uri";

            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "valid-deliver-response.xml");
            Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile));
            aanleverRequest request = CreateAanleverRequest();

            // Act
            ArgumentNullException actual;
            using (X509Certificate2 certificate = GetCertificate(_certificateThumbPrint))
            {
                WusProcessor wp = new WusProcessor(httpClient.Object, logger, certificate);
                actual = Assert.Throws<ArgumentNullException>( () => wp.Deliver(request, null));
            }

            // Assert
            Assert.Equal(expected, actual.ParamName);
        }

        [Fact]
        public void WusProcessor_NewStatus_Pass()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);

            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "valid-newstatus-response.xml");
            Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile));
            getNieuweStatussenProcesRequest request = CreateNewStatusRequest();

            // Act
            IEnumerable<StatusResultaat> actual;
            using (X509Certificate2 certificate = GetCertificate(_certificateThumbPrint))
            {
                WusProcessor wp = new WusProcessor(httpClient.Object, logger, certificate);
                actual = wp.NewStatusProcess(request, new Uri(_statusUrl));
            }

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void WusProcessor_NewStatusFault_Fail()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            string expected = "STS100";

            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "fault-newstatus-response.xml");
            Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile));
            getNieuweStatussenProcesRequest request = CreateNewStatusRequest();

            // Act
            WusException actual;
            using (X509Certificate2 certificate = GetCertificate(_certificateThumbPrint))
            {
                WusProcessor wp = new WusProcessor(httpClient.Object, logger, certificate);
                actual = Assert.Throws<WusException>( () => wp.NewStatusProcess(request, new Uri(_statusUrl)));
            }

            // Assert
            Assert.Equal(expected, actual.WusCode);
        }

        [Fact]
        public void WusProcessor_NewStatusNullArgument_Fail()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            string expected = "uri";

            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "valid-newstatus-response.xml");
            Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile));
            getNieuweStatussenProcesRequest request = CreateNewStatusRequest();

            // Act
            ArgumentNullException actual;
            using (X509Certificate2 certificate = GetCertificate(_certificateThumbPrint))
            {
                WusProcessor wp = new WusProcessor(httpClient.Object, logger, certificate);
                actual = Assert.Throws<ArgumentNullException>( () => wp.NewStatusProcess(request, null));
            }

            // Assert
            Assert.Equal(expected, actual.ParamName);
        }

        private Mock<IWusHttpClient> CreateMockHttpClient(string result)
        {
            Mock<IWusHttpClient> httpClient = new Mock<IWusHttpClient>();
            httpClient.Setup(x => x.Post(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<byte[]>()))
                                .Returns(result);
            return httpClient;
        }

        private aanleverRequest CreateAanleverRequest()
        {
            return new aanleverRequest()
            {
                berichtsoort = "Omzetbelasting",
                aanleverkenmerk = Guid.NewGuid().ToString("D"),
                autorisatieAdres = "http://geenausp.nl",
                identiteitBelanghebbende = new identiteitType() {
                    nummer = "001000044B37",
                    type = "Fi"
                },
                rolBelanghebbende = "Bedrijf",
                berichtInhoud = new berichtInhoudType() {
                    mimeType = "text/xml",
                    bestandsnaam = "Omzetbelasting.xbrl",
                    inhoud = Encoding.UTF8.GetBytes("UnitTest")
                }
            };
        }

        private getNieuweStatussenProcesRequest CreateNewStatusRequest()
        {
            return new getNieuweStatussenProcesRequest()
            {
                kenmerk = Guid.NewGuid().ToString("D"),
                autorisatieAdres = "http://geenausp.nl"
            };
        }
    }
}