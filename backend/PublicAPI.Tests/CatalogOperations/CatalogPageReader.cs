namespace PublicAPI.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
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

        public async Task<IEnumerable<PackageMetadata>> ReadPackageMetadata(CatalogPage catalogPage, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task<PackageMetadata>>();

            using var throttler = new SemaphoreSlim(maxConcurrentHttpCalls);

            foreach (var item in catalogPage.Items.Where(p => p.Type == "nuget:PackageDetails"))
            {
                tasks.Add(GetMetadataForPackage(item.Url, throttler, cancellationToken));
            }

            return await Task.WhenAll(tasks);
        }

        async Task<PackageMetadata> GetMetadataForPackage(string packageMetadataUrl, SemaphoreSlim throttler, CancellationToken cancellationToken)
        {
            try
            {
                await throttler.WaitAsync(cancellationToken);

                var packageMetadataReader = new PackageMetadataReader(httpClient);

                return await packageMetadataReader.ReadUrl(packageMetadataUrl, cancellationToken);
            }
            finally
            {
                throttler.Release();
            }
        }

        readonly HttpClient httpClient;
        readonly int maxConcurrentHttpCalls;
    }
}
