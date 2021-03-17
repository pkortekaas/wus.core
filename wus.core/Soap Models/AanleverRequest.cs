namespace CoreWUS
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
    [System.Xml.Serialization.XmlRoot(Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
    public partial class aanleverRequest
    {
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=0)]
        public string kenmerk;
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=1)]
        public string berichtsoort;
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=2)]
        public string aanleverkenmerk;
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=3)]
        public string eerderAanleverkenmerk;
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public identiteitType identiteitBelanghebbende;
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=5)]
        public string rolBelanghebbende;
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public identiteitType identiteitOntvanger;
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=7)]
        public string rolOntvanger;
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public berichtInhoudType berichtInhoud;
        [System.Xml.Serialization.XmlArrayAttribute(Order=9)]
        [System.Xml.Serialization.XmlArrayItemAttribute("bijlage", IsNullable=false)]
        public berichtInhoudType[] berichtBijlagen;
        [System.Xml.Serialization.XmlElementAttribute(DataType="anyURI", Order=10)]
        public string autorisatieAdres;
    }
}
