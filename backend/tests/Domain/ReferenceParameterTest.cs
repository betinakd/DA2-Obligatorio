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
        var parameter = new Parameter { Reference = simClass, Name = "Param" };
        var referenceParameter = new ReferenceParameter { Reference = parameter };

        var result = referenceParameter.GetReferenceClass();

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

        referenceParameter.GetReferenceClass();
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

        var referenceParameter = new ReferenceParameter
        {
            Reference = new Parameter
            {
                Reference = new SimClass { Name = "TestClass" },
                Name = "Param"
            }
        };

        var result = referenceParameter.GetSignature(signature);

        Assert.AreEqual("Param.TestMethod(param1: RefClass1 InstClass1, param2: RefClass2 InstClass2)", result);
    }

    [TestMethod]
    public void TestGetSignatureWithClassNameParameter_ShouldReturnCorrectFormat()
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
        var parameter = new Parameter { Reference = simClass, Name = "Param" };
        var referenceParameter = new ReferenceParameter { Reference = parameter };

        var result = referenceParameter.GetSignatureWithClassName(signature);

        Assert.AreEqual("MyClass.MyMethod(x: RefClassX InstClassX, y: RefClassY InstClassY)", result);
    }

    [TestMethod]
    public void TestGetReferenceId_ShouldReturnParameterId()
    {
        var expectedId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = expectedId,
            Name = "Param",
            Reference = new SimClass { Name = "TestClass" },
        };
        var referenceParameter = new ReferenceParameter { Reference = parameter };

        var actualId = referenceParameter.GetReferenceId();

        Assert.AreEqual(expectedId, actualId);
    }

    [TestMethod]
    public void GetInstanceClass_ShouldReturnParameterInstance_WhenParameterIndexMatches()
    {
        var expectedInstance = new SimClass { Name = "InstanceClass" };
        var typeClass = new SimClass { Name = "TypeClass" };
        var parameter = new Parameter { Name = "param1", Index = 0, Reference = typeClass };
        var referenceParameter = new ReferenceParameter { Reference = parameter };

        var signature = new Signature
        {
            Parameters = [
                new ParameterSignature { Index = 0, Instance = expectedInstance }
            ],
        };
        var executionInstance = new SimClass();

        var result = referenceParameter.GetInstanceClass(signature, executionInstance);

        Assert.AreEqual(expectedInstance, result);
    }

    [TestMethod]
    public void GetInstanceClass_ShouldReturnReferenceType_WhenParameterIndexNotFound()
    {
        var typeClass = new SimClass { Name = "TypeClass" };
        var parameter = new Parameter { Name = "param1", Index = 1, Reference = typeClass };
        var referenceParameter = new ReferenceParameter { Reference = parameter };

        var signature = new Signature
        {
            Parameters = [
                new ParameterSignature { Index = 0, Instance = new SimClass() }
            ],
        };
        var executionInstance = new SimClass();

        var result = referenceParameter.GetInstanceClass(signature, executionInstance);

        Assert.AreEqual(typeClass, result);
    }
}
