using System;
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
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);

            // When
            byte[] docBytes = GetCreatedDocumentBytes(xmlDSig);

            // Then
            Assert.True(docBytes != null, "Cannot create document");
        }

        [Fact]
        public void CreateDocumentBytes_CreateAndVerifySignature_Pass()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);

            // When
            byte[] docBytes = GetCreatedDocumentBytes(xmlDSig);

            bool actual = false;
            if (docBytes != null)
            {
                string xmlData = GetStringWithoutBOM(docBytes);
                actual = xmlDSig.VerifySignature(xmlData);
            }

            // Then
            Assert.True(actual, "Signature of created document cannot be verified");
        }

        private byte[] GetCreatedDocumentBytes(IWusXmlDSig xmlDSig)
        {
            ILogger logger = null;
            IWusDocument wusDocument = new WusDocument(xmlDSig, logger);

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = true
            };

            aanleverRequest request = AanleverRequest;

            byte[] docBytes = null;
            WithX509Certificate( cert =>
            {
                WusDocumentInfo wusDocumentInfo = new WusDocumentInfo()
                {
                    Envelope = request.ToXElement(xmlWriterSettings),
                    SoapAction = "http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/AanleverService/aanleverenRequest",
                    Uri = new Uri("https://cs-bedrijven.procesinfrastructuur.nl/cpl/aanleverservice/1.2"),
                    Certificate = cert
                };

                docBytes = wusDocument.CreateDocumentBytes(wusDocumentInfo);
            });

            return docBytes;
        }
    }
}