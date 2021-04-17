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
        protected readonly string BasePath;

        protected string TestDataPath => Path.Combine(BasePath, "testdata");

        protected aanleverRequest AanleverRequest => CreateAanleverRequest();

        protected getNieuweStatussenProcesRequest NieuweStatussenProcesRequest => CreateNewStatusRequest();
        public BaseTests()
        {
            BasePath = GetTestRoot();
            _byteOrderMarkUTF8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
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
            // Use a self signed x509 certificate for signature and verification
            using (X509Certificate2 certificate = new X509Certificate2(Path.Combine(TestDataPath, "wus-test.pfx")))
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

        private static aanleverRequest CreateAanleverRequest()
        {
            return new aanleverRequest()
            {
                berichtsoort = "Omzetbelasting",
                aanleverkenmerk = Guid.NewGuid().ToString("D"),
                autorisatieAdres = "http://geenausp.nl",
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