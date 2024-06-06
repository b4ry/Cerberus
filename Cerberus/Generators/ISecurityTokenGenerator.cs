namespace Cerberus.Generators
{
    /// <summary>
    /// Security token generators' interface. It has to be implemented by all the security token generators.
    /// If one needs to create a security token generator dynamically during runtime, then it can be done with a factory pattern and delegates.
    /// </summary>
    public interface ISecurityTokenGenerator
    {
        /// <summary>
        /// Generates a security token, eg. JWT.
        /// </summary>
        /// <returns>A security token. String.</returns>
        public string GenerateSecurityToken();
    }
}
