using BussinesLogic;
using Domain;
using IDataAccess;
using Moq;

namespace Tests.BussinesLogic;

[TestClass]
public class ExecutionServiceTest
{
    private Mock<IExecutionDataAccess>? _mockExecuteDataAccess;
    private ExecutionService? _executionService;

    [TestInitialize]
    public void Initialize()
    {
        _mockExecuteDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Strict);
        _executionService = new ExecutionService(_mockExecuteDataAccess.Object);
    }

    [TestMethod]
    public void ExecuteMethod_BasicMethodWithoutInvocations_ReturnsFormattedOutput()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClass = simClass,
            Invocations = []
        };
        var signature = new Signature { Name = "TestMethod", Parameters = [] };

        var mockRef = new Mock<Reference>();
        mockRef.Setup(r => r.GetSimClass()).Returns(simClass);
        mockRef.Setup(r => r.GetSignature(signature)).Returns("TestClass.TestMethod()");

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature))
            .Returns(method);

        var result = _executionService!.ExecuteMethod(mockRef.Object, mockRef.Object, signature);

        Assert.AreEqual("TestClass.TestMethod() -> TestClass.TestMethod()", result);
    }

    [TestMethod]
    public void ExecuteMethod_MethodNotFound_ReturnsErrorMessage()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        var signature = new Signature { Name = "NonExistentMethod", Parameters = [] };

        var mockRef = new Mock<Reference>();
        mockRef.Setup(r => r.GetSimClass()).Returns(simClass);

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature))
            .Returns((SimMethod)null);

        var result = _executionService!.ExecuteMethod(mockRef.Object, mockRef.Object, signature);

        Assert.AreEqual("Error: No se encontró el método NonExistentMethod en TestClass", result);
    }
}
