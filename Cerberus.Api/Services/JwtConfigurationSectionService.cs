using Cerberus.Api.ConfigurationSections;

namespace Cerberus.Api.Services
{
    public class JwtConfigurationSectionService(IConfiguration configuration) : IJwtConfigurationSectionService
    {
        public JwtConfigurationSection GetJwtConfigurationSection()
        {
            var jwtConfigurationSection = new JwtConfigurationSection();
            configuration.GetSection(Constants.Constants.Jwt).Bind(jwtConfigurationSection);

            return jwtConfigurationSection;
        }
    }
}
