using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;

// https://stackoverflow.com/questions/42235677/httpclient-this-instance-has-already-started
// https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
namespace CoreWUS.Http
{
    public sealed class HttpClientFactory
    {
        private static volatile Dictionary<string, HttpClient> _clientMap = new Dictionary<string, HttpClient>();
        private static readonly object _locker = new object();

        [SuppressMessage("Microsoft.CodeAnalysis.FxCopAnalyzers", "CA2000:DisposeObjectsBeforeLosingScope")]
        public static HttpClient Create(Uri baseUri, X509Certificate clientCertificate, string serverThumbprint, ILogger logger)
        {
            Utils.CheckNullArgument(baseUri, "baseUri");
            string key = baseUri.AbsoluteUri;
            if (!_clientMap.ContainsKey(key))
            {
                lock (_locker)
                {
                    if (!_clientMap.ContainsKey(key))
                    {
                        HttpClientHandler handler = new HttpClientHandler();
                        handler.ClientCertificates.Add(clientCertificate);
                        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                        {
                            bool valid = cert.Thumbprint.Replace(":", "").Equals(serverThumbprint?.Replace(":", ""), StringComparison.InvariantCultureIgnoreCase);
                            logger?.Log(valid ? LogLevel.Debug : LogLevel.Error, $"Validate server certificate: {cert.Thumbprint} {valid}");
                            return valid;
                        };
                        _clientMap.Add(key, new HttpClient(handler)
                        {
                            BaseAddress = baseUri,
                            MaxResponseContentBufferSize = 64 * 1024
                        });
                    }
                }
            }
            return _clientMap[key];
        }

    }
}