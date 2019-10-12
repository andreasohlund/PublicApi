namespace PublicAPI.CatalogOperations
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class CatalogPage
    {
        [JsonPropertyName("commitTimeStamp")]
        public DateTime CommitTimeStamp { get; set; }

        [JsonPropertyName("items")]
        public List<CatalogPageItem> Items { get; set; }
    }
}
