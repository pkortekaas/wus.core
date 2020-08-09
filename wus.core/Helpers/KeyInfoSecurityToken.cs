using System;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace CoreWUS
{
    internal class KeyInfoSecurityToken : KeyInfoClause
    {
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
            string mask = @"<{0}:SecurityTokenReference xmlns:{0}=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
        <{0}:Reference URI=""#{1}"" ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3""/>
    </{0}:SecurityTokenReference>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(string.Format(mask, Prefix, TokenId));
            return doc.DocumentElement;
        }
        public override void LoadXml(XmlElement value)
        {
            throw new NotImplementedException();
        }
    }
}