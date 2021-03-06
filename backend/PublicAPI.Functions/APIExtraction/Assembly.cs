﻿namespace PublicAPI.APIExtraction
{
    using System.Collections.Generic;

    public class Assembly
    {
        public string Name { get; set; }
        public List<PublicType> PublicTypes { get; set; }
        public bool IsNative { get; set; }
        public string Version { get; set; }
        public byte[] PublicKey { get; set; }
    }
}
