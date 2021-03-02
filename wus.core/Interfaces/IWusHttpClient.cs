using System;

namespace CoreWUS
{
    public interface IWusHttpClient
    {
        string Post(Uri url, string soapAction, byte[] data);
    }
}
