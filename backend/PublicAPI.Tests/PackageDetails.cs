namespace PublicAPI.Tests
{
    using System.Collections.Generic;
    
    public class PackageDetails
    {
        public PackageDetails()
        {
            TargetFrameworks = new List<TargetFramework>();
        }

        public List<TargetFramework> TargetFrameworks { get; set; }
        public string PackageId { get; internal set; }
        public string Version { get; internal set; }
        public string ApiExtractorVersion { get; internal set; }

        public override string ToString()
        {
            var fxs = string.Join(";", TargetFrameworks);

            return $"{PackageId}-{Version}: ({fxs})";
        }
    }
}
