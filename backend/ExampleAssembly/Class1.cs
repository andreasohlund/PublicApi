namespace ExampleAssembly
{
    public class SomePublicClass
    {
        public void PublicMethod() { }

        public void A_ShouldSortPublicMethodsByName() { }

        void PrivateMethodShouldNotBeIncluded(){ }
    }

    class PrivateClassShouldNotBeIncluded
    { }
}
