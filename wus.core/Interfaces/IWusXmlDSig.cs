using System.Security.Cryptography.X509Certificates;

namespace CoreWUS
{
    public interface IWusXmlDSig
    {
        string CreateSignature(string xmlData, X509Certificate2 certificate, string[] referenceIds);
        bool VerifySignature(string xmlData);
        void SetSecurityToken(string tokenId, string tokenPrefix);
    }
}