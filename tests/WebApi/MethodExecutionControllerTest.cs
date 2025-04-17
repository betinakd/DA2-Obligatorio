using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class MethodExecutionControllerTest
{
    private Mock<IExecutionAdapter>? _mockExecutionAdapter;
    private MethodExecutionController? _executionsController;

    [TestInitialize]
    public void Setup()
    {
        _mockExecutionAdapter = new Mock<IExecutionAdapter>();
        _executionsController = new MethodExecutionController(_mockExecutionAdapter.Object);
    }

    [TestMethod]
    public void ExecuteMethodOk_ReturnsObjectWithExecution()
    {
        var body = new MethodExecutionRequest()
        {
            InstanceName = "Instance1",
            InstanceTypeId = Guid.NewGuid(),
            MethodName = "TestMethod",
            Parameters = [],
            ReferenceTypeId = Guid.NewGuid(),
        };
        var expectedResult = "Class1.TestMethod()";
        _mockExecutionAdapter?.Setup(adapter => adapter.ExecuteMethod(body)).Returns(expectedResult);

        var actionResult = _executionsController?.ExecuteMethod(body) as OkObjectResult;

        _mockExecutionAdapter?.Verify(adapter => adapter.ExecuteMethod(It.IsAny<MethodExecutionRequest>()), Times.Once);

        Assert.IsNotNull(actionResult);
        Assert.AreEqual(200, actionResult.StatusCode);
        Assert.AreEqual(expectedResult, actionResult.Value);
    }
}
