using System;
using System.Security.Cryptography.X509Certificates;

namespace CoreWUS
{
    public interface IWusHttpClient
    {
        string Post(Uri url, string soapAction, byte[] data);
        X509Certificate2 ClientCertificate { get; }
    }
}
