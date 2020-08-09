namespace CoreWUS
{
    [System.Xml.Serialization.XmlRoot(Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
    public class aanleverResponse
    {
        public string kenmerk;
        public string berichtsoort;
        public string aanleverkenmerk;
        public string eerderAanleverkenmerk;
        public System.DateTime tijdstempelAangeleverd;
        public identiteitType identiteitBelanghebbende;
        public string rolBelanghebbende;
        public identiteitType identiteitOntvanger;
        public string rolOntvanger;
        public string autorisatieAdres;
        public string statuscode;
        public System.DateTime tijdstempelStatus;
        public string statusomschrijving;
        public foutType statusFoutcode;
        public string statusdetails;
    }
}