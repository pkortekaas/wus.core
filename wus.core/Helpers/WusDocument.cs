/*
MIT License

Copyright (c) 2020 Paul Kortekaas

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace CoreWUS
{
    internal class WusDocument
    {
        private readonly XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";
        private readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        private readonly XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
        private readonly XNamespace u = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
        private readonly XNamespace o ="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        private readonly XNamespace a = "http://www.w3.org/2005/08/addressing";

        private const string _bodyId = "body_0";
        private const string _timestampId = "timestamp_0";
        private const string _address0Id = "address_0";
        private const string _address1Id = "address_1";
        private const string _address2Id = "address_2";
        private const string _address3Id = "address_3";
        private const string _tokenId = "sec_0";
        private const string _tokenPrefix = "o";

        private const string _tokenValueType = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3";
        private const string _tokenEncodingType ="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary";

        private readonly IWusXmlDSig _xmlDSig;
        private readonly ILogger _logger;

        internal XDocument Envelope { get; private set; }

        private XElement _bodyElement;
        private string _action;
        private Uri _url;
        private X509Certificate2 _signingCertificate;

        public WusDocument(IWusXmlDSig xmlDSig, ILogger logger)
        {
            _logger = logger;
            _logger?.Log(LogLevel.Verbose, "Start");
            Envelope = null;
            _xmlDSig = xmlDSig;
            _xmlDSig.SetSecurityToken(_tokenId, _tokenPrefix);
            _logger?.Log(LogLevel.Verbose, "End");
        }

        public byte[] CreateDocumentBytes()
        {
            _logger?.Log(LogLevel.Verbose, "Start");
            try
            {
                if (CreateEnvelope(_bodyElement))
                {
                    if (AddAddressing(_action, _url))
                    {
                        if (AddSignature(_signingCertificate))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                Envelope.Save(ms, SaveOptions.DisableFormatting);
                                return ms.ToArray();
                            }
                        }
                    }
                }
            }
            finally
            {
                _logger?.Log(LogLevel.Verbose, "End");
            }
            return null;
        }

        public WusDocument WithEnvelope(XElement bodyElement)
        {
            _logger?.Log(LogLevel.Verbose, "Passthrough");
            _bodyElement = bodyElement;
            return this;
        }

        public WusDocument WithAddressing(string action, Uri url)
        {
            _logger?.Log(LogLevel.Verbose, "Passthrough");
            _action = action;
            _url = url;
            return this;
        }

        public WusDocument WithSignature(X509Certificate2 certificate)
        {
            _logger?.Log(LogLevel.Verbose, "Passthrough");
            _signingCertificate = certificate;
            return this;
        }

        private bool CreateEnvelope(XElement bodyElement)
        {
            _logger?.Log(LogLevel.Verbose, "Start");

            Envelope = new XDocument
            (
                new XDeclaration("1.0", "utf-8", "true"),
                new XElement(s + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "s", s),
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                    new XAttribute(XNamespace.Xmlns + "u", u),
                    new XElement(s + "Header"),
                    new XElement(s + "Body",
                        new XAttribute( u + "Id", _bodyId),
                        bodyElement
                    )
                )
            );

            _logger?.Log(LogLevel.Verbose, "End");
            return true;
        }

        private bool AddAddressing(string action, Uri url)
        {
            _logger?.Log(LogLevel.Verbose, "Start");
            bool result = false;

            XElement header = Envelope.Root.Elements().FirstOrDefault(e => e.Name.LocalName == "Header");
            if (header != null)
            {
                Envelope.Root.Add(new XAttribute(XNamespace.Xmlns + "a", a));
                header.Add(new XElement(a + "Action",
                                new XAttribute(s + "mustUnderstand", "1"),
                                new XAttribute(u + "Id", _address0Id),
                                action
                            ),
                            new XElement( a + "MessageID",
                                new XAttribute(u + "Id", _address1Id),
                                $"urn:uuid:{Guid.NewGuid():D}"
                            ),
                            new XElement( a + "ReplyTo",
                                new XAttribute(u + "Id", _address2Id),
                                new XElement( a + "Address", a.NamespaceName + "/anonymous")
                            ),
                            new XElement( a + "To",
                                new XAttribute(u + "Id", _address3Id),
                                url.OriginalString
                            )
                );
                result = true;
            }

            _logger?.Log(LogLevel.Verbose, "End");
            return result;
        }

        private bool AddSecurity(string token)
        {
            _logger?.Log(LogLevel.Verbose, "Start");
            bool result = false;

            XElement header = Envelope.Root.Elements().FirstOrDefault(e => e.Name.LocalName == "Header");
            if (header != null)
            {
                Envelope.Root.Add(new XAttribute(XNamespace.Xmlns + "o", o));
                DateTime now = DateTime.UtcNow;
                header.Add(new XElement(o + "Security",
                                new XAttribute(s + "mustUnderstand", "1"),
                                new XElement(u + "Timestamp",
                                    new XAttribute(u + "Id", _timestampId),
                                    new XElement( u + "Created", now.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)),
                                    new XElement( u + "Expires", now.AddMinutes(5).ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture))),
                                new XElement(o + "BinarySecurityToken",
                                    new XAttribute(u + "Id", _tokenId),
                                    new XAttribute("ValueType", _tokenValueType),
                                    new XAttribute("EncodingType", _tokenEncodingType),
                                    token)

                            )
                );
                result = true;
            }

            _logger?.Log(LogLevel.Verbose, "End");
            return result;
        }

        private bool AddSignature(X509Certificate2 certificate)
        {
            _logger?.Log(LogLevel.Verbose, "Start");
            bool result = AddSecurity(Convert.ToBase64String(certificate.RawData));

            XElement security = Envelope.Root.Descendants().FirstOrDefault(e => e.Name.LocalName == "Security");
            if (security != null)
            {
                string[] referenceIds = new string[] { _bodyId, _timestampId, _address0Id, _address1Id, _address2Id, _address3Id };
                string xmldsig = _xmlDSig.SignXmlDSig(Envelope.ToString(SaveOptions.DisableFormatting), certificate, referenceIds);
                XElement signature = XElement.Parse(xmldsig);
                security.Add(signature);
                result = true;
            }

            _logger?.Log(LogLevel.Verbose, "End");
            return result;
        }
    }
}