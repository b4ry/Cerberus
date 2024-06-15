namespace Cerberus.Api.Services
{
    public class SecurityTokenGeneratorFactory(IServiceProvider serviceProvider) : ISecurityTokenGeneratorFactory
    {
        public ISecurityTokenGenerator Create(string tokenType)
        {
            return tokenType.ToLowerInvariant() switch
            {
                "jwt" => serviceProvider.GetRequiredService<JwtSecurityTokenGenerator>(),
                _ => throw new ArgumentException($"Unsupported token type: {tokenType}"),
            };
        }
    }
}
