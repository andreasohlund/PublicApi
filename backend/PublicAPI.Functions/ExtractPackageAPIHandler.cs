namespace PublicAPI.Functions
{
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using PublicAPI.APIExtraction;
    using PublicAPI.Messages;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ExtractPackageAPIHandler
    {
        public ExtractPackageAPIHandler(HttpClient httpClient, CloudBlobClient blobClient)
        {
            this.httpClient = httpClient;
            this.blobClient = blobClient;
        }

        [FunctionName("ExtractPackageAPI")]
        public async Task Run([QueueTrigger("extract-package-api", Connection = "AzureWebJobsStorage")]ExtractPackageAPI message, ILogger log)
        {
            var packageId = message.PackageId;
            var version = message.PackageVersion;

            log.LogInformation($"Extracting api from {packageId}({version})");

            var url = $"https://api.nuget.org/v3-flatcontainer/{packageId}/{version}/{packageId}.{version}.nupkg";

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var extractor = new PackageAPIExtractor();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            var packageDetails = await extractor.ExtractFromStream(responseStream);

            log.LogInformation(string.Join(";", packageDetails.TargetFrameworks.Select(fx => fx.Name)));
        }

        HttpClient httpClient;
        CloudBlobClient blobClient;
    }
}
