using System.Collections.Generic;

namespace PublicAPI.APIExtraction
{
    public class PublicType
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public List<Method> Methods { get; set; }
        public List<Property> Properties { get; set; }
        public List<Field> Fields { get; set; }
        public bool IsInterface { get; set; }
        public List<string> Implements { get; set; }
        public string BaseType { get; internal set; }
    }
}