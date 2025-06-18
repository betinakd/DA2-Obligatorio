using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class ReferenceAttributeTest
{
    [TestMethod]
    public void TestGetSimClass_ShouldReturnAttributeType()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var attribute = new SimAttribute { Reference = simClass, Name = "Atri" };
        var referenceAttribute = new ReferenceAttribute { Reference = attribute };

        var result = referenceAttribute.GetReferenceClass();

        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
        Assert.AreEqual("TestClass", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void TestGetSimClass_ShouldThrowExceptionWhenTypeIsNull()
    {
        var attribute = new SimAttribute();
        var referenceAttribute = new ReferenceAttribute { Reference = attribute };

        referenceAttribute.GetReferenceClass();
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

        var simClass = new SimClass { Name = "TestClass" };
        var attribute = new SimAttribute { Reference = simClass, Name = "Atri" };
        var reference = new ReferenceAttribute { Reference = attribute };
        var result = reference.GetSignature(signature);

        Assert.AreEqual("Atri.TestMethod(param1: RefClass1 InstClass1, param2: RefClass2 InstClass2)", result);
    }

    [TestMethod]
    public void TestGetSignatureWithClassName_ShouldReturnCorrectFormat()
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
        var attribute = new SimAttribute { Reference = simClass, Name = "Attr" };
        var reference = new ReferenceAttribute { Reference = attribute };

        var result = reference.GetSignatureWithClassName(signature);

        Assert.AreEqual("MyClass.MyMethod(x: RefClassX InstClassX, y: RefClassY InstClassY)", result);
    }

    [TestMethod]
    public void TestGetReferenceId_ShouldReturnReferenceId()
    {
        var expectedId = Guid.NewGuid();
        var attribute = new SimAttribute { Id = expectedId, Name = "Attr" };
        var reference = new ReferenceAttribute { Reference = attribute };

        var actualId = reference.GetReferenceId();

        Assert.AreEqual(expectedId, actualId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void TestReference_ShouldThrowExceptionWhenStaticAttribute()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var staticAttribute = new SimAttribute
        {
            Reference = simClass,
            Name = "StaticAttr",
            IsStatic = true,
        };
        var reference = new ReferenceAttribute();

        reference.Reference = staticAttribute;
    }

    [TestMethod]
    public void GetInstanceClass_ShouldReturnReferenceInstance()
    {
        var expectedInstance = new SimClass { Name = "InstanceClass" };
        var simClass = new SimClass { Name = "TestClass" };
        var attribute = new SimAttribute
        {
            Reference = simClass,
            Name = "Attr",
            Instance = expectedInstance,
        };
        var reference = new ReferenceAttribute
        {
            Reference = attribute,
        };

        var signature = new Signature();
        var executionInstance = new SimClass();

        var result = reference.GetInstanceClass(signature, executionInstance);

        Assert.AreEqual(expectedInstance, result);
    }
}
