using Domain;
using Domain.Enums;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class SimClassTest
{
    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void SetBaseClassThrowsExceptionWhenBaseClassIsSealed()
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
    public void SetNameShouldThrowExceptionWhenNameIsInvalid()
    {
        var simClass = new SimClass();

        simClass.Name = "Invalid Name!";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void SetNameShouldThrowExceptionWhenNameContainsOnlyNumbers()
    {
        var simClass = new SimClass();

        simClass.Name = "123456";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void SetNameShouldThrowExceptionWhenNameIsReservedWord()
    {
        var simClass = new SimClass();

        simClass.Name = "class";
    }

    [TestMethod]
    public void MethodsShouldSetStateToAbstractWhenListContainsAbstractMethod()
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
    public void MethodsShouldNotChangeStateWhenListDoesNotContainAbstractMethod()
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
    public void SetBaseClassShouldSetBaseClassWhenBaseClassIsValidNormal()
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
    public void SetBaseClassShouldSetBaseClassWhenDerivedClassImplementsAllAbstractMethods()
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
    public void SetBaseClassShouldSetBaseClassWhenBothClassesAreAbstract()
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
    public void SetBaseClassShouldThrowExceptionWhenBaseClassIsSealed()
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
    public void SetBaseClassShouldThrowExceptionWhenConcreteClassDoesNotImplementAllAbstractMethods()
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
    public void SetBaseClassShouldSetBaseClassWhenAllAbstractMethodsAreImplemented()
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
    public void SetBaseClassShouldSetBaseClassWhenBothClassesAreNormal()
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
    public void SetBaseClassShouldSetBaseClassWhenAbstractClassDerivesFromAbstractClass()
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
    public void BaseClassIdWhenSetToNullShouldUseObjectGuid()
    {
        var simClass = new SimClass();

        simClass.BaseClassId = null;

        Assert.AreEqual(Guid.Parse("11111111-1111-1111-1111-111111111111"), simClass.BaseClassId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void BaseClassIdWhenClassIsAbstractAndSettingNonObjectGuidShouldThrowException()
    {
        var simClass = new SimClass
        {
            State = SimAccesibility.Abstract
        };

        simClass.BaseClassId = Guid.NewGuid();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void MethodsShouldThrowExceptionWhenInterfaceHasNonInterfaceMethods()
    {
        var simClass = new SimClass
        {
            Name = "IMyInterface",
            State = SimAccesibility.Interface
        };

        simClass.Methods =
        [
            new SimMethod { Name = "InterfaceMethod", Accesibility = SimAccesibility.Interface },
            new SimMethod { Name = "InvalidMethod", Accesibility = SimAccesibility.Normal }
        ];
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void MethodsShouldThrowExceptionWhenNonInterfaceClassHasInterfaceMethods()
    {
        var simClass = new SimClass
        {
            Name = "NormalClass",
            State = SimAccesibility.Normal
        };

        simClass.Methods =
        [
            new SimMethod { Name = "NormalMethod", Accesibility = SimAccesibility.Normal },
        new SimMethod { Name = "InterfaceMethod", Accesibility = SimAccesibility.Interface }
        ];
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void MethodsShouldThrowExceptionWhenContainsDuplicateMethodNames()
    {
        var simClass = new SimClass
        {
            Name = "TestClass"
        };

        simClass.Methods =
        [
            new SimMethod { Name = "Method1", Accesibility = SimAccesibility.Normal },
            new SimMethod { Name = "Method2", Accesibility = SimAccesibility.Normal },
            new SimMethod { Name = "Method1", Accesibility = SimAccesibility.Normal }
        ];
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void BaseClassId_ShouldThrowException_WhenInterfaceClassHasNonObjectGuid()
    {
        var simClass = new SimClass();
        simClass.Name = "ITestInterface";
        simClass.State = SimAccesibility.Interface;

        simClass.BaseClassId = new Guid("22222222-2222-2222-2222-222222222222");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void SetBaseClassShouldThrowExceptionWhenBaseClassIsInterface()
    {
        var interfaceClass = new SimClass
        {
            Name = "ITestInterface",
            State = SimAccesibility.Interface
        };

        var simClass = new SimClass
        {
            Name = "RegularClass"
        };

        simClass.BaseClass = interfaceClass;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void State_ShouldThrowException_WhenSettingAbstractWithNonObjectBaseClassId()
    {
        var simClass = new SimClass
        {
            BaseClassId = Guid.NewGuid(),
            Name = "TestClass"
        };
        simClass.BaseClassId = Guid.NewGuid();

        simClass.State = SimAccesibility.Abstract;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void State_ShouldThrowException_WhenSettingInterfaceWithNonObjectBaseClassId()
    {
        var simClass = new SimClass
        {
            BaseClassId = Guid.NewGuid(),
            Name = "ITestInterface"
        };
        simClass.BaseClassId = Guid.NewGuid();

        simClass.State = SimAccesibility.Interface;
    }
}
