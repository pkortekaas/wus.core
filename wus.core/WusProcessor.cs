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
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CoreWUS.Extensions;

namespace CoreWUS
{
    public sealed class WusProcessor
    {
        private readonly XmlWriterSettings _xmlWriterSettings;
        private readonly IWusHttpClient _httpClient;
        private readonly IWusXmlDSig _xmlDSig;
        private readonly IWusResponse _wusResponse;
        private readonly IWusDocument _wusDocument;
        private readonly X509Certificate2 _signingCertificate;
        private readonly ILogger _logger;

        private const string _deliverAction = "http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/AanleverService/aanleverenRequest";
        private const string _newStatusAction = "http://logius.nl/digipoort/wus/2.0/statusinformatieservice/1.2/StatusinformatieService/getNieuweStatussenProcesRequest";
        private const string _allStatusAction = "http://logius.nl/digipoort/wus/2.0/statusinformatieservice/1.2/StatusinformatieService/getStatussenProcesRequest";

        [SuppressMessage("Microsoft.CodeAnalysis.FxCopAnalyzers", "CA1713:EventsPrefix")]
        public event EventHandler<string> BeforeSend;
        [SuppressMessage("Microsoft.CodeAnalysis.FxCopAnalyzers", "CA1713:EventsPrefix")]
        public event EventHandler<string> AfterResponse;

        public WusProcessor(IWusHttpClient client, ILogger logger, X509Certificate2 signingCertificate)
        {
            _logger = logger;
            _logger?.Log(LogLevel.Debug, "Start");

            _xmlWriterSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = true
            };
            _xmlDSig = new WusXmlDSig(logger);
            _wusResponse = new WusResponse(_xmlDSig, logger);
            _wusDocument = new WusDocument(_xmlDSig, logger);
            _signingCertificate = signingCertificate;
            _httpClient = client;

            _logger?.Log(LogLevel.Debug, "End");
        }

        public aanleverResponse Deliver(aanleverRequest request, Uri uri)
        {
            _logger?.Log(LogLevel.Debug, "Passthrough");
            Utils.CheckNullArgument(request, "request");
            Utils.CheckNullArgument(uri, "uri");
            return Post<aanleverResponse>(request.ToXElement(_xmlWriterSettings), uri, _deliverAction);
        }

        public IEnumerable<StatusResultaat> NewStatusProcess(getNieuweStatussenProcesRequest request, Uri uri)
        {
            _logger?.Log(LogLevel.Debug, "Passthrough");
            Utils.CheckNullArgument(request, "request");
            Utils.CheckNullArgument(uri, "uri");
            return Post<getNieuweStatussenProcesResponse>(request.ToXElement(_xmlWriterSettings), uri, _newStatusAction).
                        getNieuweStatussenProcesReturn;
        }

        public IEnumerable<StatusResultaat> AllStatusProcess(getStatussenProcesRequest request, Uri uri)
        {
            _logger?.Log(LogLevel.Debug, "Passthrough");
            Utils.CheckNullArgument(request, "request");
            Utils.CheckNullArgument(uri, "uri");
            return Post<getStatussenProcesResponse>(request.ToXElement(_xmlWriterSettings), uri, _allStatusAction).
                        getStatussenProcesReturn;
        }

        private T Post<T>(XElement body, Uri uri, string soapAction) where T: class
        {
            _logger?.Log(LogLevel.Debug, "Start");

            WusDocumentInfo wusDocumentInfo = new WusDocumentInfo()
            {
                Envelope = body,
                SoapAction = soapAction,
                Uri = uri,
                Certificate = _signingCertificate
            };

            byte[] docBytes = _wusDocument.CreateDocumentBytes(wusDocumentInfo);

            if (docBytes != null)
            {
                BeforeSend?.Invoke(this, Encoding.UTF8.GetString(docBytes));
                string response = _httpClient.Post(uri, soapAction, docBytes);
                AfterResponse?.Invoke(this, response);
                return _wusResponse.HandleResponse<T>(response);
            }
            _logger?.Log(LogLevel.Debug, "End");

            return default;
        }
    }
}