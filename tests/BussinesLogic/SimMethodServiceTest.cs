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
            _simMethodService!.AddInvocation(methodId, invocation)
        );
    }
}
