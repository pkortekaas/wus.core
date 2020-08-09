using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

// https://stackoverflow.com/questions/42235677/httpclient-this-instance-has-already-started
namespace CoreWUS.Http
{
    public class HttpClientFactory
    {
        private static HttpClient _httpClient;

        public static HttpClient Create(Uri baseUri, X509Certificate clientCertificate, string serverThumbprint)
        {
            if (_httpClient == null)
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ClientCertificates.Add(clientCertificate);
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    bool valid = cert.Thumbprint == serverThumbprint;
                    Logger.Debug($"Validate server certificate: {cert.Thumbprint} {valid}");
                    return valid;
                };
                _httpClient = new HttpClient(handler);
                _httpClient.BaseAddress = baseUri;
                _httpClient.MaxResponseContentBufferSize = 64 * 1024;
            }
            else
            {
                throw new TypeLoadException("HttpClient already created");
            }
            return _httpClient;
        }

    }
}