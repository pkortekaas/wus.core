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
            // Arrange
            string expectedTokenId = "TokenId";
            string expectedPrefix = "Prefix";

            // Act
            KeyInfoSecurityToken keyInfoSecurityToken = new KeyInfoSecurityToken(expectedTokenId, expectedPrefix);

            // Assert
            Assert.Equal(expectedTokenId, keyInfoSecurityToken.TokenId);
            Assert.Equal(expectedPrefix, keyInfoSecurityToken.Prefix);
        }
    }
}