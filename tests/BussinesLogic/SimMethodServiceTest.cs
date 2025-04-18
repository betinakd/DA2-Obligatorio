using BussinesLogic;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Enums;
using IDataAccess;
using Moq;

namespace Tests.BussinesLogic;

[TestClass]
public class SimMethodServiceTest
{
    private Mock<ISimMethodDataAccess>? _mockSimMethodDataAccess;
    private Mock<ISimClassDataAccess>? _mockSimClassDataAccess;
    private SimMethodService? _simMethodService;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimMethodDataAccess = new Mock<ISimMethodDataAccess>(MockBehavior.Strict);
        _mockSimClassDataAccess = new Mock<ISimClassDataAccess>(MockBehavior.Strict);
        _simMethodService = new SimMethodService(_mockSimMethodDataAccess.Object, _mockSimClassDataAccess.Object);
    }

    [TestMethod]
    public void AddInvocation_ShouldThrowException_WhenMethodDoesNotExist()
    {
        var methodId = Guid.NewGuid();
        var invocation = new Invocation();

        _mockSimMethodDataAccess!.Setup(m => m.ExistMethodById(methodId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simMethodService!.AddInvocation(methodId, invocation));
    }

    [TestMethod]
    public void AddInvocation_ShouldReturnInvocation_WhenMethodExists()
    {
        var methodId = Guid.NewGuid();
        var newInvocation = new Invocation();
        var expectedInvocation = new Invocation();

        _mockSimMethodDataAccess!.Setup(m => m.ExistMethodById(methodId)).Returns(true);
        _mockSimMethodDataAccess.Setup(m => m.CreateInvocation(methodId, newInvocation)).Returns(expectedInvocation);

        var result = _simMethodService!.AddInvocation(methodId, newInvocation);

        Assert.AreEqual(expectedInvocation, result);
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
    }

    [TestMethod]
    public void AddMethod_ShouldThrowException_WhenMethodWithSameFirmExistsInClass()
    {
        var classId = Guid.NewGuid();
        var method = new SimMethod();

        _mockSimClassDataAccess!.Setup(m => m.ExistSimClassById(classId)).Returns(true);
        _mockSimMethodDataAccess!.Setup(m => m.ExistsMethodInClass(classId, method)).Returns(true);

        Assert.ThrowsException<InUseValueLogic>(() =>
            _simMethodService!.AddMethod(classId, method));
    }

    [TestMethod]
    public void AddMethod_ShouldReturnSimMethod_WhenClassExistsAndMethodIsUnique()
    {
        var classId = Guid.NewGuid();
        var method = new SimMethod()
        {
            Accesibility = SimAccesibility.Normal,
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            Privacity = SimPrivacity.Public,
            RelatedClass = new SimClass() { Id = classId, Name = "TestClass" },
            ReturnType = new SimClass() { Id = Guid.NewGuid(), Name = "TestClass" }
        };

        var expectedMethod = method;

        _mockSimClassDataAccess!.Setup(m => m.ExistSimClassById(classId)).Returns(true);
        _mockSimMethodDataAccess!.Setup(m => m.ExistsMethodInClass(classId, method)).Returns(false);
        _mockSimMethodDataAccess.Setup(m => m.CreateMethod(classId, method)).Returns(expectedMethod);

        var result = _simMethodService!.AddMethod(classId, method);

        Assert.AreEqual(expectedMethod, result);
        _mockSimMethodDataAccess.Verify(m => m.CreateMethod(classId, method), Times.Once);
    }

    [TestMethod]
    public void GetInvocationById_ShouldThrowException_WhenInvocationDoesNotExist()
    {
        var invocationId = Guid.NewGuid();
        _mockSimMethodDataAccess!.Setup(m => m.ExistInvocationById(invocationId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simMethodService!.GetInvocationById(invocationId));
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

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simMethodService!.GetMethodById(methodId));
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
            Type = new SimClass() { Id = Guid.NewGuid(), Name = "TypeName" }
        };
        var expectedAttribute = new LocalVariable()
        {
            Id = localVariableId,
            RelatedMethod = new SimMethod() { Accesibility = SimAccesibility.Normal, Id = methodId },
            Name = "TestVariable",
            Type = new SimClass() { Id = Guid.NewGuid(), Name = "TypeName" }
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
            Type = new SimClass() { Id = Guid.NewGuid(), Name = "Name" }
        };
        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(false);
        _mockSimMethodDataAccess!
.Setup(m => m.MethodVariableRepeatedValues(methodId, localVariable))
.Returns(false);
        _simMethodService!.AddLocalVariable(methodId, localVariable);
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
            Type = new SimClass() { Id = Guid.NewGuid(), Name = "Name" }
        };
        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);
        _mockSimMethodDataAccess!
            .Setup(m => m.MethodVariableRepeatedValues(methodId, localVariable))
            .Returns(true);

        _simMethodService!.AddLocalVariable(methodId, localVariable);
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
            Type = new SimClass() { Id = Guid.NewGuid(), Name = "TypeName" }
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
    public void AddMethodParameter_MethodDoesNotExist_ThrowsNonExistentValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter()
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Type = new SimClass() { Id = Guid.NewGuid(), Name = "TypeName" }
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(false);

        _simMethodService!.AddMethodParameter(methodId, parameter);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void AddMethodParameter_ParameterNameRepeated_ThrowsInUseValueLogicException()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter()
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Type = new SimClass() { Id = Guid.NewGuid(), Name = "TypeName" }
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);
        _mockSimMethodDataAccess!
            .Setup(m => m.MethodParameterRepeatedValues(methodId, parameter))
            .Returns(true);

        _simMethodService!.AddMethodParameter(methodId, parameter);
    }

    [TestMethod]
    public void AddMethodParameter_ValidInput_ReturnsParameter()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter()
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Type = new SimClass() { Id = Guid.NewGuid(), Name = "TypeName" }
        };

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);
        _mockSimMethodDataAccess!
            .Setup(m => m.MethodParameterRepeatedValues(methodId, parameter))
            .Returns(false);
        _mockSimMethodDataAccess!
            .Setup(m => m.AddMethodParameter(methodId, parameter))
            .Returns(parameter);

        var result = _simMethodService!.AddMethodParameter(methodId, parameter);

        Assert.AreEqual(parameter, result);
        _mockSimMethodDataAccess.Verify(m => m.AddMethodParameter(methodId, parameter), Times.Once);
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
    }

    [TestMethod]
    public void DeleteMethod_ValidId_DeletesMethod()
    {
        var methodId = Guid.NewGuid();

        _mockSimMethodDataAccess!
            .Setup(m => m.ExistMethodById(methodId))
            .Returns(true);
        _mockSimMethodDataAccess!
            .Setup(m => m.DeleteMethod(methodId))
            .Verifiable();

        _simMethodService!.DeleteMethod(methodId);

        _mockSimMethodDataAccess.Verify(m => m.DeleteMethod(methodId), Times.Once);
    }
}
