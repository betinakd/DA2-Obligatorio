using BusinessLogic;
using BusinessLogic.Exceptions;
using Domain;
using Domain.Enums;
using IDataAccess;
using Moq;

namespace Tests.BusinessLogic;

[TestClass]
public class SimMethodServiceTest
{
    private Mock<ISimMethodDataAccess>? _mockSimMethodDataAccess;
    private Mock<ISimClassDataAccess>? _mockSimClassDataAccess;
    private Mock<IExecutionDataAccess>? _mockExectuionDataAccess;

    private SimMethodService? _simMethodService;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimMethodDataAccess = new Mock<ISimMethodDataAccess>(MockBehavior.Strict);
        _mockSimClassDataAccess = new Mock<ISimClassDataAccess>(MockBehavior.Strict);
        _mockExectuionDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Strict);
        _simMethodService = new SimMethodService(_mockSimMethodDataAccess.Object, _mockSimClassDataAccess.Object, _mockExectuionDataAccess.Object);
    }

    [TestMethod]
    public void AddInvocation_ShouldThrowException_WhenMethodDoesNotExist()
    {
        var methodId = Guid.NewGuid();
        var invocation = new Invocation();
        var returnTypeId = Guid.NewGuid();
        invocation.Signature = new Signature
        {
            ReturnTypeId = returnTypeId,
            Name = "TestInvocation",
            Parameters = [],
        };

        _mockSimClassDataAccess!.Setup(m => m.ExistSimClassById(returnTypeId)).Returns(true); // Mock para ReturnTypeId

        _mockSimMethodDataAccess!.Setup(m => m.ExistMethodById(methodId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simMethodService!.AddInvocation(methodId, invocation));
        _mockSimMethodDataAccess.Verify();
    }

    [TestMethod]
    public void AddInvocation_ShouldReturnInvocation_WhenMethodExists()
    {
        var methodId = Guid.NewGuid();
        var returnTypeId = Guid.NewGuid();
        var newInvocation = new Invocation
        {
            Signature = new Signature
            {
                ReturnTypeId = returnTypeId,
                Name = "TestInvocation",
                Parameters = [],
            }
        };
        var expectedInvocation = new Invocation();

        _mockSimClassDataAccess!.Setup(m => m.ExistSimClassById(returnTypeId)).Returns(true);
        _mockSimMethodDataAccess!.Setup(m => m.ExistMethodById(methodId)).Returns(true);
        _mockSimMethodDataAccess.Setup(m => m.CreateInvocation(methodId, newInvocation)).Returns(expectedInvocation);

        var result = _simMethodService!.AddInvocation(methodId, newInvocation);

        Assert.AreEqual(expectedInvocation, result);
        _mockSimClassDataAccess.Verify(m => m.ExistSimClassById(returnTypeId), Times.Once);
        _mockSimMethodDataAccess.Verify(m => m.ExistMethodById(methodId), Times.Once);
        _mockSimMethodDataAccess.Verify(m => m.CreateInvocation(methodId, newInvocation), Times.Once);
    }

    [TestMethod]
    public void AddMethod_ShouldThrowException_WhenSimClassDoesNotExist()
    {
        var classId = Guid.NewGuid();
        var method = new SimMethod();

        _mockSimClassDataAccess!.Setup(m => m.ExistSimClassById(classId)).Returns(false);
        _mockSimMethodDataAccess!.Setup(m => m.ExistsMethodInClass(classId, method)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simMethodService!.AddMethod(classId, method));
        _mockSimMethodDataAccess.Verify();
    }

    [TestMethod]
    public void AddMethod_ShouldThrowException_WhenMethodWithSameFirmExistsInClass()
    {
        var classId = Guid.NewGuid();
        var method = new SimMethod();
        var simClass = new SimClass
        {
            Id = classId,
            Name = "TestClass",
            State = SimAccesibility.Normal,
        };

        _mockSimClassDataAccess!.Setup(m => m.ExistSimClassById(classId)).Returns(true);
        _mockSimMethodDataAccess!.Setup(m => m.ExistsMethodInClass(classId, method)).Returns(true);
        _mockSimClassDataAccess!
            .Setup(m => m.ExistSimClassById(classId))
            .Returns(true);
        _mockSimClassDataAccess!
            .Setup(m => m.GetSimClassById(classId))
            .Returns(simClass);
        _mockSimClassDataAccess!
            .Setup(m => m.InUseByOther(classId))
            .Returns(false);
        _mockSimMethodDataAccess!
            .Setup(m => m.ExistsMethodInClass(classId, method))
            .Returns(true);
        _mockSimMethodDataAccess!
            .Setup(m => m.CreateMethod(classId, method))
            .Returns(method);

        _mockSimClassDataAccess!
            .Setup(m => m.UpdateSimClass(It.IsAny<SimClass>()))
            .Verifiable();

        Assert.ThrowsException<InUseValueLogic>(() =>
            _simMethodService!.AddMethod(classId, method));
        _mockSimMethodDataAccess.Verify();
    }

    [TestMethod]
    public void GetInvocationById_ShouldReturnInvocation_WhenInvocationExists()
    {
        var invocationId = Guid.NewGuid();
        var expectedInvocation = new Invocation { Id = invocationId };

        _mockSimMethodDataAccess!.Setup(m => m.ExistInvocationById(invocationId)).Returns(true);
        _mockSimMethodDataAccess.Setup(m => m.GetInvocationById(invocationId)).Returns(expectedInvocation);

        var result = _simMethodService!.GetInvocationById(invocationId);

        Assert.AreEqual(expectedInvocation, result);
        _mockSimMethodDataAccess.Verify(m => m.GetInvocationById(invocationId), Times.Once);
    }

    [TestMethod]
    public void GetMethodById_ShouldThrowException_WhenMethodDoesNotExist()
    {
        var methodId = Guid.NewGuid();
        _mockSimMethodDataAccess!.Setup(m => m.ExistMethodById(methodId)).Returns(false);

        _mockSimMethodDataAccess.Verify();
        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simMethodService!.GetMethodById(methodId));
        _mockSimMethodDataAccess.Verify();
    }

    [TestMethod]
    public void GetMethodById_ShouldReturnMethod_WhenMethodExists()
    {
        var methodId = Guid.NewGuid();
        var expectedMethod = new SimMethod { Id = methodId };

        _mockSimMethodDataAccess!.Setup(m => m.ExistMethodById(methodId)).Returns(true);
        _mockSimMethodDataAccess.Setup(m => m.GetMethodById(methodId)).Returns(expectedMethod);

        var result = _simMethodService!.GetMethodById(methodId);

        Assert.AreEqual(expectedMethod, result);
        _mockSimMethodDataAccess.Verify(m => m.GetMethodById(methodId), Times.Once);
    }

    [TestMethod]
    public void GetParameterById_ShouldThrowException_WhenParameterDoesNotExist()
    {
        var parameterId = Guid.NewGuid();
        _mockSimMethodDataAccess!.Setup(m => m.ExistParameter(parameterId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simMethodService!.GetParameterById(parameterId));
        _mockSimMethodDataAccess.Verify();
    }

    [TestMethod]
    public void GetParameterById_ShouldReturnParameter_WhenParameterExists()
    {
        var parameterId = Guid.NewGuid();
        var expectedParameter = new Parameter { Id = parameterId };

        _mockSimMethodDataAccess!.Setup(m => m.ExistParameter(parameterId)).Returns(true);
        _mockSimMethodDataAccess.Setup(m => m.GetParameterById(parameterId)).Returns(expectedParameter);

        var result = _simMethodService!.GetParameterById(parameterId);

        Assert.AreEqual(expectedParameter, result);
        _mockSimMethodDataAccess.Verify(m => m.GetParameterById(parameterId), Times.Once);
    }

    [TestMethod]
    public void GetVariableById_ShouldThrowException_WhenVariableDoesNotExist()
    {
        var variableId = Guid.NewGuid();
        _mockSimMethodDataAccess!.Setup(m => m.ExistVariableById(variableId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simMethodService!.GetVariableById(variableId));
        _mockSimMethodDataAccess.Verify();
    }

    [TestMethod]
    public void GetVariableById_ShouldReturnVariable_WhenVariableExists()
    {
        var variableId = Guid.NewGuid();
        var relatedMethodInstance = new SimMethod { Id = Guid.NewGuid() };
        var expectedVariable = new LocalVariable { Id = variableId, Name = "TestVariable", RelatedMethod = relatedMethodInstance };

        _mockSimMethodDataAccess!.Setup(m => m.ExistVariableById(variableId)).Returns(true);
        _mockSimMethodDataAccess.Setup(m => m.GetVariableById(variableId)).Returns(expectedVariable);

        var result = _simMethodService!.GetVariableById(variableId);

        Assert.AreEqual(expectedVariable, result);
        _mockSimMethodDataAccess.Verify(m => m.GetVariableById(variableId), Times.Once);
    }

    [TestMethod]
    public void AddLocalVariable_ShouldReturnSimAttribute_WhenValid()
    {
        var methodId = Guid.NewGuid();
        var localVariableId = Guid.NewGuid();
        var localVariable = new LocalVariable()
        {
            Id = localVariableId,
            RelatedMethod = new SimMethod() { Accesibility = SimAccesibility.Normal, Id = methodId },
            Name = "TestVariable",
            Reference = new SimClass() { Id = Guid.NewGuid(), Name = "TypeName" },
        };
        var expectedAttribute = new LocalVariable()
        {
            Id = localVariableId,
            RelatedMethod = new SimMethod() { Accesibility = SimAccesibility.Normal, Id = methodId },
            Name = "TestVariable",
            Reference = new SimClass() { Id = Guid.NewGuid(), Name = "TypeName" },
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);
        _mockSimMethodDataAccess!
            .Setup(m => m.MethodVariableRepeatedValues(methodId, localVariable))
            .Returns(false);
        _mockSimMethodDataAccess!
            .Setup(m => m.AddLocalVariable(methodId, localVariable))
            .Returns(expectedAttribute);

        var result = _simMethodService!.AddLocalVariable(methodId, localVariable);

        Assert.AreEqual(expectedAttribute, result);
        _mockSimMethodDataAccess.Verify(m => m.AddLocalVariable(methodId, localVariable), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void AddLocalVariable_MethodDoesNotExist_ThrowsNonExistentValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var localVariable = new LocalVariable()
        {
            Id = Guid.NewGuid(),
            RelatedMethod = new SimMethod() { Accesibility = SimAccesibility.Normal, Id = Guid.NewGuid() },
            Name = "TestVariable",
            Reference = new SimClass() { Id = Guid.NewGuid(), Name = "Name" },
        };
        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(false);
        _mockSimMethodDataAccess!
            .Setup(m => m.MethodVariableRepeatedValues(methodId, localVariable))
            .Returns(false);
        _simMethodService!.AddLocalVariable(methodId, localVariable);
        _mockSimMethodDataAccess.Verify();
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void AddLocalVariable_LocalVariableNameRepeated_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var localVariable = new LocalVariable()
        {
            Id = Guid.NewGuid(),
            RelatedMethod = new SimMethod() { Accesibility = SimAccesibility.Normal, Id = Guid.NewGuid() },
            Name = "TestVariable",
            Reference = new SimClass() { Id = Guid.NewGuid(), Name = "Name" },
        };
        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);
        _mockSimMethodDataAccess!
            .Setup(m => m.MethodVariableRepeatedValues(methodId, localVariable))
            .Returns(true);

        _simMethodService!.AddLocalVariable(methodId, localVariable);
        _mockSimMethodDataAccess.Verify();
    }

    [TestMethod]
    public void AddLocalVariable_ValidInput_ReturnsLocalVariable()
    {
        var methodId = Guid.NewGuid();
        var localVariable = new LocalVariable()
        {
            Id = Guid.NewGuid(),
            RelatedMethod = new SimMethod() { Accesibility = SimAccesibility.Normal, Id = methodId },
            Name = "TestVariable",
            Reference = new SimClass() { Id = Guid.NewGuid(), Name = "TypeName" },
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);
        _mockSimMethodDataAccess!
            .Setup(m => m.MethodVariableRepeatedValues(methodId, localVariable))
            .Returns(false);
        _mockSimMethodDataAccess!
            .Setup(m => m.AddLocalVariable(methodId, localVariable))
            .Returns(localVariable);

        var result = _simMethodService!.AddLocalVariable(methodId, localVariable);

        Assert.AreEqual(localVariable, result);
        _mockSimMethodDataAccess.Verify(m => m.AddLocalVariable(methodId, localVariable), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void DeleteMethod_MethodDoesNotExist_ThrowsNonExistentValueLogicException()
    {
        var methodId = Guid.NewGuid();

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(false);

        _simMethodService!.DeleteMethod(methodId);
        _mockSimMethodDataAccess.Verify();
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void AddMethod_ShouldThrowException_WhenIsAbstractAndClassIsInUse()
    {
        var idClass = Guid.NewGuid();
        var method = new SimMethod { Accesibility = SimAccesibility.Abstract, Name = "AbstractMethod" };
        var simClass = new SimClass
        {
            Id = idClass,
            Name = "TestClass",
            State = SimAccesibility.Normal,
        };
        _mockSimClassDataAccess!
            .Setup(m => m.ExistSimClassById(idClass))
            .Returns(true);
        _mockSimClassDataAccess!
            .Setup(m => m.GetSimClassById(idClass))
            .Returns(simClass);
        _mockSimClassDataAccess!
            .Setup(m => m.InUseByOther(idClass))
            .Returns(true);
        _mockSimMethodDataAccess!
            .Setup(m => m.ExistsMethodInClass(idClass, method))
            .Returns(false);
        _mockSimMethodDataAccess!
            .Setup(m => m.CreateMethod(idClass, method))
            .Returns(method);

        _mockSimClassDataAccess!
            .Setup(m => m.UpdateSimClass(It.IsAny<SimClass>()))
            .Verifiable();

        _simMethodService!.AddMethod(idClass, method);
        _mockSimMethodDataAccess.Verify();
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void SignatureStaticExistsInClass_WhenNoMatchingMethod_ShouldThrowException()
    {
        var methodId = Guid.NewGuid();
        var staticClassId = Guid.NewGuid();

        var signature = new Signature
        {
            Name = "MissingMethod",
            Parameters = [],
        };

        var staticClass = new SimClass
        {
            Id = staticClassId,
            Name = "StaticClass",
            Methods = [],
        };

        var invokingMethod = new SimMethod
        {
            Id = methodId,
            RelatedClass = new SimClass(),
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(invokingMethod);

        _simMethodService!.SignatureStaticExistsInClass(staticClass, methodId, signature);
    }

    [TestMethod]
    public void ValidateStaticAttributeAccessibility_WithPrivateAttributeFromSameClass_ShouldNotThrowException()
    {
        var methodId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var sameClassId = Guid.NewGuid();

        var sameClass = new SimClass
        {
            Id = sameClassId,
            Name = "SameClass",
        };

        var method = new SimMethod
        {
            Id = methodId,
            RelatedClass = sameClass,
        };

        var privateStaticAttribute = new SimAttribute
        {
            Id = attributeId,
            Name = "PrivateStaticAttr",
            Privacity = SimPrivacity.Private,
            IsStatic = true,
            RelatedClass = sameClass,
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _simMethodService!.ValidateStaticAttributeAccessibility(privateStaticAttribute, methodId);

        _mockSimMethodDataAccess.Verify(m => m.GetMethodById(methodId), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeLogic))]
    public void ValidateStaticAttributeAccessibility_WithPrivateAttributeFromDifferentClass_ShouldThrowException()
    {
        var methodId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var callingClassId = Guid.NewGuid();
        var attributeOwnerClassId = Guid.NewGuid();

        var attributeOwnerClass = new SimClass
        {
            Id = attributeOwnerClassId,
            Name = "AttributeOwnerClass",
        };

        var callingClass = new SimClass
        {
            Id = callingClassId,
            Name = "CallingClass",
        };

        var method = new SimMethod
        {
            Id = methodId,
            RelatedClass = callingClass,
        };

        var privateStaticAttribute = new SimAttribute
        {
            Id = attributeId,
            Name = "PrivateStaticAttr",
            Privacity = SimPrivacity.Private,
            IsStatic = true,
            RelatedClass = attributeOwnerClass,
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _simMethodService!.ValidateStaticAttributeAccessibility(privateStaticAttribute, methodId);
    }

    [TestMethod]
    public void ValidateStaticAttributeAccessibility_WithProtectedAttributeFromSameClass_ShouldNotThrowException()
    {
        var methodId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var sameClassId = Guid.NewGuid();

        var sameClass = new SimClass
        {
            Id = sameClassId,
            Name = "SameClass",
        };

        var method = new SimMethod
        {
            Id = methodId,
            RelatedClass = sameClass,
        };

        var protectedStaticAttribute = new SimAttribute
        {
            Id = attributeId,
            Name = "ProtectedStaticAttr",
            Privacity = SimPrivacity.Protected,
            IsStatic = true,
            RelatedClass = sameClass,
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _simMethodService!.ValidateStaticAttributeAccessibility(protectedStaticAttribute, methodId);

        _mockSimMethodDataAccess.Verify(m => m.GetMethodById(methodId), Times.Once);
    }

    [TestMethod]
    public void ValidateStaticAttributeAccessibility_WithProtectedAttributeFromBaseClass_ShouldNotThrowException()
    {
        var methodId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var derivedClassId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();

        var baseClass = new SimClass
        {
            Id = baseClassId,
            Name = "BaseClass",
        };

        var derivedClass = new SimClass
        {
            Id = derivedClassId,
            Name = "DerivedClass",
            BaseClassId = baseClassId,
        };

        var method = new SimMethod
        {
            Id = methodId,
            RelatedClass = derivedClass,
        };

        var protectedStaticAttribute = new SimAttribute
        {
            Id = attributeId,
            Name = "ProtectedStaticAttr",
            Privacity = SimPrivacity.Protected,
            IsStatic = true,
            RelatedClass = baseClass,
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _mockSimClassDataAccess!
            .Setup(m => m.IsClassBaseOfOrSameAs(It.IsAny<SimClass>(), It.IsAny<SimClass>()))
            .Returns(true);

        _simMethodService!.ValidateStaticAttributeAccessibility(protectedStaticAttribute, methodId);

        _mockSimMethodDataAccess.Verify(m => m.GetMethodById(methodId), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeLogic))]
    public void ValidateStaticAttributeAccessibility_WithProtectedAttributeFromUnrelatedClass_ShouldThrowException()
    {
        var methodId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var unrelatedClassId = Guid.NewGuid();
        var attributeOwnerClassId = Guid.NewGuid();

        var attributeOwnerClass = new SimClass
        {
            Id = attributeOwnerClassId,
            Name = "AttributeOwnerClass",
        };

        var unrelatedClass = new SimClass
        {
            Id = unrelatedClassId,
            Name = "UnrelatedClass",
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Default base
        };

        var method = new SimMethod
        {
            Id = methodId,
            RelatedClass = unrelatedClass,
        };

        var protectedStaticAttribute = new SimAttribute
        {
            Id = attributeId,
            Name = "ProtectedStaticAttr",
            Privacity = SimPrivacity.Protected,
            IsStatic = true,
            RelatedClass = attributeOwnerClass,
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _mockSimClassDataAccess!
            .Setup(m => m.IsClassBaseOfOrSameAs(It.IsAny<SimClass>(), It.IsAny<SimClass>()))
            .Returns(false);

        _simMethodService!.ValidateStaticAttributeAccessibility(protectedStaticAttribute, methodId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeLogic))]
    public void MethodInheritsAttribute_WhenNoInheritanceRelation_ShouldThrowException()
    {
        var methodClassId = Guid.NewGuid();
        var attributeClassId = Guid.NewGuid();
        var parameterTypeId = Guid.NewGuid();
        var variableTypeId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClass = new SimClass { Id = methodClassId, Name = "MethodClass" },
            Parameters = [
                new Parameter
                {
                    Id = Guid.NewGuid(),
                    Name = "param1",
                    ReferenceId = parameterTypeId,
                    Reference = new SimClass { Id = parameterTypeId, Name = "ParamType" },
                }

            ],
            LocalVariables = [
                new LocalVariable
                {
                    Id = Guid.NewGuid(),
                    Name = "localVar1",
                    ReferenceId = variableTypeId,
                    Reference = new SimClass { Id = variableTypeId, Name = "VarType" }
                }

            ],
        };

        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "TestAttribute",
            RelatedClass = new SimClass { Id = attributeClassId, Name = "AttributeClass" },
            RelatedClassId = attributeClassId,
        };

        _mockSimClassDataAccess!
            .Setup(m => m.ClassInheritAttribute(methodClassId, attribute.Id, 0))
            .Returns(false);

        _mockSimClassDataAccess
            .Setup(m => m.ClassInheritAttribute(parameterTypeId, attribute.Id, 0))
            .Returns(false);

        _mockSimClassDataAccess
            .Setup(m => m.ClassInheritAttribute(variableTypeId, attribute.Id, 0))
            .Returns(false);

        _simMethodService!.MethodInheritsAttribute(method, attribute);
    }

    [TestMethod]
    public void MethodInheritsAttribute_Returns_WhenParameterTypeInheritsAttribute()
    {
        var methodClassId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var parameterTypeId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClass = new SimClass { Id = methodClassId, Name = "MethodClass" },
            Parameters = [
                new Parameter
                {
                    Id = Guid.NewGuid(),
                    Name = "param1",
                    ReferenceId = parameterTypeId,
                    Reference = new SimClass { Id = parameterTypeId, Name = "ParamType" },
                }

            ],
            LocalVariables = [],
        };

        var attribute = new SimAttribute
        {
            Id = attributeId,
            Name = "TestAttribute",
            RelatedClass = new SimClass { Id = Guid.NewGuid(), Name = "AttributeClass" },
            RelatedClassId = attributeId,
        };

        _mockSimClassDataAccess!
            .Setup(m => m.ClassInheritAttribute(parameterTypeId, attributeId, 0))
            .Returns(true);

        _simMethodService!.MethodInheritsAttribute(method, attribute);
    }

    [TestMethod]
    public void MethodInheritsAttribute_Returns_WhenLocalVariableTypeInheritsAttribute()
    {
        var methodClassId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var variableTypeId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClass = new SimClass { Id = methodClassId, Name = "MethodClass" },
            Parameters = [],
            LocalVariables = [
                new LocalVariable
                {
                    Id = Guid.NewGuid(),
                    Name = "localVar1",
                    ReferenceId = variableTypeId,
                    Reference = new SimClass { Id = variableTypeId, Name = "VarType" }
                }

            ],
        };

        var attribute = new SimAttribute
        {
            Id = attributeId,
            Name = "TestAttribute",
            RelatedClass = new SimClass { Id = Guid.NewGuid(), Name = "AttributeClass" },
            RelatedClassId = attributeId,
        };

        _mockSimClassDataAccess!
            .Setup(m => m.ClassInheritAttribute(variableTypeId, attributeId, 0))
            .Returns(true);

        _simMethodService!.MethodInheritsAttribute(method, attribute);
    }

    [TestMethod]
    public void MethodInheritsAttribute_Returns_WhenMethodClassInheritsAttribute()
    {
        var methodClassId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClass = new SimClass { Id = methodClassId, Name = "MethodClass" },
            Parameters = [],
            LocalVariables = [],
        };

        var attribute = new SimAttribute
        {
            Id = attributeId,
            Name = "TestAttribute",
            RelatedClass = new SimClass { Id = Guid.NewGuid(), Name = "AttributeClass" },
            RelatedClassId = attributeId,
        };

        _mockSimClassDataAccess!
            .Setup(m => m.ClassInheritAttribute(methodClassId, attributeId, 0))
            .Returns(true);

        _simMethodService!.MethodInheritsAttribute(method, attribute);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void AddMethod_ShouldThrowException_WhenAbstractMethodAndClassInUse()
    {
        var idClass = Guid.NewGuid();
        var method = new SimMethod { Accesibility = SimAccesibility.Abstract, Name = "AbstractMethod" };
        var simClass = new SimClass { Id = idClass, State = SimAccesibility.Normal };

        _mockSimClassDataAccess!.Setup(m => m.ExistSimClassById(idClass)).Returns(true);
        _mockSimClassDataAccess.Setup(m => m.GetSimClassById(idClass)).Returns(simClass);
        _mockSimMethodDataAccess!.Setup(m => m.ExistsMethodInClass(idClass, method)).Returns(false);
        _mockSimClassDataAccess.Setup(m => m.InUseByOther(idClass)).Returns(true);

        _simMethodService!.AddMethod(idClass, method);
    }

    [TestMethod]
    public void AddMethod_ShouldCallIsValidVirtualOverride()
    {
        var idClass = Guid.NewGuid();
        var method = new SimMethod
        {
            Accesibility = SimAccesibility.Normal,
            Name = "NormalMethod",
            Id = Guid.NewGuid(),
            RelatedClass = new SimClass { Id = idClass, Name = "TestClass" }
        };
        var simClass = new SimClass { Id = idClass, State = SimAccesibility.Normal };

        _mockSimClassDataAccess!.Setup(m => m.ExistSimClassById(idClass)).Returns(true);
        _mockSimClassDataAccess.Setup(m => m.GetSimClassById(idClass)).Returns(simClass);
        _mockSimMethodDataAccess!.Setup(m => m.ExistsMethodInClass(idClass, It.IsAny<SimMethod>())).Returns(false);
        _mockSimClassDataAccess.Setup(m => m.InUseByOther(idClass)).Returns(false);
        _mockExectuionDataAccess!
            .Setup(m => m.CanOverrideFromBaseClass(It.IsAny<Guid>(), It.IsAny<SimMethod>()))
            .Returns(true);
        _mockExectuionDataAccess!
            .Setup(m => m.FindSealedMethodInHierarchyFromBaseClass(It.IsAny<Guid>(), It.IsAny<SimMethod>()))
            .Returns((SimMethod?)null);
        _mockSimMethodDataAccess!
            .Setup(m => m.CreateMethod(idClass, method))
            .Returns(method);

        var result = _simMethodService!.AddMethod(idClass, method);

        Assert.AreEqual(method, result);
        _mockSimMethodDataAccess.Verify(m => m.CreateMethod(idClass, method), Times.Once);
    }

    [TestMethod]
    public void AddMethod_ShouldSetClassToAbstractAndUpdate_WhenAddingAbstractMethod()
    {
        var idClass = Guid.NewGuid();
        var method = new SimMethod
        {
            Accesibility = SimAccesibility.Abstract,
            Name = "AbstractMethod",
            Id = Guid.NewGuid(),
            RelatedClass = new SimClass { Id = idClass, Name = "TestClass" }
        };
        var simClass = new SimClass { Id = idClass, State = SimAccesibility.Normal };

        _mockSimClassDataAccess!.Setup(m => m.ExistSimClassById(idClass)).Returns(true);
        _mockSimClassDataAccess.Setup(m => m.GetSimClassById(idClass)).Returns(simClass);
        _mockSimMethodDataAccess!.Setup(m => m.ExistsMethodInClass(idClass, It.IsAny<SimMethod>())).Returns(false);
        _mockSimClassDataAccess.Setup(m => m.InUseByOther(idClass)).Returns(false);
        _mockExectuionDataAccess!
            .Setup(m => m.CanOverrideFromBaseClass(It.IsAny<Guid>(), It.IsAny<SimMethod>()))
            .Returns(true);
        _mockExectuionDataAccess!
            .Setup(m => m.FindSealedMethodInHierarchyFromBaseClass(It.IsAny<Guid>(), It.IsAny<SimMethod>()))
            .Returns((SimMethod?)null);
        _mockSimClassDataAccess!
            .Setup(m => m.UpdateSimClass(It.Is<SimClass>(c => c.State == SimAccesibility.Abstract)))
            .Verifiable();
        _mockSimMethodDataAccess!
            .Setup(m => m.CreateMethod(idClass, method))
            .Returns(method);

        var result = _simMethodService!.AddMethod(idClass, method);

        Assert.AreEqual(method, result);
        _mockSimClassDataAccess.Verify(m => m.UpdateSimClass(It.Is<SimClass>(c => c.State == SimAccesibility.Abstract)), Times.Once);
        _mockSimMethodDataAccess.Verify(m => m.CreateMethod(idClass, method), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void IsValidVirtualOverride_ShouldThrow_WhenCannotOverrideFromBaseOrInterfaces()
    {
        var idClassBase = Guid.NewGuid();
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass", BaseClassId = idClassBase };
        var method = new SimMethod { Name = "TestMethod" };

        _mockExectuionDataAccess!
            .Setup(m => m.CanOverrideFromBaseClass(idClassBase, method))
            .Returns(false);

        _mockExectuionDataAccess!
            .Setup(m => m.CanOverrideFromImplementedInterfaces(simClass, method))
            .Returns(false);

        _simMethodService!.IsValidVirtualOverride(simClass, method);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void IsValidVirtualOverride_ShouldThrow_WhenSealedMethodFoundInHierarchy()
    {
        var idClassBase = Guid.NewGuid();
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass", BaseClassId = idClassBase };
        var method = new SimMethod { Name = "TestMethod" };
        var sealedMethod = new SimMethod { Name = "SealedMethod" };

        _mockExectuionDataAccess!
            .Setup(m => m.CanOverrideFromBaseClass(idClassBase, method))
            .Returns(true);

        _mockExectuionDataAccess!
            .Setup(m => m.CanOverrideFromImplementedInterfaces(simClass, method))
            .Returns(false);

        _mockExectuionDataAccess!
            .Setup(m => m.FindSealedMethodInHierarchyFromBaseClass(idClassBase, method))
            .Returns(sealedMethod);

        _simMethodService!.IsValidVirtualOverride(simClass, method);
    }

    [TestMethod]
    public void IsValidVirtualOverride_ShouldNotThrow_WhenOverrideIsValid()
    {
        var idClassBase = Guid.NewGuid();
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass", BaseClassId = idClassBase };
        var method = new SimMethod { Name = "TestMethod" };

        _mockExectuionDataAccess!
            .Setup(m => m.CanOverrideFromBaseClass(idClassBase, method))
            .Returns(true);

        _mockExectuionDataAccess!
            .Setup(m => m.CanOverrideFromImplementedInterfaces(simClass, method))
            .Returns(false);

        _mockExectuionDataAccess!
            .Setup(m => m.FindSealedMethodInHierarchyFromBaseClass(idClassBase, method))
            .Returns((SimMethod?)null);

        _simMethodService!.IsValidVirtualOverride(simClass, method);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void AddMethodParameter_MethodDoesNotExist_ThrowsNonExistentValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Reference = new SimClass { Id = Guid.NewGuid(), Name = "TypeName" }
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.GetMethodById(methodId))
            .Returns((SimMethod)null);

        _simMethodService!.AddMethodParameter(methodId, parameter);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void AddMethodParameter_VirtualMethod_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Reference = new SimClass { Id = Guid.NewGuid(), Name = "TypeName" }
        };

        var method = new SimMethod
        {
            Id = methodId,
            IsVirtual = true,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _simMethodService!.AddMethodParameter(methodId, parameter);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void AddMethodParameter_AbstractMethod_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Reference = new SimClass { Id = Guid.NewGuid(), Name = "TypeName" }
        };

        var method = new SimMethod
        {
            Id = methodId,
            Accesibility = SimAccesibility.Abstract,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _simMethodService!.AddMethodParameter(methodId, parameter);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void AddMethodParameter_InterfaceMethod_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Reference = new SimClass { Id = Guid.NewGuid(), Name = "TypeName" }
        };

        var method = new SimMethod
        {
            Id = methodId,
            Accesibility = SimAccesibility.Interface,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _simMethodService!.AddMethodParameter(methodId, parameter);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void AddMethodParameter_ParameterNameRepeated_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Reference = new SimClass { Id = Guid.NewGuid(), Name = "TypeName" }
        };

        var method = new SimMethod
        {
            Id = methodId,
            IsVirtual = false,
            Accesibility = SimAccesibility.Normal,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _mockSimMethodDataAccess!
            .Setup(m => m.MethodParameterRepeatedValues(methodId, parameter))
            .Returns(true);

        _simMethodService!.AddMethodParameter(methodId, parameter);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void AddMethodParameter_MethodInUseByInvocations_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Reference = new SimClass { Id = Guid.NewGuid(), Name = "TypeName" }
        };

        var method = new SimMethod
        {
            Id = methodId,
            IsVirtual = false,
            Accesibility = SimAccesibility.Normal,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _mockSimMethodDataAccess!
            .Setup(m => m.MethodParameterRepeatedValues(methodId, parameter))
            .Returns(false);

        _mockSimMethodDataAccess!
            .Setup(m => m.MethodInUseByInvocations(methodId))
            .Returns(true);

        _simMethodService!.AddMethodParameter(methodId, parameter);
    }

    [TestMethod]
    public void AddMethodParameter_ValidInput_ReturnsParameter()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Reference = new SimClass { Id = Guid.NewGuid(), Name = "TypeName" }
        };

        var method = new SimMethod
        {
            Id = methodId,
            IsVirtual = false,
            Accesibility = SimAccesibility.Normal,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _mockSimMethodDataAccess!
            .Setup(m => m.MethodParameterRepeatedValues(methodId, parameter))
            .Returns(false);

        _mockSimMethodDataAccess!
            .Setup(m => m.MethodInUseByInvocations(methodId))
            .Returns(false);

        _mockSimMethodDataAccess!
            .Setup(m => m.AddMethodParameter(methodId, parameter))
            .Returns(parameter);

        var result = _simMethodService!.AddMethodParameter(methodId, parameter);

        Assert.AreEqual(parameter, result);
        _mockSimMethodDataAccess.Verify(m => m.AddMethodParameter(methodId, parameter), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void DeleteMethod_VirtualMethod_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var method = new SimMethod
        {
            Id = methodId,
            IsVirtual = true,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _simMethodService!.DeleteMethod(methodId);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void DeleteMethod_AbstractMethod_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var method = new SimMethod
        {
            Id = methodId,
            Accesibility = SimAccesibility.Abstract,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _simMethodService!.DeleteMethod(methodId);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void DeleteMethod_InterfaceMethod_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var method = new SimMethod
        {
            Id = methodId,
            Accesibility = SimAccesibility.Interface,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _simMethodService!.DeleteMethod(methodId);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void DeleteMethod_MethodInUse_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var method = new SimMethod
        {
            Id = methodId,
            IsVirtual = false,
            Accesibility = SimAccesibility.Normal,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _mockSimMethodDataAccess
            .Setup(m => m.MethodIsInUse(methodId))
            .Returns(true);

        _simMethodService!.DeleteMethod(methodId);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void DeleteMethod_MethodInUseByInvocations_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var method = new SimMethod
        {
            Id = methodId,
            IsVirtual = false,
            Accesibility = SimAccesibility.Normal,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _mockSimMethodDataAccess
            .Setup(m => m.MethodIsInUse(methodId))
            .Returns(false);

        _mockSimMethodDataAccess
            .Setup(m => m.MethodInUseByInvocations(methodId))
            .Returns(true);

        _simMethodService!.DeleteMethod(methodId);
    }

    [TestMethod]
    public void DeleteMethod_ValidMethod_DeletesSuccessfully()
    {
        var methodId = Guid.NewGuid();
        var method = new SimMethod
        {
            Id = methodId,
            IsVirtual = false,
            Accesibility = SimAccesibility.Normal,
            RelatedClass = new SimClass()
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);

        _mockSimMethodDataAccess
            .Setup(m => m.GetMethodById(methodId))
            .Returns(method);

        _mockSimMethodDataAccess
            .Setup(m => m.MethodIsInUse(methodId))
            .Returns(false);

        _mockSimMethodDataAccess
            .Setup(m => m.MethodInUseByInvocations(methodId))
            .Returns(false);

        _mockSimMethodDataAccess
            .Setup(m => m.DeleteMethod(methodId))
            .Verifiable();

        _simMethodService!.DeleteMethod(methodId);

        _mockSimMethodDataAccess.Verify(m => m.DeleteMethod(methodId), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void AddMethod_AbstractClassInUse_ThrowsInUseValueLogicException()
    {
        var classId = Guid.NewGuid();
        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            Accesibility = SimAccesibility.Normal
        };

        var simClass = new SimClass
        {
            Id = classId,
            Name = "TestClass",
            State = SimAccesibility.Abstract
        };

        _mockSimClassDataAccess!
            .Setup(m => m.ExistSimClassById(classId))
            .Returns(true);

        _mockSimClassDataAccess
            .Setup(m => m.GetSimClassById(classId))
            .Returns(simClass);

        _mockSimClassDataAccess
            .Setup(m => m.InUseByOther(classId))
            .Returns(true);

        _simMethodService!.AddMethod(classId, method);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void AddMethod_InterfaceMethodInClassInUse_ThrowsInUseValueLogicException()
    {
        var classId = Guid.NewGuid();
        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            Accesibility = SimAccesibility.Interface
        };

        var simClass = new SimClass
        {
            Id = classId,
            Name = "TestClass",
            State = SimAccesibility.Normal
        };

        _mockSimClassDataAccess!
            .Setup(m => m.ExistSimClassById(classId))
            .Returns(true);

        _mockSimClassDataAccess
            .Setup(m => m.GetSimClassById(classId))
            .Returns(simClass);

        _mockSimClassDataAccess
            .Setup(m => m.InUseByOther(classId))
            .Returns(true);

        _simMethodService!.AddMethod(classId, method);
    }

    [TestMethod]
    public void AddMethod_AbstractClassNotInUse_MethodCreatedSuccessfully()
    {
        var classId = Guid.NewGuid();
        var returnTypeId = Guid.NewGuid();
        var returnType = new SimClass { Id = returnTypeId, Name = "ReturnType" };

        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            Accesibility = SimAccesibility.Normal,
            ReturnType = returnType,
            ReturnTypeId = returnTypeId,
            Parameters = []
        };

        var simClass = new SimClass
        {
            Id = classId,
            Name = "TestClass",
            State = SimAccesibility.Abstract
        };

        _mockSimClassDataAccess!
            .Setup(m => m.ExistSimClassById(classId))
            .Returns(true);

        _mockSimClassDataAccess
            .Setup(m => m.GetSimClassById(classId))
            .Returns(simClass);

        _mockSimClassDataAccess
            .Setup(m => m.InUseByOther(classId))
            .Returns(false);

        _mockExectuionDataAccess!
            .Setup(m => m.CanOverrideFromBaseClass(It.IsAny<Guid>(), It.IsAny<SimMethod>()))
            .Returns(true);

        _mockExectuionDataAccess
            .Setup(m => m.FindSealedMethodInHierarchyFromBaseClass(It.IsAny<Guid>(), It.IsAny<SimMethod>()))
            .Returns((SimMethod)null);

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistsMethodInClass(classId, method))
            .Returns(false);

        _mockSimMethodDataAccess
            .Setup(m => m.CreateMethod(classId, method))
            .Returns(method);

        var result = _simMethodService!.AddMethod(classId, method);

        Assert.AreEqual(method, result);
        _mockSimMethodDataAccess.Verify(m => m.CreateMethod(classId, method), Times.Once);
    }
}
