using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using CoreWUS;
using CoreWUS.Extensions;
using Xunit;

namespace wus.core.Tests
{
    public class WusDocumentTests : BaseTests
    {

        public WusDocumentTests()
            : base()
        {
        }

        [Fact]
        public void CreateDocumentBytes_Create_Pass()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);

            // Act
            byte[] docBytes = GetCreatedDocumentBytes(xmlDSig);

            // Assert
            Assert.True(docBytes != null, "Cannot create document");
        }

        [Fact]
        public void CreateDocumentBytes_CreateAndVerifySignature_Pass()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);

            // Act
            byte[] docBytes = GetCreatedDocumentBytes(xmlDSig);

            bool actual = false;
            if (docBytes != null)
            {
                string xmlData = GetStringWithoutBOM(docBytes);
                actual = xmlDSig.VerifySignature(xmlData);
            }

            // Assert
            Assert.True(actual, "Signature of created document cannot be verified");
        }

        private byte[] GetCreatedDocumentBytes(IWusXmlDSig xmlDSig)
        {
            // Arrange
            ILogger logger = null;
            IWusDocument wusDocument = new WusDocument(xmlDSig, logger);

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = true
            };

            aanleverRequest request = new aanleverRequest()
            {
                berichtsoort = "Omzetbelasting",
                aanleverkenmerk = System.Guid.NewGuid().ToString("D"),
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

            byte[] docBytes;
            using (X509Certificate2 certificate = GetCertificate(_certificateThumbPrint))
            {
                WusDocumentInfo wusDocumentInfo = new WusDocumentInfo()
                {
                    Envelope = request.ToXElement(xmlWriterSettings),
                    SoapAction = "http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/AanleverService/aanleverenRequest",
                    Uri = new Uri("https://cs-bedrijven.procesinfrastructuur.nl/cpl/aanleverservice/1.2"),
                    Certificate = certificate
                };

                docBytes = wusDocument.CreateDocumentBytes(wusDocumentInfo);
            }
            return docBytes;
        }
    }
}