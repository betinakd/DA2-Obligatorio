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
    private Mock<IExecutionDataAccess>? _mockExecuteDataAccess;
    private ExecutionService? _executionService;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimClassDataAccess = new Mock<ISimClassDataAccess>(MockBehavior.Strict);
        _mockExecuteDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Strict);
        _executionService = new ExecutionService(_mockSimClassDataAccess.Object, _mockExecuteDataAccess.Object);
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

        _mockExecuteDataAccess!
        .Setup(m => m.ExecuteAbstractMethod(methodName, parameters, idInstanceType, idReferenceType))
        .Returns(false);

        _mockExecuteDataAccess!
    .Setup(m => m.FoundSealedMethod(methodName, parameters, idInstanceType, idReferenceType))
    .Returns(false);

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

        _mockExecuteDataAccess!
    .Setup(m => m.ExecuteAbstractMethod(methodName, parameters, idInstanceType, idReferenceType))
    .Returns(false);

        _mockExecuteDataAccess!
        .Setup(m => m.FoundSealedMethod(methodName, parameters, idInstanceType, idReferenceType))
        .Returns(false);

        _executionService!.ExecuteMethod(methodName, parameters, idInstanceType, idReferenceType, instanceName);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationLogic))]
    public void ExecuteMethod_ShouldThrow_WhenMethodIsAbstract()
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
            .Returns(true);

        _mockExecuteDataAccess!
            .Setup(m => m.ExecuteAbstractMethod(methodName, parameters, idInstanceType, idReferenceType))
            .Returns(true);

        _mockExecuteDataAccess!
.Setup(m => m.FoundSealedMethod(methodName, parameters, idInstanceType, idReferenceType))
.Returns(false);

        _executionService!.ExecuteMethod(methodName, parameters, idInstanceType, idReferenceType, instanceName);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationLogic))]
    public void ExecuteMethod_ShouldThrow_WhenMethodIsSealed()
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
            .Returns(true);

        _mockExecuteDataAccess!
            .Setup(m => m.ExecuteAbstractMethod(methodName, parameters, idInstanceType, idReferenceType))
            .Returns(false);

        _mockExecuteDataAccess!
            .Setup(m => m.FoundSealedMethod(methodName, parameters, idInstanceType, idReferenceType))
            .Returns(true);

        _executionService!.ExecuteMethod(methodName, parameters, idInstanceType, idReferenceType, instanceName);
    }
}
