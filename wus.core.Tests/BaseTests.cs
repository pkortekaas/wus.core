using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CoreWUS;

namespace wus.core.Tests
{
    public class BaseTests
    {
        private readonly string _byteOrderMarkUTF8;
        private readonly string _certificateThumbPrint;
        protected readonly string BasePath;

        protected string TestDataPath => Path.Combine(BasePath, "testdata");

        protected aanleverRequest AanleverRequest => CreateAanleverRequest();

        protected getNieuweStatussenProcesRequest NieuweStatussenProcesRequest => CreateNewStatusRequest();
        public BaseTests()
        {
            BasePath = GetTestRoot();
            _byteOrderMarkUTF8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            _certificateThumbPrint = Environment.GetEnvironmentVariable("PKIO_THUMB");
        }

        public string GetStringWithoutBOM(byte[] bytes)
        {
            string result = Encoding.UTF8.GetString(bytes);
            if (result.StartsWith(_byteOrderMarkUTF8))
            {
                result = result.Remove(0, _byteOrderMarkUTF8.Length);
            }
            return result;
        }

        public void WithX509Certificate(Action<X509Certificate2> action)
        {
            using (X509Certificate2 certificate = FindCertificate(_certificateThumbPrint))
            {
                action(certificate);
            }
        }

        private static string GetTestRoot()
        {
            string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            int index = executingPath.IndexOf("/bin/");
            return executingPath.Substring(0, index + 1);
        }

        private static X509Certificate2 FindCertificate(string thumbprint)
        {
            bool verifyChain = false;
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

        private static aanleverRequest CreateAanleverRequest()
        {
            return new aanleverRequest()
            {
                berichtsoort = "Omzetbelasting",
                aanleverkenmerk = Guid.NewGuid().ToString("D"),
                autorisatieAdres = "http://geenausp.nl",
                identiteitBelanghebbende = new identiteitType() {
                    nummer = "001000044B37",
                    type = "Fi"
                },
                rolBelanghebbende = "Bedrijf",
                berichtInhoud = new berichtInhoudType() {
                    mimeType = "text/xml",
                    bestandsnaam = "Omzetbelasting.xbrl",
                    inhoud = Encoding.UTF8.GetBytes("UnitTest")
                }
            };
        }

        private static getNieuweStatussenProcesRequest CreateNewStatusRequest()
        {
            return new getNieuweStatussenProcesRequest()
            {
                kenmerk = Guid.NewGuid().ToString("D"),
                autorisatieAdres = "http://geenausp.nl"
            };
        }

    }
}