﻿using Cerberus.Api.ConfigurationSections;
using Cerberus.Api.Services.Interfaces;

namespace Cerberus.Api.Services
{
    public class JwtConfigurationSectionService(IConfiguration configuration) : IJwtConfigurationSectionService
    {
        public JwtConfigurationSection GetJwtConfigurationSection()
        {
            var jwtConfigurationSection = new JwtConfigurationSection();
            configuration.GetSection(Constants.Constants.JwtSectionName).Bind(jwtConfigurationSection);

            return jwtConfigurationSection;
        }
    }
}
