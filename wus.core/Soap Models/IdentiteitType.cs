namespace CoreWUS
{
	[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
	public class identiteitType
	{
	    [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=0)]
	    public string nummer;
	    [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=1)]
	    public string type;
	}
}