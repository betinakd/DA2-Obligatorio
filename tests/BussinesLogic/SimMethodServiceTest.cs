using BussinesLogic;
using BussinesLogic.Exceptions;
using Domain;
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

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simMethodService!.AddMethod(classId, method));
    }
}
