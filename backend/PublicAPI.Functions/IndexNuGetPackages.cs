namespace PublicAPI.Functions
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using PublicAPI.CatalogOperations;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class IndexNuGetPackages
    {
        static HttpClient httpClient = new HttpClient();

        [FunctionName("IndexNuGetPackages")]
        public async static Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"IndexNuGetPackages started, next run: {myTimer.ScheduleStatus.Next}");

            var reader = new CatalogIndexReader(httpClient);

            var catalogIndex = await reader.ReadUrl("https://api.nuget.org/v3/catalog0/index.json");

            var lastPage = catalogIndex.Items.OrderBy(i => i.CommitTimeStamp).Last();

            log.LogInformation($"IndexNuGetPackages complete, last page {lastPage.Id}, commited at: {lastPage.CommitTimeStamp}");
        }
    }
}
