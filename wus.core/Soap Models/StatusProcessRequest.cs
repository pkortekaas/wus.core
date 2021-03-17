namespace CoreWUS
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
    [System.Xml.Serialization.XmlRoot(Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
    public partial class getNieuweStatussenProcesRequest
    {
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=0)]
        public string kenmerk;
        [System.Xml.Serialization.XmlElementAttribute(DataType="anyURI", Order=1)]
        public string autorisatieAdres;
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public System.DateTime tijdstempelVanaf;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tijdstempelVanafSpecified;
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public System.DateTime tijdstempelTot;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tijdstempelTotSpecified;
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
    [System.Xml.Serialization.XmlRoot(Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
    public partial class getStatussenProcesRequest
    {
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=0)]
        public string kenmerk;
        [System.Xml.Serialization.XmlElementAttribute(DataType="anyURI", Order=1)]
        public string autorisatieAdres;
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public System.DateTime tijdstempelVanaf;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tijdstempelVanafSpecified;
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public System.DateTime tijdstempelTot;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tijdstempelTotSpecified;
    }

}