namespace PublicAPI.Tests
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class CatalogOperationTests
    {
        [TestCase(5000, 100)]
        public async Task ReadAllPackageMetadataFromCatalogPage(int pageNumber, int cancelAfter = int.MaxValue)
        {
            var url = $"https://api.nuget.org/v3/catalog0/page{pageNumber}.json";

            var reader = new CatalogPageReader(new HttpClient());

            var page = await reader.ReadUrl(url);

            Console.Out.WriteLine($"Reading metadata for {url}");

            using var tokenSource = new CancellationTokenSource();
            var count = 0;

            long totalSize = 0;

            foreach (var item in await reader.ReadPackageMetadata(page, tokenSource.Token))
            {
                count++;
                totalSize += item.Size;
                if (count > cancelAfter)
                {
                    tokenSource.Cancel();
                }

                Console.Out.WriteLine(item.Size);
            }

            Console.Out.WriteLine($"Read {count} out of {page.Items.Count}");
            Console.Out.WriteLine($"Total package size {totalSize}");
        }

        [Test]
        public async Task ParseCatalogIndex()
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

        [Test]
        public async Task ParsePackageMetadata()
        {
            var url = "https://api.nuget.org/v3/catalog0/data/2018.11.24.05.52.45/transmitsms.2.0.11.json";

            var reader = new PackageMetadataReader(new HttpClient());

            var packageMetadata = await reader.ReadUrl(url);

            Assert.AreEqual("TransmitSms", packageMetadata.Id);
            Assert.AreEqual("2.0.11", packageMetadata.Version);
            Assert.AreEqual(29774, packageMetadata.Size);
        }
    }
}
