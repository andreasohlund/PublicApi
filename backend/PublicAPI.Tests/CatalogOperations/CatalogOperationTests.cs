namespace PublicAPI.Tests
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class CatalogOperationTests
    {
        [TestCase("5000")]
        public async Task ReadPageFromNuget(string cursorPosition)
        {
            var reader = new FeedCatalogReader();

            var page = await reader.ReadPageFromNuget(cursorPosition);

            foreach (var item in page.Items.Where(i => i.Type == "nuget:PackageDetails"))
            {
                Console.Out.WriteLine(item.PackageId + " " + item.Version);
            }
        }
    }
}
