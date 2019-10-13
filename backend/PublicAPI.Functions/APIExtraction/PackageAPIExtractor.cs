namespace PublicAPI.APIExtraction
{
    using Mono.Cecil;
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public class PackageAPIExtractor
    {
        public async Task<PackageDetails> ExtractFromStream(Stream stream)
        {
            var packageDetails = new PackageDetails();

            using var archive = new ZipArchive(stream);

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
                        Name = path.Split("/")[1],
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
