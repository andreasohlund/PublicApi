using ApprovalTests.Reporters;

[assembly: UseReporter(typeof(DiffReporter))]

namespace PublicAPI.Tests
{
    using ApprovalTests;
    using NUnit.Framework;
    using PublicAPI.APIExtraction;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    [TestFixture]
    public class APIExtractionTests
    {
        [Test]
        public async Task ApproveAPIExtraction()
        {
            var extractor = new AssemblyAPIExtractor();
            var binFolder = AppDomain.CurrentDomain.BaseDirectory.Replace("PublicAPI.Tests", "ExampleAssembly");
            var path = Path.Combine(binFolder, "ExampleAssembly.dll");

            using var assemblyStream = new FileStream(path, FileMode.Open);

            var publicTypes = await extractor.ExtractFromStream(assemblyStream);

            Approvals.VerifyJson(JsonSerializer.Serialize(publicTypes));
        }

        [Test]
        public async Task ApproveNServiceBus710API()
        {
            var packageId = "NServiceBus";
            var version = "7.1.0";

            var httpClient = new HttpClient();
            var url = $"https://api.nuget.org/v3-flatcontainer/{packageId}/{version}/{packageId}.{version}.nupkg";

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var extractor = new PackageAPIExtractor();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            var packageDetails = await extractor.ExtractFromStream(responseStream);

            Approvals.VerifyJson(JsonSerializer.Serialize(packageDetails));
        }

        [Test]
        public async Task ApprovePackageWithNativeLibs()
        {
            var packageId = "boost_contract-vc110";
            var version = "1.71.0";

            var httpClient = new HttpClient();
            var url = $"https://api.nuget.org/v3-flatcontainer/{packageId}/{version}/{packageId}.{version}.nupkg";

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var extractor = new PackageAPIExtractor();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            var packageDetails = await extractor.ExtractFromStream(responseStream);

            Approvals.VerifyJson(JsonSerializer.Serialize(packageDetails));
        }
    }
}
