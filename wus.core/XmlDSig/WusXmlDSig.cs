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
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace CoreWUS
{
    internal sealed class WusXmlDSig : IWusXmlDSig
    {
        private string _tokenId;
        private string _tokenPrefix;
        private readonly ILogger _logger;

        public WusXmlDSig(ILogger logger)
        {
            _logger = logger;
            _logger?.Log(LogLevel.Debug, "Start");
            _tokenId = "sec_0";
            _tokenPrefix = "o";
            _logger?.Log(LogLevel.Debug, "End");
        }

        public void SetSecurityToken(string tokenId, string tokenPrefix)
        {
            _logger?.Log(LogLevel.Debug, "Start");
            _tokenId = tokenId;
            _tokenPrefix = tokenPrefix;
            _logger?.Log(LogLevel.Debug, "End");
        }

        public string CreateSignature(string xmlData, X509Certificate2 certificate, string[] referenceIds)
        {
            _logger?.Log(LogLevel.Debug, "Start");
            XmlDocument xmlDocument = new XmlDocument()
            {
                PreserveWhitespace = true
            };
            xmlDocument.LoadXml(xmlData);

            SignedXmlWithId signedXml = new SignedXmlWithId(xmlDocument)
            {
                SigningKey = certificate.GetRSAPrivateKey()
            };

            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

            XmlDsigExcC14NTransform transform = new XmlDsigExcC14NTransform();
            foreach (string id in referenceIds)
            {
                Reference reference = new Reference
                {
                    Uri = string.Concat("#", id),
                    DigestMethod = SignedXml.XmlDsigSHA1Url
                };
                reference.AddTransform(transform);
                signedXml.AddReference(reference);
            }

            KeyInfo keyInfo = new KeyInfo();
            KeyInfoSecurityToken keyInfoSecurityToken = new KeyInfoSecurityToken()
            {
                TokenId = _tokenId,
                Prefix = _tokenPrefix
            };
            keyInfo.AddClause(keyInfoSecurityToken);
            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();
            XmlElement xmlSignature = signedXml.GetXml();
            _logger?.Log(LogLevel.Debug, "End");
            return xmlSignature.OuterXml;
        }

        public bool VerifySignature(string xmlData)
        {
            _logger?.Log(LogLevel.Debug, "Start");
            XmlDocument xmlDocument = new XmlDocument()
            {
                PreserveWhitespace = true
            };
            xmlDocument.LoadXml(xmlData);
            XmlNode securityTokenNode = xmlDocument.SelectSingleNode("//*[local-name()='BinarySecurityToken']");
            if (securityTokenNode == null)
            {
                _logger?.Log(LogLevel.Error, "No BinarySecurityToken found");
                return false;
            }
            using (X509Certificate2 cert = new X509Certificate2(Convert.FromBase64String(securityTokenNode.InnerText)))
            {
                SignedXmlWithId signedXml = new SignedXmlWithId(xmlDocument);
                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Signature");
                signedXml.LoadXml((XmlElement)nodeList[0]);
                _logger?.Log(LogLevel.Debug, "End");
                return signedXml.CheckSignature(cert, true);
            }
        }
    }
}