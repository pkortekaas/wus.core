using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Xunit;

namespace wus.core.Tests
{
    public class BaseTests
    {
        private readonly string _byteOrderMarkUTF8;
        protected readonly string _certificateThumbPrint;
        protected readonly string BasePath;

        protected string TestDataPath => Path.Combine(BasePath, "testdata");
        public BaseTests()
        {
            BasePath = GetTestRoot();
            _byteOrderMarkUTF8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            _certificateThumbPrint = Environment.GetEnvironmentVariable("PKIO_THUMB");
        }

        public X509Certificate2 GetCertificate(string thumbprint)
        {
            return FindCertificate(thumbprint);
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

    }
}