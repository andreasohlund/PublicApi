namespace PublicAPI.Functions
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.Azure.Storage.Blob;
    using PublicAPI.CatalogOperations;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System;
    using System.Text.Json;
    using System.Collections.Generic;
    using System.Threading;

    public class IndexNuGetPackages
    {
        public IndexNuGetPackages(HttpClient httpClient, CloudBlobClient blobClient)
        {
            this.httpClient = httpClient;
            this.blobClient = blobClient;
        }

        [FunctionName("IndexNuGetPackages")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"IndexNuGetPackages started, next run: {myTimer.ScheduleStatus.Next}");

            var catalogCursor = await GetCatalogCursor();

            var catalogIndexReader = new CatalogIndexReader(httpClient);

            var catalogIndex = await catalogIndexReader.ReadUrl("https://api.nuget.org/v3/catalog0/index.json");

            var catalogIndexPages = catalogIndex.Items.Where(p => p.CommitTimeStamp > catalogCursor.CommitTimeStamp)
                .ToList();

            var nextPageToProcess = catalogIndexPages.OrderBy(p => p.CommitTimeStamp).First();

            log.LogInformation($"Index parsed, {catalogIndexPages.Count} found, processing page {nextPageToProcess.Id} ({nextPageToProcess.CommitTimeStamp})");

            var catalogPageReader = new CatalogPageReader(httpClient);

            var catalogPage = await catalogPageReader.ReadUrl(nextPageToProcess.Id);

            var packageMetadata = await ReadPackageMetadata(catalogPage, catalogCursor.CommitTimeStamp);

            var packagesWithNetFxAsms = packageMetadata.Where(p => p.HasNetAssemblies).ToList();


            log.LogInformation($"Metadata read for page {nextPageToProcess.Id}: ");
            log.LogInformation($"Packages with dotnet assemblies: {packagesWithNetFxAsms.Count} ({packageMetadata.Count()})");
            log.LogInformation($"Total download size(MB): {packagesWithNetFxAsms.Sum(p => p.Size) / 1000000.0}");

            var newCursor = new CatalogCursor
            {
                CommitTimeStamp = catalogPage.CommitTimeStamp
            };

            await StoreCatalogCursor(newCursor);
        }

        Task StoreCatalogCursor(CatalogCursor newCursor)
        {
            var container = blobClient.GetContainerReference("catalogcursors");

            var nugetCursorBlob = container.GetBlockBlobReference("nuget");

            nugetCursorBlob.Properties.ContentType = "text/json";

            var content = JsonSerializer.Serialize(newCursor);

            return nugetCursorBlob.UploadTextAsync(content);
        }

        async Task<CatalogCursor> GetCatalogCursor()
        {
            var container = blobClient.GetContainerReference("catalogcursors");

            var nugetCursorBlob = container.GetBlockBlobReference("nuget");

            using var readStream = await nugetCursorBlob.OpenReadAsync();

            return await JsonSerializer.DeserializeAsync<CatalogCursor>(readStream);
        }

        public async Task<IEnumerable<PackageMetadata>> ReadPackageMetadata(CatalogPage catalogPage,
            //, [EnumeratorCancellation] CancellationToken cancellationToken = default
            DateTime previousCommitTimeStamp)
        {
            var tasks = new List<Task<PackageMetadata>>();

            using var throttler = new SemaphoreSlim(20);

            foreach (var item in catalogPage.Items.Where(p => p.IsNewPackage && p.CommitTimeStamp > previousCommitTimeStamp))
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

        HttpClient httpClient;
        CloudBlobClient blobClient;
    }
}
