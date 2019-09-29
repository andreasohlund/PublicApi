using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace PublicAPI.Tests
{
    public class Class1
    {
        [Test]
        public async Task Extract()
        {
            var extractor = new PackageAPIExtractor();

            var packageDetails = await extractor.GetFromNugetFeed("nservicebus", "7.1.0");

            Console.WriteLine(packageDetails);
        }
    }
}
