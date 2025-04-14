using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class MethodControllerTest
{
    private Mock<IMethodAdapter>? _mockmethodAdapter;
    private MethodController? _attributeController;

    [TestInitialize]
    public void Setup()
    {
        _mockmethodAdapter = new Mock<IMethodAdapter>();
        _attributeController = new MethodController(_mockmethodAdapter.Object);
    }

    [TestMethod]
    public void GetMethodWithCorrectId_ShouldReturnOk()
    {
        var methodId = Guid.NewGuid();
        var expectedResponse = new MethodResponse() { Id = methodId };
        _mockmethodAdapter?.Setup(m => m.GetMethod(methodId)).Returns(expectedResponse);

        var result = _attributeController?.GetMethod(methodId);
        _mockmethodAdapter?.Verify(m => m.GetMethod(methodId), Times.Once);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(expectedResponse, okResult?.Value);
    }

    [TestMethod]
    public void DeleteMethodCorrectly_ShouldReturnNoContent()
    {
        var methodId = Guid.NewGuid();
        _mockmethodAdapter?.Setup(m => m.DeleteMethod(methodId));

        var result = _attributeController?.DeleteMethod(methodId);
        _mockmethodAdapter?.Verify(m => m.DeleteMethod(methodId), Times.Once);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }
}
