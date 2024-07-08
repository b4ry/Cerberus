using Cerberus.Api.Services;

namespace Tests.Services
{
    public class PasswordServiceTests
    {
        [Fact]
        public void GenerateSalt_ShouldReturnBase64String()
        {
            // arrange
            var passwordService = new PasswordService();

            // act
            var salt = passwordService.GenerateSalt();

            // assert
            Assert.NotNull(salt);
            Assert.Equal(16, Convert.FromBase64String(salt).Length);
        }

        [Fact]
        public void HashPassword_ShouldReturnBase64String()
        {
            // arrange
            var passwordService = new PasswordService();

            // act
            var hashedPassword = passwordService.HashPassword("testPassword", "testSalt");

            // assert
            Assert.NotNull(hashedPassword);
            Assert.Equal(32, Convert.FromBase64String(hashedPassword).Length);
        }

        [Fact]
        public void HashPassword_ShouldReturnSameHash_ForSamePasswordAndSalt()
        {
            // arrange
            var passwordService = new PasswordService();

            // act
            var hashedPassword1 = passwordService.HashPassword("testPassword", "testSalt");
            var hashedPassword2 = passwordService.HashPassword("testPassword", "testSalt");

            // assert
            Assert.Equal(hashedPassword1, hashedPassword2);
        }

        [Fact]
        public void HashPassword_ShouldReturnDifferentHash_ForDifferentPasswordAndSameSalt()
        {
            // arrange
            var passwordService = new PasswordService();

            // act
            var hashedPassword1 = passwordService.HashPassword("testPassword1", "testSalt");
            var hashedPassword2 = passwordService.HashPassword("testPassword", "testSalt");

            // assert
            Assert.NotEqual(hashedPassword1, hashedPassword2);
        }

        [Fact]
        public void HashPassword_ShouldReturnDifferentHash_ForSamePasswordAndDifferentSalt()
        {
            // arrange
            var passwordService = new PasswordService();

            // act
            var hashedPassword1 = passwordService.HashPassword("testPassword", "testSalt1");
            var hashedPassword2 = passwordService.HashPassword("testPassword", "testSalt");

            // assert
            Assert.NotEqual(hashedPassword1, hashedPassword2);
        }
    }
}
