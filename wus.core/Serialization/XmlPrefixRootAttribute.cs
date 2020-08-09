using System;
using System.Xml.Serialization;

namespace CoreWUS.Serialization
{
    [System.AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.ReturnValue)]
    public class XmlPrefixRootAttribute: XmlRootAttribute
    {
        public string Prefix { get; set; }

        public XmlPrefixRootAttribute() : base() {}
        public XmlPrefixRootAttribute(string elementName) : base(elementName) {}
    }
}
