using Cerberus.Api.Services;

namespace Tests.Services
{
    public class PasswordServiceTests
    {
        [Fact]
        public void GenerateSalt_ShouldGenerateString()
        {
            // arrange
            var passwordService = new PasswordService();

            // act
            var salt = passwordService.GenerateSalt();

            // assert
            Assert.NotNull(salt);
        }
    }
}
