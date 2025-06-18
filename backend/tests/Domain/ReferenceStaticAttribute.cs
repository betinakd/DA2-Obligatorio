using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class ReferenceStaticAttributeTest
{
    [TestMethod]
    public void TestGetSimClass_ShouldReturnAttributeType()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var attribute = new SimAttribute { Reference = simClass, Name = "StaticAttr", IsStatic = true };
        var referenceStaticAttribute = new ReferenceStaticAttribute { Reference = attribute };

        var result = referenceStaticAttribute.GetReferenceClass();

        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
        Assert.AreEqual("TestClass", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void TestGetSimClass_ShouldThrowExceptionWhenTypeIsNull()
    {
        var attribute = new SimAttribute { IsStatic = true };
        var referenceStaticAttribute = new ReferenceStaticAttribute { Reference = attribute };

        referenceStaticAttribute.GetReferenceClass();
    }

    [TestMethod]
    public void TestGetSignature_ShouldReturnStaticSignatureFormat()
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
        var attribute = new SimAttribute { Reference = simClass, Name = "StaticAttr", IsStatic = true };
        var reference = new ReferenceStaticAttribute { Reference = attribute };

        var result = reference.GetSignature(signature);

        Assert.AreEqual("StaticAttr.TestMethod(param1: RefClass1 InstClass1, param2: RefClass2 InstClass2)", result);
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
        var attribute = new SimAttribute { Reference = simClass, Name = "StaticAttr", IsStatic = true };
        var reference = new ReferenceStaticAttribute { Reference = attribute };

        var result = reference.GetSignatureWithClassName(signature);

        Assert.AreEqual("MyClass.MyMethod(x: RefClassX InstClassX, y: RefClassY InstClassY)", result);
    }

    [TestMethod]
    public void TestGetReferenceId_ShouldReturnReferenceId()
    {
        var expectedId = Guid.NewGuid();
        var attribute = new SimAttribute { Id = expectedId, Name = "StaticAttr", IsStatic = true };
        var reference = new ReferenceStaticAttribute { Reference = attribute };

        var actualId = reference.GetReferenceId();

        Assert.AreEqual(expectedId, actualId);
    }

    [TestMethod]
    public void TestGetReferenceTypeDescription_ShouldReturnAttribute()
    {
        var reference = new ReferenceStaticAttribute();

        var result = reference.GetReferenceTypeDescription();

        Assert.AreEqual("StaticAttribute", result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void TestReference_ShouldThrowExceptionWhenNonStaticAttribute()
    {
        var simClass = new SimClass { Name = "TestClass" };
        var nonStaticAttribute = new SimAttribute
        {
            Reference = simClass,
            Name = "NonStaticAttr",
            IsStatic = false,
        };
        var reference = new ReferenceStaticAttribute();

        reference.Reference = nonStaticAttribute;
    }

    [TestMethod]
    public void GetInstanceClass_ShouldReturnReferenceInstance()
    {
        var expectedInstance = new SimClass { Name = "InstanceClass" };
        var simClass = new SimClass { Name = "TestClass" };
        var staticAttribute = new SimAttribute
        {
            Reference = simClass,
            Name = "StaticAttr",
            IsStatic = true,
            Instance = expectedInstance,
        };
        var reference = new ReferenceStaticAttribute
        {
            Reference = staticAttribute,
        };

        var signature = new Signature();
        var executionInstance = new SimClass();

        var result = reference.GetInstanceClass(signature, executionInstance);

        Assert.AreEqual(expectedInstance, result);
    }
}
