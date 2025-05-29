using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class ReferenceAttributeTest
{
    [TestMethod]
    public void TestGetSimClass_ShouldReturnAttributeType()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var attribute = new SimAttribute { Reference = simClass, Name = "Atri" };
        var referenceAttribute = new ReferenceAttribute { Reference = attribute };

        var result = referenceAttribute.GetSimClass();

        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
        Assert.AreEqual("TestClass", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void TestGetSimClass_ShouldThrowExceptionWhenTypeIsNull()
    {
        var attribute = new SimAttribute();
        var referenceAttribute = new ReferenceAttribute { Reference = attribute };

        referenceAttribute.GetSimClass();
    }

    [TestMethod]
    public void TestGetSignature_ShouldReturnThisSignatureFormat()
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
        var attribute = new SimAttribute { Reference = simClass, Name = "Atri" };
        var reference = new ReferenceAttribute { Reference = attribute };
        var result = reference.GetSignature(signature);

        Assert.AreEqual("Atri.TestMethod(param1, param2)", result);
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
        var attribute = new SimAttribute { Reference = simClass, Name = "Attr" };
        var reference = new ReferenceAttribute { Reference = attribute };

        var result = reference.GetSignatureWithClassName(signature);

        Assert.AreEqual("MyClass.MyMethod(x, y)", result);
    }

    [TestMethod]
    public void TestGetReferenceId_ShouldReturnReferenceId()
    {
        var expectedId = Guid.NewGuid();
        var attribute = new SimAttribute { Id = expectedId, Name = "Attr" };
        var reference = new ReferenceAttribute { Reference = attribute };

        var actualId = reference.GetReferenceId();

        Assert.AreEqual(expectedId, actualId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void TestReference_ShouldThrowExceptionWhenStaticAttribute()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var staticAttribute = new SimAttribute
        {
            Reference = simClass,
            Name = "StaticAttr",
            IsStatic = true
        };
        var reference = new ReferenceAttribute();

        reference.Reference = staticAttribute;
    }
}
