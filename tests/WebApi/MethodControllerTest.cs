using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
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
    public void CreateMethodCorrectly_ShouldReturnCreated()
    {
        var id = Guid.NewGuid();
        var methodRequest = new MethodRequest() { Id = id, Name = "Test Method" };
        var expectedResponse = new CreatedMethodResponse() { Id = id, Message = "Method created successfully", MethodResponse = new MethodResponse() { Id = id } };
        _mockmethodAdapter?.Setup(m => m.CreateMethod(methodRequest)).Returns(expectedResponse);

        var result = _attributeController?.CreateMethod(methodRequest);
        _mockmethodAdapter?.Verify(m => m.CreateMethod(methodRequest), Times.Once);

        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        var createdResult = result as CreatedAtActionResult;
        Assert.AreEqual(expectedResponse, createdResult?.Value);
    }
}
