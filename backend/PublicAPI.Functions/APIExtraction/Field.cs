namespace PublicAPI.APIExtraction
{
    public class Field
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsStatic { get; internal set; }
    }
}