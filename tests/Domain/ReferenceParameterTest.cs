using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class ReferenceParameterTest
{
    [TestMethod]
    public void TestGetSimClass_ShouldReturnParameterType()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var parameter = new Parameter { Type = simClass, Name = "Param" };
        var referenceParameter = new ReferenceParameter { Reference = parameter };

        var result = referenceParameter.GetSimClass();

        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
        Assert.AreEqual("TestClass", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void TestGetSimClass_ShouldThrowExceptionWhenTypeIsNull()
    {
        var parameter = new Parameter();
        var referenceParameter = new ReferenceParameter { Reference = parameter };

        referenceParameter.GetSimClass();
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
        var referenceParameter = new ReferenceParameter() { Reference = new Parameter { Type = new SimClass { Name = "TestClass" }, Name = "Param" } };

        var result = referenceParameter.GetSignature(signature);

        Assert.AreEqual("Param.TestMethod(param1, param2)", result);
    }

    [TestMethod]
    public void TestGetSignatureWithClassNameParameter_ShouldReturnCorrectFormat()
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
        var parameter = new Parameter { Type = simClass, Name = "Param" };
        var referenceParameter = new ReferenceParameter { Reference = parameter };

        var result = referenceParameter.GetSignatureWithClassName(signature);

        Assert.AreEqual("MyClass.MyMethod(x, y)", result);
    }

    [TestMethod]
    public void TestGetReferenceId_ShouldReturnParameterId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = expectedId,
            Name = "Param",
            Type = new SimClass { Name = "TestClass" }
        };
        var referenceParameter = new ReferenceParameter { Reference = parameter };

        var actualId = referenceParameter.GetReferenceId();

        Assert.AreEqual(expectedId, actualId);
    }
}
