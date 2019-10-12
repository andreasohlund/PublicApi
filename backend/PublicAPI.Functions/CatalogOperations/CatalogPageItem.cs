namespace PublicAPI.CatalogOperations
{
    using System;
    using System.Text.Json.Serialization;

    public class CatalogPageItem
    {
        [JsonPropertyName("commitTimeStamp")]
        public DateTime CommitTimeStamp { get; set; }

        [JsonPropertyName("@id")]
        public string Url { get; set; }

        [JsonPropertyName("nuget:id")]
        public string PackageId { get; set; }

        [JsonPropertyName("nuget:version")]
        public string Version { get; set; }

        [JsonPropertyName("@type")]
        public string Type { get; set; }

        public bool IsNewPackage
        {
            get
            {
                return Type == "nuget:PackageDetails";
            }
        }
    }
}
