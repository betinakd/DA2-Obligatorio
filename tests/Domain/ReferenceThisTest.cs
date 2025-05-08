using Domain;

namespace Tests.Domain;

[TestClass]
public class ReferenceThisTest()
{
    [TestMethod]
    public void TestGetSimClass_ShouldReturnReferenceProperty()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var referenceThis = new ReferenceThis() { Reference = simClass };

        var result = referenceThis.GetSimClass();

        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
        Assert.AreEqual("TestClass", result.Name);
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

        var referenceThis = new ReferenceThis();

        var result = referenceThis.GetSignature(signature);

        Assert.AreEqual("this.TestMethod(param1, param2)", result);
    }

    [TestMethod]
    public void TestGetSignatureWithClassName_ShouldReturnSignatureFormat()
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

        var referenceThis = new ReferenceThis() { Reference = new SimClass() { Name = "TestClass" } };

        var result = referenceThis.GetSignatureWithClassName(signature);

        Assert.AreEqual("TestClass.TestMethod(param1, param2)", result);
    }
}
