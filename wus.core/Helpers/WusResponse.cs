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
using System.Linq;
using System.Security;
using System.Xml.Linq;
using System.Xml.Serialization;
using CoreWUS.Serialization;

namespace CoreWUS
{
    internal sealed class WusResponse : IWusResponse
    {
        private readonly ILogger _logger;
        private readonly IWusXmlDSig _xmlDSig;

        public WusResponse(IWusXmlDSig xmlDSig, ILogger logger)
        {
            _xmlDSig = xmlDSig;
            _logger = logger;
        }

        public T HandleResponse<T>(string response) where T : class
        {
            _logger?.Log(LogLevel.Debug, "Start");
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

                    if (!_xmlDSig.VerifySignature(response))
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
                _logger?.Log(LogLevel.Debug, "End");
            }
            return default;
        }
    }
}