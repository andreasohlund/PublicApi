﻿using System.Collections.Generic;

namespace PublicAPI.APIExtraction
{
    public class PublicType
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public List<Method> Methods { get; set; }
    }
}