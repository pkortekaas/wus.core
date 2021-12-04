using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

// https://stackoverflow.com/questions/51478525/httpclient-this-instance-has-already-started-one-or-more-requests-properties-ca

namespace CoreWUS.Http
{
    public sealed class WusHttpClient : IWusHttpClient
    {
        private readonly HttpClient _client;
        private readonly X509Certificate2 _clientCertificate;
        private ILogger _logger;

        public WusHttpClient(Uri baseUri, X509Certificate2 clientCertificate, string serverThumbprint, ILogger logger)
        {
            _logger = logger;
            _clientCertificate = clientCertificate;
            _client = HttpClientFactory.Create(baseUri, clientCertificate, serverThumbprint, logger);
        }

        public X509Certificate2 ClientCertificate => _clientCertificate;

        public string Post(Uri url, string soapAction, byte[] data) => PostAsync(url, soapAction, data).Result;

        [SuppressMessage("Microsoft.CodeAnalysis.FxCopAnalyzers", "CA2007:ConfigureAwait")]
        public async Task<string> PostAsync(Uri url, string soapAction, byte[] data)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = new ByteArrayContent(data);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                request.Content.Headers.Add("SOAPAction", soapAction);
                using (HttpResponseMessage response = await _client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        _logger?.Log(LogLevel.Debug, "End");
                        return await response.Content.ReadAsStringAsync();
                    }
                    throw new HttpException(response.StatusCode);
                }
            }
        }
    }
}