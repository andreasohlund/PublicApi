﻿namespace PublicAPI.APIExtraction
{
    using System.Collections.Generic;

    public class TargetFramework
    {
        public string Name { get; set; }
        public List<Assembly> Assemblies { get; set; }
    }
}
