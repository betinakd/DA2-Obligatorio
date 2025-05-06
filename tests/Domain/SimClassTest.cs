using Domain;
using Domain.Enums;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class SimClassTest
{
    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
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
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void SetName_ShouldThrowException_WhenNameIsInvalid()
    {
        var simClass = new SimClass();

        simClass.Name = "Invalid Name!";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void SetName_ShouldThrowException_WhenNameContainsOnlyNumbers()
    {
        var simClass = new SimClass();

        simClass.Name = "123456";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void SetName_ShouldThrowException_WhenNameIsReservedWord()
    {
        var simClass = new SimClass();

        simClass.Name = "class";
    }

    [TestMethod]
    public void Methods_ShouldSetStateToAbstract_WhenListContainsAbstractMethod()
    {
        var simClass = new SimClass
        {
            Methods =
        [
            new SimMethod { Name = "ConcreteMethod", Accesibility = SimAccesibility.Normal },
            new SimMethod { Name = "AbstractMethod", Accesibility = SimAccesibility.Abstract }
        ]
        };

        Assert.AreEqual(SimAccesibility.Abstract, simClass.State);
    }

    [TestMethod]
    public void Methods_ShouldNotChangeState_WhenListDoesNotContainAbstractMethod()
    {
        var simClass = new SimClass
        {
            Methods =
        [
            new SimMethod { Name = "ConcreteMethod1", Accesibility = SimAccesibility.Normal },
            new SimMethod { Name = "ConcreteMethod2", Accesibility = SimAccesibility.Normal }
        ]
        };

        Assert.AreEqual(SimAccesibility.Normal, simClass.State);
    }

    [TestMethod]
    public void SetBaseClass_ShouldSetBaseClass_WhenBaseClassIsValidNormal()
    {
        var baseClass = new SimClass
        {
            Name = "NormalBaseClass",
            State = SimAccesibility.Normal
        };

        var simClass = new SimClass
        {
            Name = "DerivedClass"
        };

        simClass.BaseClass = baseClass;

        Assert.AreEqual(baseClass, simClass.BaseClass);
        Assert.AreEqual(baseClass.Id, simClass.BaseClass.Id);
    }

    [TestMethod]
    public void SetBaseClass_ShouldSetBaseClass_WhenDerivedClassImplementsAllAbstractMethods()
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
            Methods =
            [
                new SimMethod { Name = "AbstractMethod1", Accesibility = SimAccesibility.Normal },
                new SimMethod { Name = "AbstractMethod2", Accesibility = SimAccesibility.Normal }
            ]
        };

        derivedClass.BaseClass = baseClass;

        Assert.AreEqual(baseClass, derivedClass.BaseClass);
    }

    [TestMethod]
    public void SetBaseClass_ShouldSetBaseClass_WhenBothClassesAreAbstract()
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
            Name = "AbstractDerivedClass",
            State = SimAccesibility.Abstract,
            Methods =
            [
                new SimMethod { Name = "NewAbstractMethod", Accesibility = SimAccesibility.Abstract }
            ]
        };

        derivedClass.BaseClass = baseClass;

        Assert.AreEqual(baseClass, derivedClass.BaseClass);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void SetBaseClass_ShouldThrowException_WhenBaseClassIsSealed()
    {
        var baseClass = new SimClass
        {
            Name = "SealedBaseClass",
            State = SimAccesibility.Sealed
        };

        var simClass = new SimClass
        {
            Name = "DerivedClass"
        };

        simClass.SetBaseClass(baseClass);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void SetBaseClass_ShouldThrowException_WhenConcreteClassDoesNotImplementAllAbstractMethods()
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
                new SimMethod { Name = "AbstractMethod1", Accesibility = SimAccesibility.Normal }
            ]
        };

        derivedClass.SetBaseClass(baseClass);
    }

    [TestMethod]
    public void SetBaseClass_ShouldSetBaseClass_WhenAllAbstractMethodsAreImplemented()
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
                new SimMethod { Name = "AbstractMethod1", Accesibility = SimAccesibility.Normal },
                new SimMethod { Name = "AbstractMethod2", Accesibility = SimAccesibility.Normal },
                new SimMethod { Name = "AdditionalMethod", Accesibility = SimAccesibility.Normal }
            ]
        };

        derivedClass.SetBaseClass(baseClass);

        Assert.AreEqual(baseClass, derivedClass.BaseClass);
        Assert.AreEqual(baseClass.Id, derivedClass.BaseClass.Id);
    }

    [TestMethod]
    public void SetBaseClass_ShouldSetBaseClass_WhenBothClassesAreNormal()
    {
        var baseClass = new SimClass
        {
            Name = "NormalBaseClass",
            State = SimAccesibility.Normal
        };

        var derivedClass = new SimClass
        {
            Name = "NormalDerivedClass",
            State = SimAccesibility.Normal
        };

        derivedClass.SetBaseClass(baseClass);

        Assert.AreEqual(baseClass, derivedClass.BaseClass);
        Assert.AreEqual(baseClass.Id, derivedClass.BaseClass.Id);
    }

    [TestMethod]
    public void SetBaseClass_ShouldSetBaseClass_WhenAbstractClassDerivesFromAbstractClass()
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
            Name = "AbstractDerivedClass",
            State = SimAccesibility.Abstract
        };

        derivedClass.SetBaseClass(baseClass);

        Assert.AreEqual(baseClass, derivedClass.BaseClass);
    }

    [TestMethod]
    public void BaseClassId_WhenSetToNull_ShouldUseObjectGuid()
    {
        var simClass = new SimClass();

        simClass.BaseClassId = null;

        Assert.AreEqual(Guid.Parse("11111111-1111-1111-1111-111111111111"), simClass.BaseClassId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void BaseClassId_WhenClassIsAbstractAndSettingNonObjectGuid_ShouldThrowException()
    {
        var simClass = new SimClass
        {
            State = SimAccesibility.Abstract
        };

        simClass.BaseClassId = Guid.NewGuid();
    }
}
