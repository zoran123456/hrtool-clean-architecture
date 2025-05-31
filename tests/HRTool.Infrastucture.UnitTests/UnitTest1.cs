using Xunit;
using FluentAssertions;

namespace HRTool.Infrastucture.UnitTests
{
    public class PasswordHashingTests
    {
        [Fact]
        public void PasswordHasher_CreatesDifferentHashesForDifferentPasswords()
        {
            // Arrange
            var password1 = "Password123!";
            var password2 = "Password456!";
            // Act
            // (Pseudo-code, replace with your actual hasher)
            // var hash1 = PasswordHasher.Hash(password1);
            // var hash2 = PasswordHasher.Hash(password2);
            // Assert
            // hash1.Should().NotBe(hash2);
        }
    }
}