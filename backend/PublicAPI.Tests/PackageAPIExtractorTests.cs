namespace PublicAPI.Tests
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class PackageAPIExtractorTests
    {
        [TestCase("nservicebus", "7.1.0")]
        [TestCase("nservicebus", "7.0.0")]
        public async Task ExtractFromNuGetFeed(string packageId, string version)
        {
            var client = new HttpClient();
            var url = $"https://api.nuget.org/v3-flatcontainer/{packageId}/{version}/{packageId}.{version}.nupkg";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var extractor = new PackageAPIExtractor();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            var packageDetails = await extractor.ExtractFromStream(responseStream);

            Console.WriteLine(string.Join(";", packageDetails.TargetFrameworks.Select(fx =>fx.Name)));
        }
    }
}
