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

        var result = referenceThis.GetReferenceClass();

        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
        Assert.AreEqual("TestClass", result.Name);
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

        var referenceThis = new ReferenceThis();

        var result = referenceThis.GetSignature(signature);

        Assert.AreEqual("this.TestMethod(param1: RefClass1 InstClass1, param2: RefClass2 InstClass2)", result);
    }

    [TestMethod]
    public void TestGetSignatureWithClassName_ShouldReturnSignatureFormat()
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

        var referenceThis = new ReferenceThis { Reference = new SimClass { Name = "TestClass" } };

        var result = referenceThis.GetSignatureWithClassName(signature);

        Assert.AreEqual("TestClass.TestMethod(param1: RefClass1 InstClass1, param2: RefClass2 InstClass2)", result);
    }

    [TestMethod]
    public void GetInstanceClass_ShouldReturnReferenceInstance_ForReferenceThis()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var referenceThis = new ReferenceThis { Reference = simClass };

        var signature = new Signature();
        var executionInstance = new SimClass();

        var result = referenceThis.GetInstanceClass(signature, executionInstance);

        Assert.AreEqual(executionInstance, result);
    }
}
