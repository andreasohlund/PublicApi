namespace PublicAPI.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;

    public class PackageMetadata
    {
        public PackageMetadata()
        {
            PackageEntries = new List<PackageEntry>();
        }
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("packageSize")]
        public long Size { get; set; }

        [JsonPropertyName("packageEntries")]
        public List<PackageEntry> PackageEntries { get; set; }

        public bool HasNetAssemblies
        {
            get
            {
                return PackageEntries.Any(pe => pe.FullName.ToLower().StartsWith("lib/"));
            }
        }

        public class PackageEntry
        {
            [JsonPropertyName("fullName")]
            public string FullName { get; set; }
        }
    }
}
