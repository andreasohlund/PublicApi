namespace PublicAPI.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class CatalogPageReader
    {
        public CatalogPageReader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<CatalogPage> ReadUrl(string catalogPageUrl)
        {
            var response = await httpClient.GetAsync(catalogPageUrl);

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync<CatalogPage>(responseStream);
        }

        public async Task<IEnumerable<PackageMetadata>> ReadPackageMetadata(CatalogPage catalogPage)
        {
            var result = new List<PackageMetadata>();

            foreach (var item in catalogPage.Items.Where(p=>p.Type == "nuget:PackageDetails").Take(100))
            {
                var response = await httpClient.GetAsync(item.Url);

                response.EnsureSuccessStatusCode();

                using var responseStream = await response.Content.ReadAsStreamAsync();

                var packageMetadata = await System.Text.Json.JsonSerializer.DeserializeAsync<PackageMetadata>(responseStream);

                result.Add(packageMetadata);
            }

            return result;
        }

        readonly HttpClient httpClient;
    }

    public class PackageMetadata
    {
        [JsonPropertyName("id")]
        public string PackageId { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}
