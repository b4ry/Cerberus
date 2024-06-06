namespace Cerberus.Constants
{
    /// <summary>
    /// JWT configuration property constant names. Strings.
    /// </summary>
    public static class JwtConfigurationPropertyNames
    {
        /// <summary>
        /// JWT key used for generating symmetric security key. String.
        /// </summary>
        public static readonly string Key = "Jwt:Key";
        /// <summary>
        /// JWT issuer used for generating a JWT token. String.
        /// </summary>
        public static readonly string Issuer = "Jwt:Issuer";
    }
}
