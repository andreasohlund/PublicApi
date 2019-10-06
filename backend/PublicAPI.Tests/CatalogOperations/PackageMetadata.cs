namespace PublicAPI.Tests
{
    using System.Text.Json.Serialization;

    public class PackageMetadata
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("packageSize")]
        public double Size { get; set; }
    }
}
