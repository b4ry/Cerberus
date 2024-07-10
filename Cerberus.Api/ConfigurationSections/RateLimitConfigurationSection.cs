namespace Cerberus.Api.Options
{
    /// <summary>
    /// Class encapsulating rate limit configuration section coming from IConfiguration.
    /// </summary>
    public class RateLimitConfigurationSection
    {
        /// <summary>
        /// Rate limiting algorithm name
        /// </summary>
        public string PolicyName { get; set; }

        /// <summary>
        /// Number of requests in a window, before blocking
        /// </summary>
        public int PermitLimit { get; set; }

        /// <summary>
        /// Size of a window 
        /// </summary>
        public int Window { get; set; }

        /// <summary>
        /// Maximum number of segments a window is divided to (used for sliding algorithm)
        /// </summary>
        public int SegmentsPerWindow { get; set; }

        /// <summary>
        /// How many requests to queue when a window is filled
        /// </summary>
        public int QueueLimit { get; set; }
    }
}
