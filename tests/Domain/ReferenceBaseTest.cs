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

        var result = referenceBase.GetSimClass();

        Assert.IsNotNull(result);
        Assert.AreEqual("BaseClass", result.Name);
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

        var referenceBase = new ReferenceBase();

        var result = referenceBase.GetSignature(signature);

        Assert.AreEqual("base.TestMethod(param1, param2)", result);
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void TestGetSimClass_ShouldThrowExceptionWhenBaseClassIsNull()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var referenceBase = new ReferenceBase() { Reference = simClass };

        referenceBase.GetSimClass();
    }

    [TestMethod]
    public void TestGetSignatureWithClassNameBase_ShouldReturnCorrectFormat()
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
        var baseClass = new SimClass { Name = "BaseClass" };
        var simClass = new SimClass { Name = "ChildClass", BaseClass = baseClass, BaseClassId = baseClass.Id };
        var referenceBase = new ReferenceBase { Reference = simClass };

        var result = referenceBase.GetSignatureWithClassName(signature);

        Assert.AreEqual("BaseClass.MyMethod(x, y)", result);
    }
}
