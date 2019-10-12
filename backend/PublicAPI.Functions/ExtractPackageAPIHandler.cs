namespace PublicAPI.Functions
{
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using PublicAPI.Messages;
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
            log.LogInformation($"Extracting api from {message.PackageId}({message.PackageVersion})");
        }

        HttpClient httpClient;
        CloudBlobClient blobClient;
    }
}
