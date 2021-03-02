/*
MIT License

Copyright (c) 2020 Paul Kortekaas

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CoreWUS.Serialization;
using CoreWUS.Extensions;

namespace CoreWUS
{
    public sealed class WusProcessor
    {
        private readonly XmlWriterSettings _xmlWriterSettings;
        private readonly IWusHttpClient _httpClient;
        private readonly IWusXmlDSig _xmlDSig;
        private readonly X509Certificate2 _signingCertificate;
        private readonly ILogger _logger;

        private const string _deliverAction = "http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/AanleverService/aanleverenRequest";
        private const string _newStatusAction = "http://logius.nl/digipoort/wus/2.0/statusinformatieservice/1.2/StatusinformatieService/getNieuweStatussenProcesRequest";
        private const string _allStatusAction = "http://logius.nl/digipoort/wus/2.0/statusinformatieservice/1.2/StatusinformatieService/getStatussenProcesRequest";

        public WusProcessor(IWusHttpClient client, ILogger logger, X509Certificate2 signingCertificate)
        {
            _logger = logger;
            _logger?.Log(LogLevel.Verbose, "Start");
            _xmlWriterSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = true
            };
            _xmlDSig = new WusXmlDSig(logger);
            _signingCertificate = signingCertificate;
            _httpClient = client;
            _logger?.Log(LogLevel.Verbose, "End");
        }

        public aanleverResponse Deliver(aanleverRequest request, Uri url)
        {
            _logger?.Log(LogLevel.Verbose, "Passthrough");
            return Post<aanleverResponse>(request.ToXElement(_xmlWriterSettings), url, _deliverAction);
        }

        public IEnumerable<StatusResultaat> NewStatusProcess(getNieuweStatussenProcesRequest request, Uri url)
        {
            _logger?.Log(LogLevel.Verbose, "Passthrough");
            return Post<getNieuweStatussenProcesResponse>(request.ToXElement(_xmlWriterSettings), url, _newStatusAction).
                        getNieuweStatussenProcesReturn;
        }

        public IEnumerable<StatusResultaat> AllStatusProcess(getStatussenProcesRequest request, Uri url)
        {
            _logger?.Log(LogLevel.Verbose, "Passthrough");
            return Post<getStatussenProcesResponse>(request.ToXElement(_xmlWriterSettings), url, _allStatusAction).
                        getStatussenProcesReturn;
        }

        private T Post<T>(XElement body, Uri url, string soapAction) where T: class
        {
            _logger?.Log(LogLevel.Verbose, "Start");
            try
            {
                byte[] docBytes = new WusDocument(_xmlDSig, _logger)
                    .WithEnvelope(body)
                    .WithAddressing(soapAction, url)
                    .WithSignature(_signingCertificate)
                    .CreateDocumentBytes();

                if (docBytes != null)
                {
                    string response = _httpClient.Post(url, soapAction, docBytes);
                    return HandleResponse<T>(response);
                }
            }
            finally
            {
                _logger?.Log(LogLevel.Verbose, "End");
            }
            return default;
        }

        private T HandleResponse<T>(string response) where T: class
        {
            _logger?.Log(LogLevel.Verbose, "Start");
            XDocument xDoc = XDocument.Parse(response);
            XElement body = xDoc.Root.Elements().FirstOrDefault(e => e.Name.LocalName == "Body");
            try
            {
                if (body != null)
                {
                    XElement x;
                    XElement fault = body.Elements().FirstOrDefault(e => e.Name.LocalName == "Fault");
                    if (fault != null)
                    {
                        string code = fault.Element("faultcode")?.Value;
                        string actor = fault.Element("faultactor")?.Value;
                        string msg = fault.Element("faultstring")?.Value;

                        x = fault.Descendants().FirstOrDefault(e => e.Name.LocalName == "aanleverFault" ||
                                                                    e.Name.LocalName == "statusinformatieFault");
                        if (x != null)
                        {
                            XmlRootAttribute rootAttribute = new XmlRootAttribute()
                            {
                                ElementName = x.Name.LocalName,
                                Namespace = x.Name.NamespaceName
                            };
                            foutType ft = Serializer.Deserialize<foutType>(x.CreateReader(), rootAttribute);
                            throw new WusException(msg, code, actor, ft.foutcode, ft.foutbeschrijving);
                        }

                        throw new SoapException(msg, code, actor);
                    }

                    if (!_xmlDSig.VerifyXmlDSig(response))
                    {
                        throw new VerificationException("XmlDSig signature verification failed.");
                    }

                    x = body.Elements().FirstOrDefault(e => e.Name.LocalName == typeof(T).Name);
                    if (x != null)
                    {
                        return Serializer.Deserialize<T>(x.CreateReader());
                    }
                }
                else
                {
                    _logger?.Log(LogLevel.Warning, "No Body element in response");
                }
            }
            finally
            {
                _logger?.Log(LogLevel.Verbose, "End");
            }
            return default;
        }

    }
}