using Cerberus.Api.ConfigurationSections;

namespace Cerberus.Api.Services.Interfaces
{
    public interface IJwtConfigurationSectionService
    {
        public JwtConfigurationSection GetJwtConfigurationSection();
    }
}
