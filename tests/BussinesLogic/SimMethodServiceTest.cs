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
}
