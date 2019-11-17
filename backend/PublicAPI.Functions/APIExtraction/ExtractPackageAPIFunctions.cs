namespace PublicAPI.Functions
{
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using PublicAPI.APIExtraction;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using PublicAPI.Messages;

    public class ExtractPackageAPIFunctions
    {
        public ExtractPackageAPIFunctions(HttpClient httpClient, CloudBlobClient blobClient)
        {
            this.httpClient = httpClient;
            this.blobClient = blobClient;
        }

        [FunctionName("GetPackageApi")]
        public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "packages/{package}/{version}")]HttpRequestMessage req,
            string package,
            string version,
            ILogger log)
        {
            var url = await ExtractPackage(package, version, false, log);

            return new RedirectResult(url, true);
        }

        [FunctionName("ExtractPackageAPI")]
        public async Task HandleMessage([QueueTrigger("extract-package-api", Connection = "AzureWebJobsStorage")]ExtractPackageAPI message, ILogger log)
        {
            var skipDownload = !message.HasDotNetAssemblies;

            await ExtractPackage(message.PackageId, message.PackageVersion, skipDownload, log);
        }

        async Task<string> ExtractPackage(string packageId, string packageVersion, bool skipDownload, ILogger log)
        {
            var version = packageVersion.Split("+").First();//remove the semver build info part if present

            var extractor = new PackageAPIExtractor();

            var container = blobClient.GetContainerReference("packages");

            var packageBlob = container.GetBlockBlobReference($"{packageId.ToLower()}/{version.ToLower()}");

            if (await packageBlob.ExistsAsync())
            {
                await packageBlob.FetchAttributesAsync();

                var schemaversion = packageBlob.Metadata["schemaversion"];

                if (Version.Parse(extractor.Version) <= Version.Parse(schemaversion))
                {
                    log.LogInformation($"API for {packageId}({version}) already generated with schema version {extractor.Version}");

                    return packageBlob.Uri.ToString();
                }
            }

            if (skipDownload)
            {
                log.LogInformation($"{packageId}({version}) has no assemblies");

                await StorePackageApi(packageId, version, extractor.Version, new PackageDetails());

                return packageBlob.Uri.ToString();
            }

            var url = $"https://api.nuget.org/v3-flatcontainer/{packageId}/{version}/{packageId}.{version}.nupkg";

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            var packageDetails = await extractor.ExtractFromStream(responseStream);

            await StorePackageApi(packageId, version, extractor.Version, packageDetails);

            return packageBlob.Uri.ToString();
        }

        async Task StorePackageApi(string packageId, string version, string schemaVersion, PackageDetails packageDetails)
        {
            var container = blobClient.GetContainerReference("packages");

            var packageBlob = container.GetBlockBlobReference($"{packageId.ToLower()}/{version.ToLower()}");

            packageBlob.Properties.ContentEncoding = "gzip";
            packageBlob.Properties.ContentType = "text/json";
            packageBlob.Metadata["schemaversion"] = schemaVersion;

            using var blobStream = new MemoryStream();
            using var gZipStream = new GZipStream(blobStream, CompressionMode.Compress, true);

            await JsonSerializer.SerializeAsync(gZipStream, packageDetails);

            gZipStream.Close();
            blobStream.Position = 0;

            await packageBlob.UploadFromStreamAsync(blobStream);
        }

        HttpClient httpClient;
        CloudBlobClient blobClient;
    }
}
