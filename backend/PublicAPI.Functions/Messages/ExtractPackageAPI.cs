namespace PublicAPI.Messages
{
    public class ExtractPackageAPI
    {
        public string PackageId { get; set; }
        public string PackageVersion { get; set; }
        public bool HasDotNetAssemblies { get; set; }
    }
}
