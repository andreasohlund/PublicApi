namespace PublicAPI.Tests
{
    using System.Text.Json.Serialization;

    public class PackageMetadata
    {
        [JsonPropertyName("id")]
        public string PackageId { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}
