using Mono.Cecil;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PublicAPI.Tests
{
    class PackageAPIExtractor
    {
        public async Task<PackageDetails> GetFromNugetFeed(string packageId, string version)
        {
            var client = new HttpClient();
            var url = $"https://api.nuget.org/v3-flatcontainer/{packageId}/{version}/{packageId}.{version}.nupkg";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var packageDetails = new PackageDetails
            {
                PackageId = packageId,
                Version = version,
                ApiExtractorVersion = "1.0.0"
            };

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var archive = new ZipArchive(responseStream);

            foreach (var entry in archive.Entries)
            {
                var path = WebUtility.UrlDecode(entry.FullName).ToLower();
                if (path.StartsWith("lib/") && path.EndsWith(".dll"))
                {
                    using var asmStream = entry.Open();
                    using var memStream = new MemoryStream();

                    //copy to a mem stream since cecil needs the stream to be seekable
                    await asmStream.CopyToAsync(memStream);

                    memStream.Position = 0;
                    using var assembly = AssemblyDefinition.ReadAssembly(memStream);

                    var publicTypes = assembly.Modules.SelectMany(m => m.GetTypes())
                        .Where(t => !t.IsNested && ShouldIncludeType(t))
                        .OrderBy(t => t.FullName, StringComparer.Ordinal)
                        .Select(ti => ConvertTypeInfoToPublicTypeDTO(ti))
                        .ToList();

                    packageDetails.TargetFrameworks.Add(new TargetFramework
                    {
                        Name = path.Split("/").First(),
                        PublicTypes = publicTypes
                    });
                }
            }

            return packageDetails;
        }

        PublicType ConvertTypeInfoToPublicTypeDTO(TypeDefinition typeDefinition)
        {
            return new PublicType
            {
                Name = typeDefinition.Name
            };
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
