using System.Collections.Generic;

namespace PublicAPI.APIExtraction
{
    public class Method
    {
        public string Name { get; set; }

        public string ReturnType { get; set; }

        public List<Parameter> Parameters { get; set; }
        public bool IsStatic { get; set; }
    }
}