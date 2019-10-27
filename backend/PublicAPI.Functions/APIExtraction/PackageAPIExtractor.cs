namespace PublicAPI.APIExtraction
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public class PackageAPIExtractor
    {
        public string Version { get; } = "0.2.0";

        public async Task<PackageDetails> ExtractFromStream(Stream stream)
        {
            var packageDetails = new PackageDetails();
            var assemblyApiExtractor = new AssemblyAPIExtractor();

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

                    var tfm = path.Split("/")[1].ToLower();

                    var targetFramework = packageDetails.TargetFrameworks.SingleOrDefault(tf => tf.Name == tfm);

                    if (targetFramework == null)
                    {
                        targetFramework = new TargetFramework
                        {
                            Name = tfm,
                            PublicTypes = new List<PublicType>()
                        };

                        packageDetails.TargetFrameworks.Add(targetFramework);
                    }

                    try
                    {
                        var publicTypes = await assemblyApiExtractor.ExtractFromStream(memStream);

                        targetFramework.PublicTypes.AddRange(publicTypes);
                    }
                    catch (System.BadImageFormatException)
                    {
                        targetFramework.HasNativeLibs = true;
                    }
                }
            }

            return packageDetails;
        }
    }
}
