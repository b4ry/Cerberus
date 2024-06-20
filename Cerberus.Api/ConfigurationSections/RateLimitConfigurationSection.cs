namespace Cerberus.Api.Options
{
    public class RateLimitConfigurationSection
    {
        public string PolicyName { get; set; }
        public int PermitLimit { get; set; }
        public int Window { get; set; }
        public int SegmentsPerWindow { get; set; }
        public int QueueLimit { get; set; }
    }
}
