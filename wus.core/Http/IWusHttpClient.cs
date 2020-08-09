using System;

public interface IWusHttpClient
{
    string Post(Uri url, string soapAction, byte[] data);
}
