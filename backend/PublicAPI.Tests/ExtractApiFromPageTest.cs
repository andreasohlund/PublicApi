namespace PublicAPI.Tests
{
    using NUnit.Framework;
    using System.Linq;
    using System.Threading.Tasks;

    public class ExtractApiFromPageTest
    {
        [TestCase("5000")]
        public async Task ReadPageFromNuget(string pageNumber)
        {
            var reader = new FeedCatalogReader();

            var page = await reader.ReadPageFromNuget(pageNumber);

            var apiExtractor = new PackageAPIExtractorTests();
            foreach (var item in page.Items.Where(i => i.Type == "nuget:PackageDetails"))
            {
                await apiExtractor.ExtractFromNuGetFeed(item.PackageId,item.Version);
            }
        }
    }
}
