using System;
using Xunit;

namespace Xunit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestAttribute : FactAttribute
    {
        public TestAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseAttribute : Attribute
    {
        public object[] Arguments { get; }
        public TestCaseAttribute(params object[] args) { Arguments = args; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TestFixtureAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SetUpAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TearDownAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OneTimeSetUpAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OneTimeTearDownAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CategoryAttribute : Attribute
    {
        public string Name { get; }
        public CategoryAttribute(string name) { Name = name; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestFixtureSetUpAttribute : OneTimeSetUpAttribute { }
}
