namespace PublicAPI.APIExtraction
{
    using System.Collections.Generic;

    public class PackageDetails
    {
        public PackageDetails()
        {
            TargetFrameworks = new List<TargetFramework>();
        }

        public List<TargetFramework> TargetFrameworks { get; set; }

        public string ApiExtractorVersion { get; set; }
    }
}
