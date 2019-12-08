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
        public string BaseType { get; set; }
        public bool IsStatic { get; set; }
        public bool IsEnum { get; set; }
        public bool IsClass { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsSealed { get; set; }
        public bool IsValueType { get; set; }
    }
}