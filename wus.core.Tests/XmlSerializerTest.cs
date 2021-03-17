using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CoreWUS;
using CoreWUS.Extensions;
using Xunit;

namespace wus.core.Tests
{
    public class XmlSerializerTests : BaseTests
    {
        [Fact]
        public void XmlSerializer_ToXml_Pass()
        {
            // Given
            aanleverRequest request = AanleverRequest;
            XmlWriterSettings xmlWriterSettings = CreateXmlWriterSettings();

            // When
            string actual = request.ToXml(xmlWriterSettings);

            // Then
            Assert.True(actual?.Length > 0);
        }

        [Fact]
        public void XmlSerializer_ToXElementHasOnlyDefaultNamespace_Pass()
        {
            // Given
            aanleverRequest request = AanleverRequest;
            XmlWriterSettings xmlWriterSettings = CreateXmlWriterSettings();
            string expected = "http://logius.nl/digipoort/koppelvlakservices/1.2/";

            // When
            XElement element = request.ToXElement(xmlWriterSettings);
            int nsCount = element.Attributes().Count();
            string actual = element.Attribute("xmlns").Value;

            // Then
            Assert.True(nsCount == 1, "Verify single attribute");
            Assert.Equal(expected, actual);
        }

        private XmlWriterSettings CreateXmlWriterSettings()
        {
            return new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = true
            };
        }
    }
}