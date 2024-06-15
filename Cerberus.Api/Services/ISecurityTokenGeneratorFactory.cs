namespace Cerberus.Api.Services
{
    public interface ISecurityTokenGeneratorFactory
    {
        public ISecurityTokenGenerator Create(string tokenGeneratorType);
    }
}