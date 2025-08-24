// Lightweight global shim to allow remaining NUnit-style attributes to compile while migrating to xUnit.
using System;
using Xunit;

// Define attributes in global namespace so bare [Test] or [SetUp] work without 'using' directives.
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestAttribute : FactAttribute
{
    public TestAttribute() { }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SetUpAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TearDownAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TestFixtureAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute : Attribute
{
    public object[] Arguments { get; }
    public TestCaseAttribute(params object[] args) { Arguments = args; }
}

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
