namespace PublicAPI.Functions
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.Azure.Storage.Blob;
    using PublicAPI.CatalogOperations;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class IndexNuGetPackages
    {
        public IndexNuGetPackages(HttpClient httpClient, CloudBlobClient blobClient)
        {
            this.httpClient = httpClient;
            this.blobClient = blobClient;
        }

        [FunctionName("IndexNuGetPackages")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"IndexNuGetPackages started, next run: {myTimer.ScheduleStatus.Next}");

            var reader = new CatalogIndexReader(httpClient);

            var catalogIndex = await reader.ReadUrl("https://api.nuget.org/v3/catalog0/index.json");

            var lastPage = catalogIndex.Items.OrderBy(i => i.CommitTimeStamp).Last();

            log.LogInformation($"IndexNuGetPackages complete, last page {lastPage.Id}, commited at: {lastPage.CommitTimeStamp}");
        }

        HttpClient httpClient;
        CloudBlobClient blobClient;
    }
}
