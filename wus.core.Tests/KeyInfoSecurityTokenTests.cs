using CoreWUS;
using Xunit;

namespace wus.core.Tests
{
    public class KeyInfoSecurityTokenTests : BaseTests
    {
        public KeyInfoSecurityTokenTests()
            : base()
        {
        }

        [Fact]
        public void KeyInfoSecurityToken_SetTokenAndPrefixAndVerify_Pass()
        {
            // Given
            string expectedTokenId = "TokenId";
            string expectedPrefix = "Prefix";

            // When
            KeyInfoSecurityToken keyInfoSecurityToken = new KeyInfoSecurityToken(expectedTokenId, expectedPrefix);

            // Then
            Assert.Equal(expectedTokenId, keyInfoSecurityToken.TokenId);
            Assert.Equal(expectedPrefix, keyInfoSecurityToken.Prefix);
        }
    }
}