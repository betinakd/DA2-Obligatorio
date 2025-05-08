using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class ParameterMethodControllerTest
{
    private Mock<IMethodAdapter>? _mockParameterMethodAdapter;
    private ParameterMethodController? _parameterMethodController;

    [TestInitialize]
    public void Setup()
    {
        _mockParameterMethodAdapter = new Mock<IMethodAdapter>();
        _parameterMethodController = new ParameterMethodController(_mockParameterMethodAdapter.Object);
    }

    [TestMethod]
    public void GetParameterMethodCorrectly_ShouldOk()
    {
        var id = Guid.NewGuid();
        var expectedResponse = new ParameterResponse() { Id = id, Name = "Test Variable Method" };
        _mockParameterMethodAdapter?.Setup(m => m.GetParameter(id)).Returns(expectedResponse);

        var result = _parameterMethodController?.GetParameter(id);
        _mockParameterMethodAdapter?.Verify(m => m.GetParameter(id), Times.Once);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(expectedResponse, okResult?.Value);
    }
}
