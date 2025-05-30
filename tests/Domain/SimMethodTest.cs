using Domain;
using Domain.Enums;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class SimMethodTest
{
    [TestMethod]
    public void MatchSignature_WithMatchingSignature_ReturnsTrue()
    {
        var intType = new SimClass { Id = Guid.NewGuid(), Name = "int" };
        var stringType = new SimClass { Id = Guid.NewGuid(), Name = "string" };

        var method = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = intType.Id, Type = intType },
                new Parameter { Name = "param2", TypeId = stringType.Id, Type = stringType }
            ]
        };

        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x", TypeId = intType.Id },
                new ParameterSignature { Name = "y", TypeId = stringType.Id }
            ]
        };

        var result = method.MatchSignature(signature);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void MatchSignature_WithDifferentName_ReturnsFalse()
    {
        var intType = new SimClass { Id = Guid.NewGuid(), Name = "int" };

        var method = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = intType.Id, Type = intType }
            ]
        };

        var signature = new Signature
        {
            Name = "DifferentMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x", TypeId = intType.Id }
            ]
        };

        var result = method.MatchSignature(signature);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void MatchSignature_WithDifferentParameterCount_ReturnsFalse()
    {
        var intType = new SimClass { Id = Guid.NewGuid(), Name = "int" };
        var stringType = new SimClass { Id = Guid.NewGuid(), Name = "string" };

        var method = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = intType.Id, Type = intType },
                new Parameter { Name = "param2", TypeId = stringType.Id, Type = stringType }
            ]
        };

        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x", TypeId = intType.Id }
            ]
        };

        var result = method.MatchSignature(signature);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void MatchSignature_WithDifferentParameterTypes_ReturnsFalse()
    {
        var intType = new SimClass { Id = Guid.NewGuid(), Name = "int" };
        var stringType = new SimClass { Id = Guid.NewGuid(), Name = "string" };
        var doubleType = new SimClass { Id = Guid.NewGuid(), Name = "double" };

        var method = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = intType.Id, Type = intType },
                new Parameter { Name = "param2", TypeId = stringType.Id, Type = stringType }
            ]
        };

        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x", TypeId = intType.Id },
                new ParameterSignature { Name = "y", TypeId = doubleType.Id } // Different type
            ]
        };

        var result = method.MatchSignature(signature);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void MatchSignature_WithDifferentParameterOrder_ReturnsFalse()
    {
        var intType = new SimClass { Id = Guid.NewGuid(), Name = "int" };
        var stringType = new SimClass { Id = Guid.NewGuid(), Name = "string" };

        var method = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = intType.Id, Type = intType },
                new Parameter { Name = "param2", TypeId = stringType.Id, Type = stringType }
            ]
        };

        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "y", TypeId = stringType.Id }, // Swapped order
                new ParameterSignature { Name = "x", TypeId = intType.Id }
            ]
        };

        var result = method.MatchSignature(signature);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetMethodSignature_ReturnsCorrectFormat()
    {
        var method = new SimMethod
        {
            RelatedClass = new SimClass { Id = Guid.NewGuid(), Name = "MyClass" },
            Name = "MyMethod"
        };

        var signature = new Signature
        {
            Name = "MyMethod",
            Parameters =
            [
                new ParameterSignature { Name = "a", TypeId = Guid.NewGuid() },
                new ParameterSignature { Name = "b", TypeId = Guid.NewGuid() }
            ]
        };

        var result = method.GetMethodSignature(signature);

        Assert.AreEqual("MyClass.MyMethod(a, b)", result);
    }

    [TestMethod]
    public void Equals_WithNonSimMethodObject_ReturnsFalse()
    {
        var method = new SimMethod { Name = "TestMethod" };
        var nonSimMethodObject = new object();

        var result = method.Equals(nonSimMethodObject);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithDifferentMethodName_ReturnsFalse()
    {
        var method1 = new SimMethod { Name = "TestMethod" };
        var method2 = new SimMethod { Name = "DifferentMethod" };

        var result = method1.Equals(method2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithDifferentParameterCount_ReturnsFalse()
    {
        var typeId = Guid.NewGuid();

        var method1 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [new Parameter { Name = "param1", TypeId = typeId }]
        };

        var method2 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [
                new Parameter { Name = "param1", TypeId = typeId },
                new Parameter { Name = "param2", TypeId = typeId }
            ]
        };

        var result = method1.Equals(method2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithDifferentParameterNames_ReturnsTrue()
    {
        var typeId = Guid.NewGuid();

        var method1 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [new Parameter { Name = "param1", TypeId = typeId }]
        };

        var method2 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [new Parameter { Name = "differentParam", TypeId = typeId }]
        };

        var result = method1.Equals(method2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_WithDifferentParameterTypes_ReturnsFalse()
    {
        var typeId1 = Guid.NewGuid();
        var typeId2 = Guid.NewGuid();

        var method1 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [new Parameter { Name = "param1", TypeId = typeId1 }]
        };

        var method2 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [new Parameter { Name = "param1", TypeId = typeId2 }]
        };

        var result = method1.Equals(method2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_TypeIdComparisonFalseCase()
    {
        var typeId1 = Guid.NewGuid();
        var typeId2 = Guid.NewGuid();

        var method1 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [
                new Parameter { Name = "param1", TypeId = typeId1 }
            ]
        };

        var method2 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [
                new Parameter { Name = "param1", TypeId = typeId2 }
            ]
        };

        var result = method1.Equals(method2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Name_WhenSetToValidValue_ShouldSetValue()
    {
        var simClass = new SimClass();

        simClass.Name = "ValidClassName";

        Assert.AreEqual("ValidClassName", simClass.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_WhenSetToEmptyString_ShouldThrowException()
    {
        var simClass = new SimClass();

        simClass.Name = string.Empty;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_WhenSetToInvalidCharacters_ShouldThrowException()
    {
        var simClass = new SimClass();

        simClass.Name = "Invalid@Name";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_WhenSetToOnlyNumbers_ShouldThrowException()
    {
        var simClass = new SimClass();

        simClass.Name = "12345";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_WhenSetToReservedWord_ShouldThrowException()
    {
        var simClass = new SimClass();

        simClass.Name = "class";
    }

    [TestMethod]
    public void Name_WhenSetToValidNameWithNumbers_ShouldSetValue()
    {
        var simClass = new SimClass();

        simClass.Name = "ClassName123";

        Assert.AreEqual("ClassName123", simClass.Name);
    }

    [TestMethod]
    public void Parameters_WhenSettingParametersOnNonInterfaceMethod_ShouldNotThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            Accesibility = SimAccesibility.Normal
        };
        var parameters = new List<Parameter>
    {
        new Parameter { Name = "param1", TypeId = Guid.NewGuid() }
    };

        method.Parameters = parameters;

        CollectionAssert.AreEqual(parameters, method.Parameters);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void LocalVariables_WhenSettingLocalVariablesOnInterfaceMethod_ShouldThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            Accesibility = SimAccesibility.Interface
        };

        method.LocalVariables =
    [
        new LocalVariable { Name = "localVar1", TypeId = Guid.NewGuid() }
    ];
    }

    [TestMethod]
    public void LocalVariables_WhenSettingLocalVariablesOnNonInterfaceMethod_ShouldNotThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            Accesibility = SimAccesibility.Normal
        };
        var localVariables = new List<LocalVariable>
    {
        new LocalVariable { Name = "localVar1", TypeId = Guid.NewGuid() }
    };

        method.LocalVariables = localVariables;

        CollectionAssert.AreEqual(localVariables, method.LocalVariables);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Invocations_WhenSettingInvocationsOnInterfaceMethod_ShouldThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            Accesibility = SimAccesibility.Interface
        };

        method.Invocations =
    [
        new Invocation { RelatedMethodId = Guid.NewGuid() }
    ];
    }

    [TestMethod]
    public void Invocations_WhenSettingInvocationsOnNonInterfaceMethod_ShouldNotThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            Accesibility = SimAccesibility.Normal
        };
        var invocations = new List<Invocation>
    {
        new Invocation { RelatedMethodId = Guid.NewGuid() }
    };

        method.Invocations = invocations;

        CollectionAssert.AreEqual(invocations, method.Invocations);
    }

    [TestMethod]
    public void EqualsWithoutReturnType_WithNotSameObject_ShouldReturnFalse()
    {
        var method = new SimMethod { Name = "TestMethod" };
        var differentObject = new object();

        var result = method.EqualsWithoutReturnType(differentObject);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void EqualsWithReturnType_SameSignatureButDifferentReturnType_ReturnsFalse()
    {
        var paramTypeId = Guid.NewGuid();

        var method1 = new SimMethod
        {
            Name = "TestMethod",
            ReturnTypeId = Guid.NewGuid(),
            Parameters =
        [
            new Parameter { Name = "param1", TypeId = paramTypeId }
        ]
        };

        var method2 = new SimMethod
        {
            Name = "TestMethod",
            ReturnTypeId = Guid.NewGuid(),
            Parameters =
        [
            new Parameter { Name = "differentParamName", TypeId = paramTypeId }
        ]
        };

        Assert.IsTrue(method1.EqualsWithoutReturnType(method2));

        var result = method1.Equals(method2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Invocations_WhenStaticMethodHasNonStaticInvocations_ShouldThrowException()
    {
        var staticMethod = new SimMethod
        {
            Name = "StaticTestMethod",
            IsStatic = true
        };

        var nonStaticReference = new ReferenceAttribute();
        staticMethod.Invocations =
        [
            new Invocation
            {
                RelatedMethodId = Guid.NewGuid(),
                Reference = nonStaticReference
            }

        ];
    }

    [TestMethod]
    public void Invocations_WhenStaticMethodHasOnlyStaticInvocations_ShouldNotThrowException()
    {
        var staticMethod = new SimMethod
        {
            Name = "StaticTestMethod",
            IsStatic = true
        };

        var staticReference = new ReferenceStatic();
        staticMethod.Invocations =
        [
            new Invocation
            {
                RelatedMethodId = Guid.NewGuid(),
                Reference = staticReference
            }

        ];

        Assert.IsNotNull(staticMethod.Invocations);
        Assert.AreEqual(1, staticMethod.Invocations.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Accesibility_WhenStaticMethodSetToAbstract_ShouldThrowException()
    {
        var staticMethod = new SimMethod
        {
            Name = "StaticMethod",
            IsStatic = true
        };

        staticMethod.Accesibility = SimAccesibility.Abstract;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Accesibility_WhenStaticMethodSetToInterface_ShouldThrowException()
    {
        var staticMethod = new SimMethod
        {
            Name = "StaticMethod",
            IsStatic = true
        };

        staticMethod.Accesibility = SimAccesibility.Interface;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Accesibility_WhenStaticMethodSetToSealed_ShouldThrowException()
    {
        var staticMethod = new SimMethod
        {
            Name = "StaticMethod",
            IsStatic = true
        };

        staticMethod.Accesibility = SimAccesibility.Sealed;
    }

    [TestMethod]
    public void Accesibility_WhenStaticMethodSetToNormal_ShouldNotThrowException()
    {
        var staticMethod = new SimMethod
        {
            Name = "StaticMethod",
            IsStatic = true
        };

        staticMethod.Accesibility = SimAccesibility.Normal;

        Assert.AreEqual(SimAccesibility.Normal, staticMethod.Accesibility);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void IsStatic_WhenSetToTrueWithAbstractAccesibility_ShouldThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            Accesibility = SimAccesibility.Abstract
        };

        method.IsStatic = true;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void IsStatic_WhenSetToTrueWithInterfaceAccesibility_ShouldThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            Accesibility = SimAccesibility.Interface
        };

        method.IsStatic = true;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void IsStatic_WhenSetToTrueWithSealedAccesibility_ShouldThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            Accesibility = SimAccesibility.Sealed
        };

        method.IsStatic = true;
    }

    [TestMethod]
    public void IsStatic_WhenSetToTrueWithNormalAccesibility_ShouldNotThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            Accesibility = SimAccesibility.Normal
        };

        method.IsStatic = true;

        Assert.IsTrue(method.IsStatic);
    }

    [TestMethod]
    public void IsStatic_WhenSetToFalse_ShouldNotThrowExceptionRegardlessOfAccesibility()
    {
        var methodAbstract = new SimMethod { Accesibility = SimAccesibility.Abstract };
        var methodInterface = new SimMethod { Accesibility = SimAccesibility.Interface };
        var methodSealed = new SimMethod { Accesibility = SimAccesibility.Sealed };

        methodAbstract.IsStatic = false;
        methodInterface.IsStatic = false;
        methodSealed.IsStatic = false;

        Assert.IsFalse(methodAbstract.IsStatic);
        Assert.IsFalse(methodInterface.IsStatic);
        Assert.IsFalse(methodSealed.IsStatic);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void IsVirtual_WhenSetToTrueWithStaticMethod_ShouldThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            IsStatic = true
        };

        method.IsVirtual = true;
    }

    [TestMethod]
    public void IsVirtual_WhenSetToTrueWithNonStaticMethod_ShouldNotThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            IsStatic = false
        };

        method.IsVirtual = true;

        Assert.IsTrue(method.IsVirtual);
    }

    [TestMethod]
    public void IsVirtual_WhenSetToFalse_ShouldNotThrowExceptionRegardlessOfStaticFlag()
    {
        var staticMethod = new SimMethod { IsStatic = true };
        var nonStaticMethod = new SimMethod { IsStatic = false };

        staticMethod.IsVirtual = false;
        nonStaticMethod.IsVirtual = false;

        Assert.IsFalse(staticMethod.IsVirtual);
        Assert.IsFalse(nonStaticMethod.IsVirtual);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void IsVirtual_WhenSetToTrueWithInterfaceMethod_ShouldThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            Accesibility = SimAccesibility.Interface
        };

        method.IsVirtual = true;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void IsOverride_WhenSetToTrueWithStaticMethod_ShouldThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            IsStatic = true
        };

        method.IsOverride = true;
    }

    [TestMethod]
    public void IsOverride_WhenSetToTrueWithNonStaticMethod_ShouldNotThrowException()
    {
        var method = new SimMethod
        {
            Name = "TestMethod",
            IsStatic = false,
            IsVirtual = true
        };

        method.IsOverride = true;

        Assert.IsTrue(method.IsOverride);
    }

    [TestMethod]
    public void IsOverride_WhenSetToFalse_ShouldNotThrowExceptionRegardlessOfStaticFlag()
    {
        var staticMethod = new SimMethod { IsStatic = true };
        var nonStaticMethod = new SimMethod { IsStatic = false };

        staticMethod.IsOverride = false;
        nonStaticMethod.IsOverride = false;

        Assert.IsFalse(staticMethod.IsOverride);
        Assert.IsFalse(nonStaticMethod.IsOverride);
    }

    [TestMethod]
    public void Validate_WhenMethodIsCorrectlyConfigured_ShouldNotThrowException()
    {
        var method = new SimMethod
        {
            Name = "ValidMethod",
            IsVirtual = true,
            IsOverride = true,
            Privacity = SimPrivacity.Public
        };

        method.Validate();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Validate_WhenOverrideIsNotVirtual_ShouldThrowException()
    {
        var method = new SimMethod
        {
            Name = "InvalidMethod",
            IsVirtual = false,
            IsOverride = true
        };

        method.Validate();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Validate_WhenOverrideIsPrivate_ShouldThrowException()
    {
        var method = new SimMethod
        {
            Name = "InvalidMethod",
            IsVirtual = true,
            IsOverride = true,
            Privacity = SimPrivacity.Private
        };

        method.Validate();
    }

    [TestMethod]
    public void Validate_WhenNotOverride_ShouldNotThrowException()
    {
        var method = new SimMethod
        {
            Name = "ValidMethod",
            IsVirtual = false,
            IsOverride = false,
            Privacity = SimPrivacity.Private
        };

        method.Validate();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Parameters_WhenSettingDuplicateParameterNames_ShouldThrowException()
    {
        var typeId = Guid.NewGuid();

        var method = new SimMethod
        {
            Name = "TestMethod"
        };

        var parameters = new List<Parameter>
    {
        new Parameter { Name = "param1", TypeId = typeId },
        new Parameter { Name = "Param1", TypeId = typeId } // Duplicado (case-insensitive)
    };

        method.Parameters = parameters;
    }
}
