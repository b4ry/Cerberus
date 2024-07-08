namespace Cerberus.Api.Services.Interfaces
{
    public interface IPasswordService
    {
        public string GenerateSalt();
        public string HashPassword(string password, string salt);
    }
}
