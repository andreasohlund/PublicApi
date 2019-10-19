namespace PublicAPI.Functions
{
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using PublicAPI.APIExtraction;
    using PublicAPI.Messages;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
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
            var extractor = new PackageAPIExtractor();


            if (!message.HasDotNetAssemblies)
            {
                log.LogInformation($"{packageId}({version}) has no assemblies");

                await StorePackageApi(packageId, version, extractor.Version, new PackageDetails());

                return;
            }


            log.LogInformation($"Extracting api from {packageId}({version})");

            var url = $"https://api.nuget.org/v3-flatcontainer/{packageId}/{version}/{packageId}.{version}.nupkg";

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();


            using var responseStream = await response.Content.ReadAsStreamAsync();

            var packageDetails = await extractor.ExtractFromStream(responseStream);

            await StorePackageApi(packageId, version, extractor.Version, packageDetails);
        }

        Task StorePackageApi(string packageId, string version, string schemaVersion, PackageDetails packageDetails)
        {
            var container = blobClient.GetContainerReference("packages");

            var packageBlob = container.GetBlockBlobReference($"{packageId.ToLower()}/{version.ToLower()}");

            packageBlob.Properties.ContentType = "text/json";
            packageBlob.Metadata["schemaversion"] = schemaVersion;

            var content = JsonSerializer.Serialize(packageDetails);

            return packageBlob.UploadTextAsync(content);
        }

        HttpClient httpClient;
        CloudBlobClient blobClient;
    }
}
