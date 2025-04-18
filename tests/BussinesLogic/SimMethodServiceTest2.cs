using BussinesLogic;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Enums;
using IDataAccess;
using Moq;

namespace Tests.BussinesLogic;

[TestClass]
public class SimMethodServiceTest2
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
}
