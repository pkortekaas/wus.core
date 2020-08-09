namespace CoreWUS
{
    [System.Xml.Serialization.XmlRoot(Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
    public partial class getNieuweStatussenProcesResponse
    {
        //[System.Xml.Serialization.XmlArrayAttribute(ElementName="getNieuweStatussenProcesReturn")]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public StatusResultaat[] getNieuweStatussenProcesReturn;
    }

    [System.Xml.Serialization.XmlRoot(Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
    public partial class getStatussenProcesResponse
    {
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public StatusResultaat[] getStatussenProcesReturn;
    }
}
