namespace PublicAPI.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;

    public class CatalogPageReader
    {
        public CatalogPageReader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            maxConcurrentHttpCalls = 20;
        }

        public async Task<CatalogPage> ReadUrl(string catalogPageUrl)
        {
            var response = await httpClient.GetAsync(catalogPageUrl);

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<CatalogPage>(responseStream);
        }

        public async Task<IEnumerable<PackageMetadata>> ReadPackageMetadata(CatalogPage catalogPage)
        {
            var tasks = new List<Task<PackageMetadata>>();

            using var throttler = new SemaphoreSlim(maxConcurrentHttpCalls);

            foreach (var item in catalogPage.Items.Where(p => p.Type == "nuget:PackageDetails"))
            {
                tasks.Add(GetMetadataForPackage(item.Url, throttler));
            }

            return await Task.WhenAll(tasks);
        }

        async Task<PackageMetadata> GetMetadataForPackage(string packageMetadataUrl, SemaphoreSlim throttler)
        {
            try
            {
                await throttler.WaitAsync();

                var response = await httpClient.GetAsync(packageMetadataUrl);

                response.EnsureSuccessStatusCode();

                using var responseStream = await response.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<PackageMetadata>(responseStream);
            }
            finally
            {
                throttler.Release();
            }
        }

        readonly HttpClient httpClient;
        readonly int maxConcurrentHttpCalls;
    }

    public class PackageMetadata
    {
        [JsonPropertyName("id")]
        public string PackageId { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}
