using Domain;
using Domain.Enums;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class SimClassTest
{
    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void SetBaseClass_ThrowsException_WhenBaseClassIsSealed()
    {
        var baseClass = new SimClass
        {
            Name = "SealedBaseClass"
        };
        baseClass.State = SimAccesibility.Sealed;

        var simClass = new SimClass
        {
            Name = "DerivedClass"
        };

        simClass.BaseClass = baseClass;
    }

    [TestMethod]
    public void AddCorrectAttributeToSimClass_CorrectlyAddsIt()
    {
        var simClass = new SimClass()
        {
            Name = "TestSimClassWithAttribute"
        };
        var relatedClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var typeClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var simAttribute = new SimAttribute() { Name = "TestAttribute", RelatedClass = relatedClass, Type = typeClass, Privacity = SimPrivacity.Public };

        simClass.AddAttribute(simAttribute);

        Assert.IsTrue(simClass.Attributes.Any(a => a.Name == "TestAttribute"));
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void AddAttributeWithRepetedNameClass_ShouldThrowSimClassInvalidAttribute()
    {
        var simClass = new SimClass()
        {
            Name = "TestSimClassWithAttribute"
        };
        var relatedClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var typeClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var simAttribute = new SimAttribute() { Name = "TestAttribute", RelatedClass = relatedClass, Type = typeClass, Privacity = SimPrivacity.Public };
        var repeatedName = new SimAttribute() { Name = "TestAttribute", RelatedClass = typeClass, Type = relatedClass, Privacity = SimPrivacity.Private };

        simClass.AddAttribute(simAttribute);
        simClass.AddAttribute(repeatedName);
    }

    [TestMethod]
    public void DeleteExistentAttribute_ShouldDeleteIt()
    {
        var simClass = new SimClass()
        {
            Name = "TestSimClassWithAttribute"
        };
        var relatedClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var typeClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var simAttribute = new SimAttribute() { Name = "TestAttribute", RelatedClass = relatedClass, Type = typeClass, Privacity = SimPrivacity.Public };
        var anotherAttribute = new SimAttribute() { Name = "TestAnother", RelatedClass = typeClass, Type = relatedClass, Privacity = SimPrivacity.Private };

        simClass.AddAttribute(simAttribute);
        simClass.AddAttribute(anotherAttribute);

        simClass.DeleteAttribute(simAttribute);

        Assert.IsFalse(simClass.Attributes.Any(a => a.Name == "TestAttribute"));
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidOperation))]
    public void DeleteInexistentAttribute_ShouldThrowException()
    {
        var simClass = new SimClass()
        {
            Name = "TestSimClassWithAttribute"
        };
        var relatedClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var typeClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var simAttribute = new SimAttribute() { Name = "TestAttribute", RelatedClass = relatedClass, Type = typeClass, Privacity = SimPrivacity.Public };
        simClass.AddAttribute(simAttribute);
        simClass.DeleteAttribute(simAttribute);
        simClass.DeleteAttribute(simAttribute);
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void SetName_ShouldThrowException_WhenNameIsInvalid()
    {
        var simClass = new SimClass();

        simClass.Name = "Invalid Name!";
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void SetName_ShouldThrowException_WhenNameContainsOnlyNumbers()
    {
        var simClass = new SimClass();

        simClass.Name = "123456";
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void SetName_ShouldThrowException_WhenNameIsReservedWord()
    {
        var simClass = new SimClass();

        simClass.Name = "class";
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void SetBaseClass_ShouldThrowException_WhenAbstractMethodsAreNotImplemented()
    {
        var baseClass = new SimClass
        {
            Name = "AbstractBaseClass",
            State = SimAccesibility.Abstract,
            Methods =
        [
            new SimMethod { Name = "AbstractMethod1", Accesibility = SimAccesibility.Abstract },
            new SimMethod { Name = "AbstractMethod2", Accesibility = SimAccesibility.Abstract }
        ]
        };

        var derivedClass = new SimClass
        {
            Name = "DerivedClass",
            State = SimAccesibility.Normal,
            Methods =
        [
            new SimMethod { Name = "ConcreteMethod", Accesibility = SimAccesibility.Normal }
        ]
        };

        derivedClass.BaseClass = baseClass;
    }
}
