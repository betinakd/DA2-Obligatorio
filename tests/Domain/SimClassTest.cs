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
        // Crear un tipo de retorno para los métodos
        var voidType = new SimClass
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "void",
            State = SimAccesibility.Normal,
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        };

        // Inicializar la clase base abstracta con métodos completos
        var baseClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "AbstractBaseClass",
            State = SimAccesibility.Abstract,
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Methods =
            [
                new SimMethod
                {
                Name = "AbstractMethod1",
                Accesibility = SimAccesibility.Abstract,
                ReturnType = voidType,
                ReturnTypeId = voidType.Id,
                Parameters = [],
                Privacity = SimPrivacity.Public
            },
            new SimMethod
            {
                Name = "AbstractMethod2",
                Accesibility = SimAccesibility.Abstract,
                ReturnType = voidType,
                ReturnTypeId = voidType.Id,
                Parameters = [],
                Privacity = SimPrivacity.Public
            }

            ]
        };

        // Configurar la clase derivada con implementación parcial
        var derivedClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "DerivedClass",
            State = SimAccesibility.Normal,
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Methods =
            [
                new SimMethod
                {
                Name = "AbstractMethod1",
                Accesibility = SimAccesibility.Normal,
                ReturnType = voidType,
                ReturnTypeId = voidType.Id,
                Parameters = [],
                Privacity = SimPrivacity.Public
            }

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

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void AttributesShouldThrowExceptionWhenContainsDuplicateAttributeNames()
    {
        var simClass = new SimClass
        {
            Name = "TestClass"
        };
        var boolClass = new SimClass
        {
            Name = "bool",
            State = SimAccesibility.Normal,
            Id = Guid.NewGuid(),
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        };
        var stringClass = new SimClass
        {
            Name = "string",
            State = SimAccesibility.Normal,
            Id = Guid.NewGuid(),
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        };

        simClass.Attributes =
        [
            new SimAttribute { Name = "Attribute1", Type = stringClass },
            new SimAttribute { Name = "Attribute2", Type = stringClass },
            new SimAttribute { Name = "Attribute1", Type = boolClass }
        ];
    }

    [TestMethod]
    public void AttributesShouldSetValueWhenNoDuplicateAttributeNames()
    {
        var simClass = new SimClass
        {
            Name = "TestClass"
        };
        var boolClass = new SimClass
        {
            Name = "bool",
            State = SimAccesibility.Normal,
            Id = Guid.NewGuid(),
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        };
        var stringClass = new SimClass
        {
            Name = "string",
            State = SimAccesibility.Normal,
            Id = Guid.NewGuid(),
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        };
        var attributes = new List<SimAttribute>
        {
            new SimAttribute { Name = "Attribute1", Type = stringClass },
            new SimAttribute { Name = "Attribute2", Type = stringClass },
            new SimAttribute { Name = "Attribute3", Type = boolClass }
        };

        simClass.Attributes = attributes;

        CollectionAssert.AreEqual(attributes, simClass.Attributes);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Attributes_ShouldThrowException_WhenSettingAttributesOnInterface()
    {
        var simClass = new SimClass
        {
            Name = "ITestInterface",
            State = SimAccesibility.Interface
        };

        simClass.Attributes =
    [
        new SimAttribute { Name = "TestAttribute" }
    ];
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Implements_ShouldThrowException_WhenInterfaceImplementsClasses()
    {
        var simClass = new SimClass
        {
            Name = "ITestInterface",
            State = SimAccesibility.Interface
        };

        var classToImplement = new SimClass
        {
            Name = "RegularClass",
            State = SimAccesibility.Normal
        };

        simClass.Implements =
    [
        classToImplement
    ];
    }

    [TestMethod]
    public void Implements_ShouldNotThrowException_WhenNonInterfaceImplementsClasses()
    {
        var simClass = new SimClass
        {
            Name = "RegularClass",
            State = SimAccesibility.Normal
        };

        var classToImplement = new SimClass
        {
            Name = "InterfaceToImplement",
            State = SimAccesibility.Interface
        };

        var implementations = new List<SimClass>
    {
        classToImplement
    };

        simClass.Implements = implementations;

        CollectionAssert.AreEqual(implementations, simClass.Implements);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Implements_ShouldThrowException_WhenImplementingNonInterface()
    {
        var simClass = new SimClass
        {
            Name = "RegularClass",
            State = SimAccesibility.Normal
        };

        var regularClassToImplement = new SimClass
        {
            Name = "AnotherRegularClass",
            State = SimAccesibility.Normal
        };

        simClass.Implements =
    [
        regularClassToImplement
    ];
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void SetImplements_ShouldThrowException_WhenMissingInterfaceMethods()
    {
        var voidType = new SimClass
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "void",
            State = SimAccesibility.Normal,
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        };

        var interfaceClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ITestInterface",
            State = SimAccesibility.Interface,
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Methods =
            [
                new SimMethod
                {
                Name = "Method1",
                Accesibility = SimAccesibility.Interface,
                ReturnType = voidType,
                ReturnTypeId = voidType.Id,
                Parameters = [],
                Privacity = SimPrivacity.Public
            },
            new SimMethod
            {
                Name = "Method2",
                Accesibility = SimAccesibility.Interface,
                ReturnType = voidType,
                ReturnTypeId = voidType.Id,
                Parameters = [],
                Privacity = SimPrivacity.Public
            }

            ]
        };

        var simClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ImplementingClass",
            State = SimAccesibility.Normal,
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Methods =
            [
                new SimMethod
                {
                Name = "Method1",
                Accesibility = SimAccesibility.Normal,
                ReturnType = voidType,
                ReturnTypeId = voidType.Id,
                Parameters = [],
                Privacity = SimPrivacity.Public
            }

            ]
        };

        simClass.SetImplements([interfaceClass]);
    }

    [TestMethod]
    public void SetImplements_ShouldNotThrowException_WhenAllInterfaceMethodsImplemented()
    {
        var interfaceClass = new SimClass
        {
            Name = "ITestInterface",
            State = SimAccesibility.Interface,
            Methods =
            [
                new SimMethod { Name = "Method1", Accesibility = SimAccesibility.Interface },
            new SimMethod { Name = "Method2", Accesibility = SimAccesibility.Interface }
            ]
        };

        var simClass = new SimClass
        {
            Name = "ImplementingClass",
            State = SimAccesibility.Normal,
            Methods =
            [
                new SimMethod { Name = "Method1", Accesibility = SimAccesibility.Normal },
            new SimMethod { Name = "Method2", Accesibility = SimAccesibility.Normal },
            new SimMethod { Name = "ExtraMethod", Accesibility = SimAccesibility.Normal }
            ]
        };

        simClass.SetImplements([interfaceClass]);

        CollectionAssert.Contains(simClass.Implements.ToList(), interfaceClass);
    }

    [TestMethod]
    public void Equals_SameId_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var class1 = new SimClass { Id = id, Name = "Class1" };
        var class2 = new SimClass { Id = id, Name = "DifferentName" };

        Assert.IsTrue(class1.Equals(class2));
        Assert.IsTrue(class2.Equals(class1));
        Assert.IsTrue(class1 == class1);
    }

    [TestMethod]
    public void Equals_DifferentIds_ReturnsFalse()
    {
        var class1 = new SimClass { Id = Guid.NewGuid(), Name = "Class1" };
        var class2 = new SimClass { Id = Guid.NewGuid(), Name = "Class1" };

        Assert.IsFalse(class1.Equals(class2));
    }

    [TestMethod]
    public void Equals_NullOrDifferentType_ReturnsFalse()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "Class1" };
        var differentType = "Not a SimClass";

        Assert.IsFalse(simClass.Equals(null));
        Assert.IsFalse(simClass.Equals(differentType));
    }

    [TestMethod]
    public void GetHashCode_SameId_ReturnsSameHashCode()
    {
        var id = Guid.NewGuid();
        var class1 = new SimClass { Id = id, Name = "Class1" };
        var class2 = new SimClass { Id = id, Name = "DifferentName" };

        Assert.AreEqual(class1.GetHashCode(), class2.GetHashCode());
    }

    [TestMethod]
    public void Collection_Contains_WorksWithCustomEquals()
    {
        var id = Guid.NewGuid();
        var class1 = new SimClass { Id = id, Name = "Class1" };
        var class2 = new SimClass { Id = id, Name = "Same class, different instance" };

        var collection = new List<SimClass> { class1 };

        Assert.IsTrue(collection.Contains(class2), "Collection should find an object with the same ID");
    }
}
