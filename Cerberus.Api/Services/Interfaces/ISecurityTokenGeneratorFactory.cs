namespace Cerberus.Api.Services.Interfaces
{
    public interface ISecurityTokenGeneratorFactory
    {
        public ISecurityTokenGenerator Create(string tokenGeneratorType);
    }
}