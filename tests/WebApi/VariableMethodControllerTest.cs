using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class VariableMethodControllerTest
{
    private Mock<IMethodAdapter>? _mockVariableMethodAdapter;
    private VariableMethodController? _variableMethodController;

    [TestInitialize]
    public void Setup()
    {
        _mockVariableMethodAdapter = new Mock<IMethodAdapter>();
        _variableMethodController = new VariableMethodController(_mockVariableMethodAdapter.Object);
    }

    [TestMethod]
    public void GetVariableMethodCorrectly_ShouldOk()
    {
        var id = Guid.NewGuid();
        var expectedResponse = new VariableResponse() { Id = id, Name = "Test Variable Method" };
        _mockVariableMethodAdapter?.Setup(m => m.GetVariable(id)).Returns(expectedResponse);

        var result = _variableMethodController?.GetVariable(id);
        _mockVariableMethodAdapter?.Verify(m => m.GetVariable(id), Times.Once);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(expectedResponse, okResult?.Value);
    }
}
