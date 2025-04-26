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
        var attribute = new SimAttribute { Type = simClass };
        var referenceAttribute = new ReferenceAttribute { Reference = attribute };

        var result = referenceAttribute.GetSimClass();

        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
        Assert.AreEqual("TestClass", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
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
                new Parameter { Name = "param1" },
                new Parameter { Name = "param2" }
            ]
        };

        var referenceAttribute = new ReferenceAttribute();

        var result = referenceAttribute.GetSignature(signature);

        Assert.AreEqual("this.TestMethod(param1, param2)", result);
    }
}