namespace RollercoasterDataAnalytics.Configurations
{
    public class WartezeitenAppConfiguration
    {
        public required string Uri { get; set; }
        public string Language { get; set; } = "en";
        public string ApiVersion { get; set; } = "v1";
    }
}
