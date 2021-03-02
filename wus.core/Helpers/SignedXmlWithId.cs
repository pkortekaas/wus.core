using System.Security.Cryptography.Xml;
using System.Xml;

namespace CoreWUS
{
    internal class SignedXmlWithId : SignedXml
    {
        private XmlNamespaceManager _nsManager;

        public SignedXmlWithId()
            : base()
        {
        }

        public SignedXmlWithId(XmlDocument xml)
            : base(xml)
        {
        }

        public SignedXmlWithId(XmlElement xmlElement)
            : base(xmlElement)
        {
        }

        public override XmlElement GetIdElement(XmlDocument doc, string id)
        {
            XmlElement idElem = base.GetIdElement(doc, id);

            if (idElem == null)
            {
                if (_nsManager == null)
                {
                    _nsManager = new XmlNamespaceManager(doc.NameTable);
                    _nsManager.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
                }
                idElem = doc.SelectSingleNode("//*[@wsu:Id=\"" + id + "\"]", _nsManager) as XmlElement;
            }

            return idElem;
        }
    }
}