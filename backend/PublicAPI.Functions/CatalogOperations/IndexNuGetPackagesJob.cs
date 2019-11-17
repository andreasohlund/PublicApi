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
    using PublicAPI.Messages;

    public class IndexNuGetPackagesJob
    {
        public IndexNuGetPackagesJob(HttpClient httpClient, CloudBlobClient blobClient)
        {
            this.httpClient = httpClient;
            this.blobClient = blobClient;
        }

        [FunctionName("IndexNuGetPackages")]
        [Disable("DisableScheduleJobs")]
        public async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, ILogger log, [Queue("extract-package-api", Connection = "AzureWebJobsStorage")]IAsyncCollector<Messages.ExtractPackageAPI> collector)
        {
            var catalogCursor = await GetCatalogCursor();

            var catalogIndexReader = new CatalogIndexReader(httpClient);

            var catalogIndex = await catalogIndexReader.ReadUrl("https://api.nuget.org/v3/catalog0/index.json");

            var catalogIndexPages = catalogIndex.Items.Where(p => p.CommitTimeStamp > catalogCursor.CommitTimeStamp)
                .ToList();

            var nextPageToProcess = catalogIndexPages.OrderBy(p => p.CommitTimeStamp).FirstOrDefault();

            if (nextPageToProcess == null)
            {
                log.LogInformation($"Index parsed, no new pages found");

                return;
            }

            log.LogInformation($"Index parsed, {catalogIndexPages.Count} found, processing page {nextPageToProcess.Id} ({nextPageToProcess.CommitTimeStamp})");

            var catalogPageReader = new CatalogPageReader(httpClient);

            var catalogPage = await catalogPageReader.ReadUrl(nextPageToProcess.Id);

            var packageMetadata = await ReadPackageMetadata(catalogPage, catalogCursor.CommitTimeStamp);

            foreach (var package in packageMetadata)
            {
                await collector.AddAsync(new ExtractPackageAPI
                {
                    PackageId = package.Id,
                    PackageVersion = package.Version,
                    HasDotNetAssemblies = package.HasDotNetAssemblies
                });
            }

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

        public async Task<IEnumerable<PackageMetadata>> ReadPackageMetadata(CatalogPage catalogPage, DateTime previousCommitTimeStamp)
        {
            var tasks = new List<Task<PackageMetadata>>();

            using var throttler = new SemaphoreSlim(20);

            foreach (var item in catalogPage.Items.Where(p => p.IsNewPackage && p.CommitTimeStamp > previousCommitTimeStamp))
            {
                tasks.Add(GetMetadataForPackage(item.Url, throttler));
            }

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
