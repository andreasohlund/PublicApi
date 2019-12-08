namespace PublicAPI.APIExtraction
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public class PackageAPIExtractor
    {
        public string Version { get; } = "0.3.0";

        public async Task<PackageDetails> ExtractFromStream(Stream stream)
        {
            var packageDetails = new PackageDetails();
            var assemblyApiExtractor = new AssemblyAPIExtractor();

            using var archive = new ZipArchive(stream);

            foreach (var entry in archive.Entries)
            {
                var path = WebUtility.UrlDecode(entry.FullName);
                if (path.StartsWith("lib/", StringComparison.InvariantCultureIgnoreCase) &&
                    (path.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) ||
                    path.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)))
                {
                    using var asmStream = entry.Open();
                    using var memStream = new MemoryStream();

                    //copy to a mem stream since cecil needs the stream to be seekable
                    await asmStream.CopyToAsync(memStream);

                    memStream.Position = 0;

                    var parts = path.Split("/");
                    var tfm = parts[1].ToLower();
                    var targetFramework = packageDetails.TargetFrameworks.SingleOrDefault(tf => tf.Name == tfm);

                    if (targetFramework == null)
                    {
                        targetFramework = new TargetFramework
                        {
                            Name = tfm,
                            Assemblies = new List<Assembly>()
                        };

                        packageDetails.TargetFrameworks.Add(targetFramework);
                    }

                    var assembly = await assemblyApiExtractor.ExtractFromStream(memStream);

                    if (assembly.Name == null)
                    {
                        var assemblyName = parts.Last();

                        assembly.Name = assemblyName;
                    }

                    targetFramework.Assemblies.Add(assembly);
                }
            }

            return packageDetails;
        }
    }
}
