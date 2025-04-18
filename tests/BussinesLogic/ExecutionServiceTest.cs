using BussinesLogic;
using BussinesLogic.Exceptions;
using Domain;
using IDataAccess;
using Moq;

namespace Tests.BussinesLogic;

[TestClass]
public class ExecutionServiceTest
{
    private Mock<ISimClassDataAccess>? _mockSimClassDataAccess;
    private ExecutionService? _executionService;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimClassDataAccess = new Mock<ISimClassDataAccess>(MockBehavior.Strict);
        _executionService = new ExecutionService(_mockSimClassDataAccess.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void ExecuteMethod_ShouldThrow_WhenInstanceClassNotFound()
    {
        var methodName = "TestMethod";
        var parameters = new List<Parameter>();
        var idInstanceType = Guid.NewGuid();
        var idReferenceType = Guid.NewGuid();
        var instanceName = "TestInstance";

        _mockSimClassDataAccess!
            .Setup(m => m.ExistSimClassById(idInstanceType))
            .Returns(false);

        _mockSimClassDataAccess!
    .Setup(m => m.ExistSimClassById(idReferenceType))
    .Returns(true);

        _executionService!.ExecuteMethod(methodName, parameters, idInstanceType, idReferenceType, instanceName);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void ExecuteMethod_ShouldThrow_WhenReferenceClassNotFound()
    {
        var methodName = "TestMethod";
        var parameters = new List<Parameter>();
        var idInstanceType = Guid.NewGuid();
        var idReferenceType = Guid.NewGuid();
        var instanceName = "TestInstance";

        _mockSimClassDataAccess!
            .Setup(m => m.ExistSimClassById(idInstanceType))
            .Returns(true);

        _mockSimClassDataAccess!
            .Setup(m => m.ExistSimClassById(idReferenceType))
            .Returns(false);

        _executionService!.ExecuteMethod(methodName, parameters, idInstanceType, idReferenceType, instanceName);
    }
}
