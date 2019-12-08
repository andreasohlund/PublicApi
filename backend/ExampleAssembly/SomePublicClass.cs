using System;

namespace ExampleAssembly
{
    public class SomePublicClass : SomeBaseClass, ISomeInterface, ISomeOtherInterface
    {
        public void PublicMethod() { }

        public static void StaticPublicMethod() { }

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

        public static int StaticProperty { get; set; }

        public int GetOnlyProperty { get; }

        public int SetOnlyProperty { set { throw new NotImplementedException(); } }

        public int PublicField;

        public static int StaticField;

        void NonPublicMethodShouldNotBeIncluded() { }

        int NonPublicPropertyShouldNotBeIncluded { get; set; }

        int NonPublicFieldShouldNotBeIncluded;
    }

    public interface ISomeOtherInterface
    {
    }

    public interface ISomeInterface
    {
    }

    public class SomeBaseClass
    {
    }

    public abstract class SomeAbstractClass
    {
    }

    public static class SomeStaticClass
    {
    }

}
