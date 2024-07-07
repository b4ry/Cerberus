using Cerberus.Api.ConfigurationSections;

namespace Cerberus.Api.Services
{
    public interface IJwtConfigurationSectionService
    {
        public JwtConfigurationSection GetJwtConfigurationSection();
    }
}
