﻿namespace PublicAPI.APIExtraction
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool HasDefault { get; set; }
        public string Modifier { get; set; }
    }
}