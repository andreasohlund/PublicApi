namespace PublicAPI.CatalogOperations
{
    using System;
    using System.Text.Json.Serialization;

    public class CatalogCursor
    {
        [JsonPropertyName("commitTimeStamp")]
        public DateTime CommitTimeStamp { get; set; }
    }
}