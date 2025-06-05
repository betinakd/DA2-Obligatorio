using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class ReferenceStaticAttributeTest
{
    [TestMethod]
    public void TestGetSimClass_ShouldReturnAttributeType()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var attribute = new SimAttribute { Reference = simClass, Name = "StaticAttr", IsStatic = true };
        var referenceStaticAttribute = new ReferenceStaticAttribute { Reference = attribute };

        var result = referenceStaticAttribute.GetReferenceClass();

        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
        Assert.AreEqual("TestClass", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void TestGetSimClass_ShouldThrowExceptionWhenTypeIsNull()
    {
        var attribute = new SimAttribute { IsStatic = true };
        var referenceStaticAttribute = new ReferenceStaticAttribute { Reference = attribute };

        referenceStaticAttribute.GetReferenceClass();
    }

    [TestMethod]
    public void TestGetSignature_ShouldReturnStaticSignatureFormat()
    {
        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "param1" },
                new ParameterSignature { Name = "param2" }
            ]
        };
        var simClass = new SimClass { Name = "TestClass" };
        var attribute = new SimAttribute { Reference = simClass, Name = "StaticAttr", IsStatic = true };
        var reference = new ReferenceStaticAttribute { Reference = attribute };

        var result = reference.GetSignature(signature);

        Assert.AreEqual("StaticAttr.TestMethod(param1, param2)", result);
    }

    [TestMethod]
    public void TestGetSignatureWithClassName_ShouldReturnCorrectFormat()
    {
        var signature = new Signature
        {
            Name = "MyMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x" },
                new ParameterSignature { Name = "y" }
            ]
        };
        var simClass = new SimClass { Name = "MyClass" };
        var attribute = new SimAttribute { Reference = simClass, Name = "StaticAttr", IsStatic = true };
        var reference = new ReferenceStaticAttribute { Reference = attribute };

        var result = reference.GetSignatureWithClassName(signature);

        Assert.AreEqual("MyClass.MyMethod(x, y)", result);
    }

    [TestMethod]
    public void TestGetReferenceId_ShouldReturnReferenceId()
    {
        var expectedId = Guid.NewGuid();
        var attribute = new SimAttribute { Id = expectedId, Name = "StaticAttr", IsStatic = true };
        var reference = new ReferenceStaticAttribute { Reference = attribute };

        var actualId = reference.GetReferenceId();

        Assert.AreEqual(expectedId, actualId);
    }

    [TestMethod]
    public void TestGetReferenceTypeDescription_ShouldReturnAttribute()
    {
        var reference = new ReferenceStaticAttribute();

        var result = reference.GetReferenceTypeDescription();

        Assert.AreEqual("StaticAttribute", result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void TestReference_ShouldThrowExceptionWhenNonStaticAttribute()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var nonStaticAttribute = new SimAttribute
        {
            Reference = simClass,
            Name = "NonStaticAttr",
            IsStatic = false
        };
        var reference = new ReferenceStaticAttribute();

        reference.Reference = nonStaticAttribute;
    }
}
