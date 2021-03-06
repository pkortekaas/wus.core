namespace CoreWUS
{
    internal interface IWusDocument
    {
        byte[] CreateDocumentBytes(WusDocumentInfo wusDocumentInfo);
    }
}