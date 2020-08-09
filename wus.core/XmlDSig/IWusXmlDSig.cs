using System.Security.Cryptography.X509Certificates;

namespace CoreWUS
{
    public interface IWusXmlDSig
    {
        string SignXmlDSig(string xmlData, X509Certificate2 certificate, string[] referenceIds);
        bool VerifyXmlDSig(string xmlData);
        void SetSecurityToken(string tokenId, string tokenPrefix);
    }
}