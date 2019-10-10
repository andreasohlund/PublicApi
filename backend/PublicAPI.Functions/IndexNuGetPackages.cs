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

            var reader = new CatalogIndexReader(httpClient);

            var catalogIndex = await reader.ReadUrl("https://api.nuget.org/v3/catalog0/index.json");

            var pages = catalogIndex.Items.Where(p => p.CommitTimeStamp > catalogCursor.CommitTimeStamp)
                .ToList();

            var nextPageToProcess = pages.OrderBy(p => p.CommitTimeStamp).First();

            log.LogInformation($"Index parsed, {pages.Count} found, processing page {nextPageToProcess.Id} ({nextPageToProcess.CommitTimeStamp})");
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
