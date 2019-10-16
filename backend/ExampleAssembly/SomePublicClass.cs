using System;

namespace ExampleAssembly
{
    public class SomePublicClass
    {
        public void PublicMethod() { }

        public void PublicMethodWithRefParam(ref object refParam) { }

        public void PublicMethodWithOutParam(out object outParam)
        {
            outParam = null;
        }

        public void PublicMethodWithParam(object param)
        {
        }

        public void PublicMethodWithOptionalParam(int optionalParam = 1)
        {
        }

        public void A_ShouldSortPublicMethodsByName() { }

        public int GetSetProperty { get; set; }

        public int GetOnlyProperty { get; }

        public int SetOnlyProperty { set { throw new NotImplementedException(); } }

        void NonPublicMethodShouldNotBeIncluded() { }

        int NonPublicPropertyShouldNotBeIncluded { get; set; }
    }
}
