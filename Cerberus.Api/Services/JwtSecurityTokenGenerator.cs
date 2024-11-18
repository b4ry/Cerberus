using Cerberus.Api.DTOs;
using Cerberus.Api.Services.Interfaces;
using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cerberus.Api.Services
{
    /// <summary>
    /// Jwt security and refresh tokens generator. Implements ISecurityTokenGenerator interface.
    /// </summary>
    /// <param name="jwtConfigurationSectionService">Service to retrieve jwt configuration section</param>
    /// <param name="refreshTokenRepository">Refresh tokens repository</param>
    public class JwtSecurityTokenGenerator(
        IJwtConfigurationSectionService jwtConfigurationSectionService,
        IRefreshTokenRepository refreshTokenRepository) : ISecurityTokenGenerator
    {
        /// <summary>
        /// Generates a jwt security token based on the jwt configuration's key and issuer. Also, generates a refresh token.
        /// </summary>
        /// <param name="username">Logging in user's name</param>
        /// <param name="generateRefreshToken">Flag to control refresh token generation</param>
        /// <returns>
        ///     AuthToken containing a jwt token issued by the configuration issuer, expiring in x minutes, signed with the configuration key
        ///     and a corresponding refresh token.
        /// </returns>
        public async Task<AuthToken> GenerateSecurityTokenAsync(string username, bool generateRefreshToken)
        {
            try
            {
                var jwtConfigurationSection = jwtConfigurationSectionService.GetJwtConfigurationSection();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigurationSection.Key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, username) };

                var jwtSecurityToken = new JwtSecurityToken(
                    jwtConfigurationSection.Issuer,
                    null,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials,
                    claims: claims
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                // TODO: move it somewhere else; it should not be a responsibility of this method
                if (generateRefreshToken)
                {
                    var refreshToken = new RefreshTokenEntity()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ValidUntil = DateTime.UtcNow.AddDays(1)
                    };

                    await refreshTokenRepository.AddAsync(refreshToken);

                    return new AuthToken(jwt, refreshToken.Id);
                }

                return new AuthToken(jwt, null);
            }
            catch(Exception ex)
            {
                ex.Data.Add("JwtTokenGeneration", $"Error occured while generating jwt token for user: {username}");

                throw;
            }
        }
    }
}
