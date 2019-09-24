using Mono.Cecil;
using NUnit.Framework;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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
                    var path = WebUtility.UrlDecode(entry.FullName).ToLower();
                    if (path.StartsWith("lib/") && path.EndsWith(".dll"))
                    {
                        using (var asmStream = entry.Open())
                        using (var memStream = new MemoryStream())
                        {
                            //copy to a mem stream since cecil needs the stream to be seekable
                            await asmStream.CopyToAsync(memStream);

                            memStream.Position = 0;
                            var assembly = AssemblyDefinition.ReadAssembly(memStream);

                            var publicTypes = assembly.Modules.SelectMany(m => m.GetTypes())
                                .Where(t => !t.IsNested && ShouldIncludeType(t))
                                .OrderBy(t => t.FullName, StringComparer.Ordinal);

                            Console.WriteLine(path + " - " + publicTypes.Count());
                        }
                            
                    }
                    
                }
            }
      
        }

        static bool ShouldIncludeType(TypeDefinition t)
        {
            return (t.IsPublic || t.IsNestedPublic || t.IsNestedFamily) && !IsCompilerGenerated(t);
        }

        static bool IsCompilerGenerated(IMemberDefinition m)
        {
            return m.CustomAttributes.Any(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
        }

    }
}
