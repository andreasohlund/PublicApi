using NUnit.Framework;
using PublicAPI.CatalogOperations;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PublicAPI.Tests
{
    public class CatalogOperationTests
    {
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
            Assert.AreEqual(3, packageMetadata.PackageEntries.Count);
            Assert.NotNull(packageMetadata.PackageEntries.Single(pe => pe.FullName == "lib/net35/TransmitSms.dll"));
            Assert.True(packageMetadata.HasNetAssemblies);
        }
    }
}
