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
        var localVariable = new LocalVariable { Type = simClass, Name = "Variable" };
        var referencelocalVariable = new ReferenceVariable { Reference = localVariable };

        var result = referencelocalVariable.GetSimClass();

        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
        Assert.AreEqual("TestClass", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
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

        var referenceVariable = new ReferenceVariable() { Reference = new LocalVariable { Type = new SimClass { Name = "TestClass" }, Name = "Variable" } };

        var result = referenceVariable.GetSignature(signature);

        Assert.AreEqual("Variable.TestMethod(param1, param2)", result);
    }

    [TestMethod]
    public void TestGetSignatureWithClassNameVariable_ShouldReturnCorrectFormat()
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
        var localVariable = new LocalVariable { Type = simClass, Name = "Variable" };
        var referenceVariable = new ReferenceVariable { Reference = localVariable };

        var result = referenceVariable.GetSignatureWithClassName(signature);

        Assert.AreEqual("MyClass.MyMethod(x, y)", result);
    }
}
