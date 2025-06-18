using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class ReferenceBaseTest()
{
    [TestMethod]
    public void TestGetSimClass_ShouldReturnReferenceProperty()
    {
        var baseClass = new SimClass { Name = "BaseClass" };

        var simClass = new SimClass { Name = "TestClass", BaseClassId = baseClass.Id, BaseClass = baseClass };

        var referenceBase = new ReferenceBase() { Reference = simClass };

        var result = referenceBase.GetReferenceClass();

        Assert.IsNotNull(result);
        Assert.AreEqual("BaseClass", result.Name);
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

        var referenceBase = new ReferenceBase();

        var result = referenceBase.GetSignature(signature);

        Assert.AreEqual("base.TestMethod(param1: RefClass1 InstClass1, param2: RefClass2 InstClass2)", result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void TestGetSimClass_ShouldThrowExceptionWhenBaseClassIsNull()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var referenceBase = new ReferenceBase() { Reference = simClass };

        referenceBase.GetReferenceClass();
    }

    [TestMethod]
    public void TestGetSignatureWithClassNameBase_ShouldReturnCorrectFormat()
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

        var baseClass = new SimClass { Name = "BaseClass" };
        var simClass = new SimClass { Name = "ChildClass", BaseClass = baseClass, BaseClassId = baseClass.Id };
        var referenceBase = new ReferenceBase { Reference = simClass };

        var result = referenceBase.GetSignatureWithClassName(signature);

        Assert.AreEqual("BaseClass.MyMethod(x: RefClassX InstClassX, y: RefClassY InstClassY)", result);
    }

    [TestMethod]
    public void TestGetReferenceId_ShouldReturnBaseClassId()
    {
        var expectedId = Guid.NewGuid();
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        var simClass = new SimClass
        {
            Id = expectedId,
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };

        var referenceBase = new ReferenceBase { Reference = simClass };

        var actualId = referenceBase.GetReferenceId();

        Assert.AreEqual(expectedId, actualId);
    }

    [TestMethod]
    public void GetInstanceClass_ShouldReturnBaseClass()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        var simClass = new SimClass { Name = "ChildClass", BaseClass = baseClass, BaseClassId = baseClass.Id };
        var referenceBase = new ReferenceBase { Reference = simClass };
        var signature = new Signature();
        var executionInstance = new SimClass();

        var result = referenceBase.GetInstanceClass(signature, executionInstance);

        Assert.AreEqual(baseClass, result);
    }
}
