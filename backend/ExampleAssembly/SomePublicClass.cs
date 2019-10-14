namespace ExampleAssembly
{
    public class SomePublicClass
    {
        public void PublicMethod() { }

        public void PublicMethodWithRefParam(ref object refParam) { }

        public void PublicMethodWithOutParam(out object outParam) {
            outParam = null;
        }

        public void PublicMethodWithParam(object param)
        {
        }

        public void PublicMethodWithOptionalParam(int optionalParam = 1)
        {
        }

        public void A_ShouldSortPublicMethodsByName() { }

        void PrivateMethodShouldNotBeIncluded(){ }
    }
}
