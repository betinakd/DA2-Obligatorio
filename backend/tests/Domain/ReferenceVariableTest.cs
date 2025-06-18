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
        var localVariable = new LocalVariable { Reference = simClass, Name = "Variable" };
        var referencelocalVariable = new ReferenceVariable { Reference = localVariable };

        var result = referencelocalVariable.GetReferenceClass();

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

        referenceVariable.GetReferenceClass();
    }

    [TestMethod]
    public void TestGetSignature_ShouldReturnThisSignatureFormat()
    {
        var referenceClass1 = new SimClass { Name = "RefClass1" };
        var instanceClass1 = new SimClass { Name = "InstClass1" };
        var referenceClass2 = new SimClass { Name = "RefClass2" };
        var instanceClass2 = new SimClass { Name = "InstClass2" };

        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature
            {
                Name = "param1",
                Reference = referenceClass1,
                Instance = instanceClass1
            },
            new ParameterSignature
            {
                Name = "param2",
                Reference = referenceClass2,
                Instance = instanceClass2
            }

            ],
        };

        var referenceVariable = new ReferenceVariable
        {
            Reference = new LocalVariable
            {
                Reference = new SimClass { Name = "TestClass" },
                Name = "Variable"
            }
        };

        var result = referenceVariable.GetSignature(signature);

        Assert.AreEqual("Variable.TestMethod(param1: RefClass1 InstClass1, param2: RefClass2 InstClass2)", result);
    }

    [TestMethod]
    public void TestGetSignatureWithClassNameVariable_ShouldReturnCorrectFormat()
    {
        var referenceClassX = new SimClass { Name = "RefClassX" };
        var instanceClassX = new SimClass { Name = "InstClassX" };
        var referenceClassY = new SimClass { Name = "RefClassY" };
        var instanceClassY = new SimClass { Name = "InstClassY" };

        var signature = new Signature
        {
            Name = "MyMethod",
            Parameters =
            [
                new ParameterSignature
            {
                Name = "x",
                Reference = referenceClassX,
                Instance = instanceClassX
            },
            new ParameterSignature
            {
                Name = "y",
                Reference = referenceClassY,
                Instance = instanceClassY
            }

            ],
        };
        var simClass = new SimClass { Name = "MyClass" };
        var localVariable = new LocalVariable { Reference = simClass, Name = "Variable" };
        var referenceVariable = new ReferenceVariable { Reference = localVariable };

        var result = referenceVariable.GetSignatureWithClassName(signature);

        Assert.AreEqual("MyClass.MyMethod(x: RefClassX InstClassX, y: RefClassY InstClassY)", result);
    }

    [TestMethod]
    public void TestGetReferenceId_ShouldReturnVariableId()
    {
        var expectedId = Guid.NewGuid();
        var localVariable = new LocalVariable
        {
            Id = expectedId,
            Name = "Variable",
            Reference = new SimClass { Name = "TestClass" },
        };
        var referenceVariable = new ReferenceVariable { Reference = localVariable };

        var actualId = referenceVariable.GetReferenceId();

        Assert.AreEqual(expectedId, actualId);
    }

    [TestMethod]
    public void GetInstanceClass_ShouldReturnReferenceInstance()
    {
        var expectedInstance = new SimClass { Name = "InstanceClass" };
        var referenceClass = new SimClass { Name = "ReferenceClass" };
        var localVariable = new LocalVariable
        {
            Name = "var1",
            Reference = referenceClass,
            Instance = expectedInstance,
        };
        var referenceVariable = new ReferenceVariable
        {
            Reference = localVariable,
        };

        var signature = new Signature();
        var executionInstance = new SimClass();

        var result = referenceVariable.GetInstanceClass(signature, executionInstance);

        Assert.AreEqual(expectedInstance, result);
    }
}
