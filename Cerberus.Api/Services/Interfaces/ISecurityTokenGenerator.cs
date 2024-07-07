namespace Cerberus.Api.Services.Interfaces
{
    /// <summary>
    /// Security token generators' interface. It has to be implemented by all the security token generators.
    /// </summary>
    public interface ISecurityTokenGenerator
    {
        /// <summary>
        /// Generates a security token, eg. JWT.
        /// </summary>
        /// <param name="userName">Logging in user's name</param>
        /// <returns>A security token. String.</returns>
        public string GenerateSecurityToken(string userName);
    }
}
