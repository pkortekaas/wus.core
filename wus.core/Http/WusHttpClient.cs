using System;
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
        private ILogger _logger;

        public WusHttpClient(Uri baseUri, X509Certificate2 clientCertificate, string serverThumbprint, ILogger logger)
        {
            _logger = logger;
            _client = HttpClientFactory.Create(baseUri, clientCertificate, serverThumbprint, logger);
        }

        public string Post(Uri url, string soapAction, byte[] data)
        {
            _logger?.Log(LogLevel.Verbose, "Start");
            HttpResponseMessage response = PostAsync(url, soapAction, data).Result;
            if (response.IsSuccessStatusCode)
            {
                _logger?.Log(LogLevel.Verbose, "End");
                return response.Content.ReadAsStringAsync().Result;
            }
            throw new HttpException(response.StatusCode);
        }

        private async Task<HttpResponseMessage> PostAsync(Uri url, string soapAction, byte[] data)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = new ByteArrayContent(data);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                request.Content.Headers.Add("SOAPAction", soapAction);
                return await _client.SendAsync(request).ConfigureAwait(false);
            }
        }
    }
}