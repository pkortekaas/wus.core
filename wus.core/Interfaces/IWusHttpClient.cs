using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CoreWUS
{
    public interface IWusHttpClient
    {
        string Post(Uri url, string soapAction, byte[] data);
        Task<string> PostAsync(Uri url, string soapAction, byte[] data);
        X509Certificate2 ClientCertificate { get; }
    }
}
