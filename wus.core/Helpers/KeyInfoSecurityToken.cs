using System;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace CoreWUS
{
    internal class KeyInfoSecurityToken : KeyInfoClause
    {
        private static readonly string _schema = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        private static readonly string _valueType = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3";

        public string TokenId { get; set; }
        public string Prefix { get; set; }

        public KeyInfoSecurityToken()
        {
        }

        public KeyInfoSecurityToken(string tokenId, string prefix)
        {
            TokenId = tokenId;
            Prefix = prefix;
        }

        public override XmlElement GetXml()
        {
            string xml = $@"<{Prefix}:SecurityTokenReference xmlns:{Prefix}=""{_schema}"">
                <{Prefix}:Reference URI=""#{TokenId}"" ValueType=""{_valueType}""/>
            </{Prefix}:SecurityTokenReference>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement;
        }

        public override void LoadXml(XmlElement value)
        {
            throw new NotImplementedException();
        }
    }
}