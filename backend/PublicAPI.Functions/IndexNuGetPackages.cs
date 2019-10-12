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

            var packageMetadata = await catalogPageReader.ReadPackageMetadata(catalogPage);

            var packagesWithNetFxAsms = packageMetadata.Where(p => p.HasNetAssemblies).ToList();


            log.LogInformation($"Metadata read for page {nextPageToProcess.Id}: ");
            log.LogInformation($"Packages with dotnet assemblies: {packagesWithNetFxAsms.Count} ({packageMetadata.Count()})");
            log.LogInformation($"Total download size(MB): {packagesWithNetFxAsms.Sum(p => p.Size) / 1000000.0}");
        }

        async Task<CatalogCursor> GetCatalogCursor()
        {
            var container = blobClient.GetContainerReference("catalogcursors");

            var nugetCursorBlob = container.GetBlockBlobReference("nuget");

            using var readStream = await nugetCursorBlob.OpenReadAsync();

            return await JsonSerializer.DeserializeAsync<CatalogCursor>(readStream);
        }

        HttpClient httpClient;
        CloudBlobClient blobClient;
    }
}
