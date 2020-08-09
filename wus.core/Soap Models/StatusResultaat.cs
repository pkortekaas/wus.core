namespace CoreWUS
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
    public partial class StatusResultaat
    {
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=0)]
        public string kenmerk;
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public identiteitType identiteitBelanghebbende;
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=2)]
        public string statuscode;
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public System.DateTime tijdstempelStatus;
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=4)]
        public string statusomschrijving;
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public foutType statusFoutcode;
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=6)]
        public string statusdetails;
    }
}