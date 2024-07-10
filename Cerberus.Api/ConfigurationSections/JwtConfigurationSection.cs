namespace Cerberus.Api.ConfigurationSections
{
    /// <summary>
    /// Class encapsulating JWT configuration section coming from IConfiguration.
    /// </summary>
    public class JwtConfigurationSection
    {
        /// <summary>
        /// JWT key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// JWT issuer
        /// </summary>
        public string Issuer { get; set; }
    }
}
