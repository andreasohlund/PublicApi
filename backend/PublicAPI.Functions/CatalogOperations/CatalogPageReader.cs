namespace PublicAPI.CatalogOperations
{
    using System;
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

        //TODO: Convert back to async stream once functions support netcore3.0
        public async Task<IEnumerable<PackageMetadata>> ReadPackageMetadata(CatalogPage catalogPage,
            //, [EnumeratorCancellation] CancellationToken cancellationToken = default
            DateTime previousCommitTimeStamp)
        {
            var tasks = new List<Task<PackageMetadata>>();

            using var throttler = new SemaphoreSlim(maxConcurrentHttpCalls);

            foreach (var item in catalogPage.Items.Where(p => p.Type == "nuget:PackageDetails" && p.CommitTimeStamp > previousCommitTimeStamp))
            {
                tasks.Add(GetMetadataForPackage(item.Url, throttler
                    //, cancellationToken
                    ));
            }

            //while (tasks.Count > 0)
            //{
            //    var done = await Task.WhenAny(tasks);
            //    tasks.Remove(done);

            //    yield return await done;
            //}

            return await Task.WhenAll(tasks);
        }

        async Task<PackageMetadata> GetMetadataForPackage(string packageMetadataUrl, SemaphoreSlim throttler, CancellationToken cancellationToken = default)
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
