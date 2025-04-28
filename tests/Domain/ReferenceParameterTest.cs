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
    [ExpectedException(typeof(SimClassInvalidAttribute))]
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
}
