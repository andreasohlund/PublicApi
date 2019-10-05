namespace PublicAPI.Tests
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class CatalogOperationTests
    {
        [TestCase("5000")]
        public async Task ReadPageFromNuget(string pageNumber)
        {
            var url = $"https://api.nuget.org/v3/catalog0/page{pageNumber}.json";

            var reader = new CatalogPageReader(new HttpClient());

            var page = await reader.ReadUrl(url);

            foreach (var item in page.Items.Where(i => i.Type == "nuget:PackageDetails"))
            {
                Console.Out.WriteLine(item.PackageId + " " + item.Version);
            }
        }

        [Test]
        public async Task ParseNuGetCatalogIndex()
        {
            var cursor = DateTime.UtcNow - TimeSpan.FromDays(1);
            var reader = new CatalogIndexReader(new HttpClient());

            var catalogIndex = await reader.ReadUrl("https://api.nuget.org/v3/catalog0/index.json");

            Console.Out.WriteLine($"All pages since: {cursor}");

            foreach (var page in catalogIndex.Items.Where(i => i.CommitTimeStamp > cursor).OrderByDescending(i => i.CommitTimeStamp))
            {
                Console.Out.WriteLine($"{page.CommitTimeStamp} - {page.Id} ({page.Count})");
            }
        }
    }
}
