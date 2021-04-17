using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CoreWUS;
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
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);


            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "valid-deliver-response.xml");

            aanleverRequest request = AanleverRequest;

            // When
            aanleverResponse actual = null;
            WithX509Certificate( cert =>
            {
                Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile), cert);
                WusProcessor wp = new WusProcessor(httpClient.Object, logger);
                actual = wp.Deliver(request, new Uri(_deliveryUrl));
            });

            // Then
            Assert.NotNull(actual);
        }

        [Fact]
        public void WusProcessor_DeliverFault_Fail()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            string expected = "ALS100";

            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "fault-deliver-response.xml");
            aanleverRequest request = AanleverRequest;

            // When
            WusException actual = null;
            WithX509Certificate( cert =>
            {
                Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile), cert);
                WusProcessor wp = new WusProcessor(httpClient.Object, logger);
                actual = Assert.Throws<WusException>( () => wp.Deliver(request, new Uri(_deliveryUrl)));
            });

            // Then
            Assert.Equal(expected, actual.WusCode);
        }

        [Fact]
        public void WusProcessor_DeliverNullArgument_Fail()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            string expected = "uri";

            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "valid-deliver-response.xml");
            aanleverRequest request = AanleverRequest;

            // When
            ArgumentNullException actual = null;
            WithX509Certificate( cert =>
            {
                Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile), cert);
                WusProcessor wp = new WusProcessor(httpClient.Object, logger);
                actual = Assert.Throws<ArgumentNullException>( () => wp.Deliver(request, null));
            });

            // Then
            Assert.Equal(expected, actual.ParamName);
        }

        [Fact]
        public void WusProcessor_NewStatus_Pass()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);

            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "valid-newstatus-response.xml");
            getNieuweStatussenProcesRequest request = NieuweStatussenProcesRequest;

            // When
            IEnumerable<StatusResultaat> actual = null;
            WithX509Certificate( cert =>
            {
                Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile), cert);
                WusProcessor wp = new WusProcessor(httpClient.Object, logger);
                actual = wp.NewStatusProcess(request, new Uri(_statusUrl));
            });

            // Then
            Assert.NotNull(actual);
        }

        [Fact]
        public void WusProcessor_NewStatusFault_Fail()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            string expected = "STS100";

            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "fault-newstatus-response.xml");
            getNieuweStatussenProcesRequest request = NieuweStatussenProcesRequest;

            // When
            WusException actual = null;
            WithX509Certificate( cert =>
            {
                Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile), cert);
                WusProcessor wp = new WusProcessor(httpClient.Object, logger);
                actual = Assert.Throws<WusException>( () => wp.NewStatusProcess(request, new Uri(_statusUrl)));
            });

            // Then
            Assert.Equal(expected, actual.WusCode);
        }

        [Fact]
        public void WusProcessor_NewStatusNullArgument_Fail()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            string expected = "uri";

            // Mock a valid response
            string responseFile = Path.Combine(TestDataPath, "valid-newstatus-response.xml");
            getNieuweStatussenProcesRequest request = NieuweStatussenProcesRequest;

            // When
            ArgumentNullException actual = null;
            WithX509Certificate( cert =>
            {
                Mock<IWusHttpClient> httpClient = CreateMockHttpClient(File.ReadAllText(responseFile), cert);
                WusProcessor wp = new WusProcessor(httpClient.Object, logger);
                actual = Assert.Throws<ArgumentNullException>( () => wp.NewStatusProcess(request, null));
            });

            // Then
            Assert.Equal(expected, actual.ParamName);
        }

        private Mock<IWusHttpClient> CreateMockHttpClient(string result, X509Certificate2 certificate)
        {
            Mock<IWusHttpClient> httpClient = new Mock<IWusHttpClient>();
            httpClient.Setup(x => x.Post(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<byte[]>()))
                                .Returns(result);
            httpClient.Setup(c => c.ClientCertificate).Returns(certificate);
            return httpClient;
        }
    }
}