using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

// https://stackoverflow.com/questions/42235677/httpclient-this-instance-has-already-started
// https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
namespace CoreWUS.Http
{
    public sealed class HttpClientFactory
    {
        private static HttpClient _httpClient;

        [SuppressMessage("Microsoft.CodeAnalysis.FxCopAnalyzers", "CA2000:DisposeObjectsBeforeLosingScope")]
        public static HttpClient Create(Uri baseUri, X509Certificate clientCertificate, string serverThumbprint, ILogger logger)
        {
            if (_httpClient == null)
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ClientCertificates.Add(clientCertificate);
                handler.ServerCertificateCustomValidationCallback  = (message, cert, chain, errors) =>
                {
                    bool valid = cert.Thumbprint == serverThumbprint;
                    logger.Log(valid ? LogLevel.Debug : LogLevel.Error, $"Validate server certificate: {cert.Thumbprint} {valid}");
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