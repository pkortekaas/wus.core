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
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CoreWUS;
using CoreWUS.Http;

namespace WusClient
{
    class Program
    {
        // run from solution folder: dotnet run -p WusClient [-- -r <reference number>]
        static void Main(string[] args)
        {
            string[] scenarios = new string[] { "Happyflow", "AanleverFault", "StatusInformatieFault",
                                                "Afleverfout", "Foutuitvragendepartij",
                                                "TechnischProbleemAfleveren", "MaximaalAantalAfleverpogingen",
                                                "ZwaarbelastAanleveren", "OverbelastAanleveren",
                                                "ZwaarbelastStatusInformatie", "OverbelastStatusInformatie"};
            // Change for your own certificate, or use filename/password
            string myPKIoThumbprint = Environment.GetEnvironmentVariable("PKIO_THUMB");
            string path = Path.GetFileName(Directory.GetCurrentDirectory());

            string reference = null;
            string instanceFile = "./xbrl/VB-01_bd-rpt-ob-aangifte-2021.xbrl";
            string noAusp = "http://geenausp.nl";
            string scenario = scenarios[0];
            bool preprod = false;

            string baseUrl = "https://cs-bedrijven.procesinfrastructuur.nl/cpl/";
            string deliveryUrl = "aanleverservice/1.2";
            string statusUrl = "statusinformatieservice/1.2";
            string serverCertificateThumbprint = "03035D253D775796F68B7C2E22C15B16DE0C9954";

            if (preprod)
            {
                baseUrl = "https://preprod-dgp2.procesinfrastructuur.nl/wus/2.0/";
                serverCertificateThumbprint = "C8B724E50EAF26FF01C59A6D9694FCB0DCBB3E4F";
            }

            // quirk to adjust instance name when
            // running in debugger...
            // Don't want to use an absolute path
            if (path == "WusClient")
            {
                instanceFile = "." + instanceFile;
            }
            if (!File.Exists(instanceFile))
            {
                Console.WriteLine($"Unable to find the sample instance document: {instanceFile}");
                Environment.Exit(-1);
            }

            // allow command line argument -r <reference number>
            if (args.Length == 2 && args[0] == "-r")
            {
                reference = args[1];
            }

            aanleverRequest request = new aanleverRequest()
            {
                berichtsoort = "Omzetbelasting",
                aanleverkenmerk = preprod ? Guid.NewGuid().ToString("D") : scenario,
                autorisatieAdres = noAusp,
                identiteitBelanghebbende = new identiteitType()
                {
                    nummer = "001000044B37",
                    type = "Fi"
                },
                rolBelanghebbende = "Bedrijf",
                berichtInhoud = new berichtInhoudType()
                {
                    mimeType = "text/xml",
                    bestandsnaam = "Omzetbelasting.xbrl",
                    inhoud = File.ReadAllBytes(instanceFile)
                }
            };

            //using (X509Certificate2 cert = new X509Certificate2("filename", "password"))
            using (X509Certificate2 cert = FindCertificate(myPKIoThumbprint))
            {
                ILogger logger = new Logger(LogLevel.Debug);

                try
                {
                    logger.Log(LogLevel.Info, "Startup");
                    IWusHttpClient wusHttpClient = new WusHttpClient(new Uri(baseUrl), cert, serverCertificateThumbprint, logger);
                    WusProcessor wp = new WusProcessor(wusHttpClient, logger);
                    wp.BeforeSend += (object sender, string data) =>
                    {
                        logger.Log(LogLevel.Verbose, $"Request:{Environment.NewLine}{data}");
                    };
                    wp.AfterResponse += (object sender, string data) =>
                    {
                        logger.Log(LogLevel.Verbose, $"Response:{Environment.NewLine}{data}");
                    };

                    if (string.IsNullOrEmpty(reference))
                    {
                        logger.Log(LogLevel.Info, "-------------------- Deliver --------------------");
                        aanleverResponse response = wp.Deliver(request, new Uri(deliveryUrl, UriKind.Relative));
                        reference = response.kenmerk;
                        logger.Log(LogLevel.Info, $"Aanleverkenmerk: {response.aanleverkenmerk}");
                        logger.Log(LogLevel.Info, $"Kenmerk: {reference}");
                    }

                    logger.Log(LogLevel.Info, "------------------- New Status ------------------");
                    getNieuweStatussenProcesRequest statusNewRequest = new getNieuweStatussenProcesRequest()
                    {
                        kenmerk = reference,
                        autorisatieAdres = noAusp
                    };
                    IEnumerable<StatusResultaat> statusResponse = wp.NewStatusProcess(statusNewRequest, new Uri(statusUrl, UriKind.Relative));
                    foreach (StatusResultaat statusResultaat in statusResponse)
                    {
                        logger.Log(LogLevel.Info, $"Status: {statusResultaat.statuscode} - {statusResultaat.statusomschrijving}");
                    }

                    logger.Log(LogLevel.Info, "------------------- All Status ------------------");
                    getStatussenProcesRequest statusAllRequest = new getStatussenProcesRequest()
                    {
                        kenmerk = reference,
                        autorisatieAdres = noAusp
                    };
                    statusResponse = wp.AllStatusProcess(statusAllRequest, new Uri(statusUrl, UriKind.Relative));
                    foreach (StatusResultaat statusResultaat in statusResponse)
                    {
                        logger.Log(LogLevel.Info, $"Status: {statusResultaat.statuscode} - {statusResultaat.statusomschrijving}");
                    }
                }
                catch (WusException e)
                {
                    logger.Log(LogLevel.Error, $"WusException: {e.WusCode} - {e.WusMessage}", e);
                }
                catch (SoapException e)
                {
                    logger.Log(LogLevel.Error, $"SoapException: {e.Code} - {e.Message}");
                }
                catch (HttpException e)
                {
                    logger.Log(LogLevel.Error, $"HttpException: {e.Code} - {e.Message}");
                }
                catch (Exception e)
                {
                    logger.Log(LogLevel.Error, $"Exception: {e.Message}");
                }
                finally
                {
                    logger.Log(LogLevel.Info, "Shutdown");
                }
            }

        }

        public static X509Certificate2 FindCertificate(string thumbprint)
        {
            bool verifyChain = false; // should be true, but using a test certificate now
            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certCollection = certStore.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    thumbprint,
                    verifyChain);

                if (certCollection.Count > 0)
                {
                    return certCollection[0];
                }
            }
            return null;
        }

        /*
        private static void StoreCertificate(X509Certificate2 certificate)
        {
            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadWrite);
                certStore.Add(certificate);
            }
        }
        */
    }
}
