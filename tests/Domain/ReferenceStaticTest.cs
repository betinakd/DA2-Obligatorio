using Domain;

namespace Tests.Domain;

[TestClass]
public class ReferenceStaticTest
{
    [TestMethod]
    public void TestGetSimClass_ShouldReturnReference()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var referenceStatic = new ReferenceStatic { Reference = simClass };

        var result = referenceStatic.GetReferenceClass();

        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
        Assert.AreEqual("TestClass", result.Name);
    }

    [TestMethod]
    public void TestGetSignature_ShouldReturnCorrectStaticFormat()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var referenceStatic = new ReferenceStatic { Reference = simClass };
        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "param1" },
                new ParameterSignature { Name = "param2" }
            ]
        };

        var result = referenceStatic.GetSignature(signature);

        Assert.AreEqual("TestClass.TestMethod(param1, param2)", result);
    }

    [TestMethod]
    public void TestGetSignature_WithEmptyParameters_ShouldReturnCorrectFormat()
    {
        var simClass = new SimClass { Name = "StaticClass" };
        var referenceStatic = new ReferenceStatic { Reference = simClass };
        var signature = new Signature
        {
            Name = "EmptyMethod",
            Parameters = []
        };

        var result = referenceStatic.GetSignature(signature);

        Assert.AreEqual("StaticClass.EmptyMethod()", result);
    }

    [TestMethod]
    public void TestGetSignatureWithClassName_ShouldReturnCorrectFormat()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var referenceStatic = new ReferenceStatic { Reference = simClass };
        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x" },
                new ParameterSignature { Name = "y" }
            ]
        };

        var result = referenceStatic.GetSignatureWithClassName(signature);

        Assert.AreEqual("TestClass.TestMethod(x, y)", result);
    }

    [TestMethod]
    public void TestGetReferenceId_ShouldReturnReferenceId()
    {
        var expectedId = Guid.NewGuid();
        var simClass = new SimClass { Id = expectedId, Name = "TestClass" };
        var referenceStatic = new ReferenceStatic { Reference = simClass };

        var actualId = referenceStatic.GetReferenceId();

        Assert.AreEqual(expectedId, actualId);
    }

    [TestMethod]
    public void TestGetReferenceTypeDescription_ShouldReturnStatic()
    {
        var referenceStatic = new ReferenceStatic();

        var result = referenceStatic.GetReferenceTypeDescription();

        Assert.AreEqual("Static", result);
    }

    [TestMethod]
    public void GetInstanceClass_ShouldReturnReference()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var referenceStatic = new ReferenceStatic { Reference = simClass };
        var signature = new Signature();
        var executionInstance = new SimClass();

        var result = referenceStatic.GetInstanceClass(signature, executionInstance);

        Assert.AreEqual(simClass, result);
    }
}
