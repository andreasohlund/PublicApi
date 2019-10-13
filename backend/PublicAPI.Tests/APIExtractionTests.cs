using ApprovalTests.Reporters;

[assembly: UseReporter(typeof(DiffReporter))]

namespace PublicAPI.Tests
{
    using ApprovalTests;
    using NUnit.Framework;
    using PublicAPI.APIExtraction;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    [TestFixture]
    public class APIExtractionTests
    {
        [Test]
        public async Task ApproveAPIExtraction()
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
    }
}
