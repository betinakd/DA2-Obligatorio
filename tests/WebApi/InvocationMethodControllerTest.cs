using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class InvocationMethodControllerTest
{
    private Mock<IMethodInvocationAdapter>? _mockInvocationMethodAdapter;
    private InvocationsMethodController? _invocationsMethodController;

    [TestInitialize]
    public void Setup()
    {
        _mockInvocationMethodAdapter = new Mock<IMethodInvocationAdapter>();
        _invocationsMethodController = new InvocationsMethodController(_mockInvocationMethodAdapter.Object);
    }

    [TestMethod]
    public void GetInvocationMethodCorrectly_ShouldThrowOk()
    {
        var id = Guid.NewGuid();
        var expectedResponse = new InvocationResponse { IdReference = id, MethodName = "TestMethod", Parametros = [] };
        _mockInvocationMethodAdapter?.Setup(m => m.GetInvocation(id)).Returns(expectedResponse);

        var result = _invocationsMethodController?.GetInvocation(id) as OkObjectResult;

        _mockInvocationMethodAdapter?.Verify(m => m.GetInvocation(id), Times.Once);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResponse, result.Value);
    }
}
