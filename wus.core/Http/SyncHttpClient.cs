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
    public class SyncHttpClient : IWusHttpClient
    {
        Uri _baseUri;
        X509Certificate2 _clientCertificate;
        string _serverThumbprint;

        public SyncHttpClient(Uri baseUri, X509Certificate2 clientCertificate, string serverThumbprint)
        {
            _baseUri = baseUri;
            _clientCertificate = clientCertificate;
            _serverThumbprint = serverThumbprint;
        }

        public string Post(Uri url, string soapAction, byte[] data)
        {
            Logger.Verbose("Start");
            string path = new Uri(_baseUri, url).AbsoluteUri;
            HttpWebRequest request = CreateRequest(HttpMethod.Post, path);

            request.ContentType = "text/xml";
            request.ContentLength = data.Length;
            request.Headers.Add("SOAPAction", soapAction);

            string result = ExecutePostRequest(request, data);
            Logger.Verbose("End");
            return result;
        }

        private string ExecutePostRequest(HttpWebRequest request, byte[] body)
        {
            string data = null;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(body, 0, body.Length);
                requestStream.Close();
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
            request.Method = method.ToString().ToUpper();
            request.ClientCertificates.Add(_clientCertificate);
            request.ServerCertificateValidationCallback = (message, cert, chain, errors) =>
            {
                X509Certificate2 cert2 = cert as X509Certificate2;
                bool valid = cert2.Thumbprint == _serverThumbprint;
                Logger.Debug($"Validate server certificate: {cert2.Thumbprint} {valid}");
                return valid;
            };

            return request;
        }
    }
}