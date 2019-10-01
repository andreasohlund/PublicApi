namespace PublicAPI.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public partial class CatalogOperationTests
    {
        public class CatalogIndex
        {
            [JsonPropertyName("commitTimeStamp")]
            public DateTime CommitTimeStamp { get; set; }

            [JsonPropertyName("items")]
            public List<Page> Items { get; set; }

            public class Page
            {
                [JsonPropertyName("commitTimeStamp")]
                public DateTime CommitTimeStamp { get; set; }

                [JsonPropertyName("@id")]
                public string Id { get; set; }

                [JsonPropertyName("count")]
                public int Count { get; set; }
            }
        }
    }
}
