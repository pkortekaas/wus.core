namespace CoreWUS
{
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://logius.nl/digipoort/koppelvlakservices/1.2/")]
	public partial class berichtInhoudType
	{
	    [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=0)]
	    public string mimeType;
	    [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=1)]
	    public string bestandsnaam;
	    [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", Order=2)]
	    public byte[] inhoud;
	}
}