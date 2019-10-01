namespace PublicAPI.Tests
{
    using System.Collections.Generic;

    public class TargetFramework
    {
        public string Name { get; set; }
        public List<PublicType> PublicTypes { get; internal set; }

        public override string ToString()
        {
            return Name;
        }

    }
}
