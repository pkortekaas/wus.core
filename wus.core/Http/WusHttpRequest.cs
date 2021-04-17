using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CoreWUS.Extensions;

// https://github.com/pierodetomi/dotnet-webutils/tree/master/DotNet.WebUtils
namespace CoreWUS
{
    public sealed class WusHttpRequest : IWusHttpClient
    {
        private readonly Uri _baseUri;
        private readonly X509Certificate2 _clientCertificate;
        private readonly string _serverThumbprint;
        private ILogger _logger;

        public X509Certificate2 ClientCertificate => _clientCertificate;

        public WusHttpRequest(Uri baseUri, X509Certificate2 clientCertificate, string serverThumbprint, ILogger logger)
        {
            _baseUri = baseUri;
            _clientCertificate = clientCertificate;
            _serverThumbprint = serverThumbprint?.Replace(":", "");
            _logger = logger;
        }

        public string Post(Uri url, string soapAction, byte[] data)
        {
            _logger?.Log(LogLevel.Debug, "Start");
            Utils.CheckNullArgument(url, "url");
            Utils.CheckNullArgument(data, "data");

            string path = new Uri(_baseUri, url).AbsoluteUri;
            HttpWebRequest request = CreateRequest(HttpMethod.Post, path);

            request.ContentType = "text/xml";
            request.ContentLength = data.Length;
            request.Headers.Add("SOAPAction", soapAction);

            string result = ExecutePostRequest(request, data);
            _logger?.Log(LogLevel.Debug, "End");
            return result;
        }

        private static string ExecutePostRequest(HttpWebRequest request, byte[] body)
        {
            string data = null;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(body, 0, body.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.IsSuccessStatusCode())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            data = sr.ReadToEnd();
                        }
                    }
                    request = null;
                }
                else
                {
                    throw new HttpException(response.StatusCode);
                }
            }

            return data;
        }

        private HttpWebRequest CreateRequest(HttpMethod method, string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = method.ToString().ToUpperInvariant();
            request.ClientCertificates.Add(_clientCertificate);
            request.ServerCertificateValidationCallback = (message, cert, chain, errors) =>
            {
                X509Certificate2 cert2 = cert as X509Certificate2;
                bool valid = cert2.Thumbprint.Replace(":", "").Equals(_serverThumbprint, StringComparison.InvariantCultureIgnoreCase);
                _logger?.Log(valid ? LogLevel.Debug : LogLevel.Error, $"Validate server certificate: {cert2.Thumbprint} {valid}");
                return valid;
            };

            return request;
        }
    }
}