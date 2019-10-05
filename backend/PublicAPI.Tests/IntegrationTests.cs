namespace PublicAPI.Tests
{
    using NUnit.Framework;
    using System.Threading.Tasks;

    public class IntegrationTests
    {
        [TestCase("5000")]
        public async Task ExtractApiFromAllPackagesInCatalogPage(string pageNumber)
        {
            //var reader = new CatalogPageReader();

            //var page = await reader.ReadPageFromNuget(pageNumber);

            //var apiExtractor = new PackageAPIExtractorTests();
            //foreach (var item in page.Items.Where(i => i.Type == "nuget:PackageDetails").Take(10))
            //{
            //    await apiExtractor.ExtractFromNuGetFeed(item.PackageId,item.Version);
            //}
        }
    }
}
