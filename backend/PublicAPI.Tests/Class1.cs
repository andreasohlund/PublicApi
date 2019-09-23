using NUnit.Framework;
using System;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace PublicAPI.Tests
{
    public class Class1
    {
        [Test]
        public async Task Testit()
        {
            var client = new HttpClient();

            var response = await client.GetAsync("https://api.nuget.org/v3-flatcontainer/newtonsoft.json/9.0.1/newtonsoft.json.9.0.1.nupkg");
            response.EnsureSuccessStatusCode();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (var archive = new ZipArchive(responseStream))
            {
                foreach (var entry in archive.Entries)
                {
                    Console.WriteLine(entry.FullName);
                }
            }
      
        }

    }
}
