using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class ReferenceVariableTest
{
    [TestMethod]
    public void TestGetSimClass_ShouldReturnParameterType()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var localVariable = new LocalVariable { Type = simClass };
        var referencelocalVariable = new ReferenceVariable { Reference = localVariable };

        var result = referencelocalVariable.GetSimClass();

        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
        Assert.AreEqual("TestClass", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void TestGetSimClass_ShouldThrowExceptionWhenTypeIsNull()
    {
        var localVariable = new LocalVariable();
        var referenceVariable = new ReferenceVariable { Reference = localVariable };

        referenceVariable.GetSimClass();
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

        var referenceVariable = new ReferenceVariable { Reference = null };

        var result = referenceVariable.GetSignature(signature);

        Assert.AreEqual("this.TestMethod(param1, param2)", result);
    }
}
