namespace PublicAPI.Tests
{
    using NUnit.Framework;
    using PublicAPI.CatalogOperations;
    using System;
    using System.Collections.Generic;
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

            using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            var packagesWithNetFxAsms = new List<PackageMetadata>();
            var totalCount = 0;
            try
            {
                await foreach (var package in reader.ReadPackageMetadata(page, tokenSource.Token)
                    .ConfigureAwait(false))
                {
                    totalCount++;
                    if (package.HasNetAssemblies)
                    {
                        packagesWithNetFxAsms.Add(package);
                        Console.Out.Write("!");
                    }
                    else
                    {
                        Console.Out.Write("~");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }

            Console.Out.WriteLine();
            Console.Out.WriteLine($"Packages with dotnet assemblies: {packagesWithNetFxAsms.Count} ({totalCount})");
            Console.Out.WriteLine($"Total download size(MB): {packagesWithNetFxAsms.Sum(p => p.Size) / 1000000.0}");
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
            Assert.AreEqual(3, packageMetadata.PackageEntries.Count);
            Assert.NotNull(packageMetadata.PackageEntries.Single(pe => pe.FullName == "lib/net35/TransmitSms.dll"));
            Assert.True(packageMetadata.HasNetAssemblies);
        }
    }
}
