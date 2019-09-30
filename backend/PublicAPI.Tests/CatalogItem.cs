﻿namespace PublicAPI.Tests
{
    using System.Text.Json.Serialization;

    public class CatalogItem
    {
        [JsonPropertyName("nuget:id")]
        public string PackageId { get; set; }

        [JsonPropertyName("nuget:version")]
        public string Version { get; set; }

        [JsonPropertyName("@type")]
        public string Type { get; set; }
    }
}