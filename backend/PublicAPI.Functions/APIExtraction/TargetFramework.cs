namespace PublicAPI.APIExtraction
{
    using System.Collections.Generic;

    public class TargetFramework
    {
        public string Name { get; set; }
        public List<PublicType> PublicTypes { get; set; }
        public bool HasNativeLibs { get; set; }
    }
}
