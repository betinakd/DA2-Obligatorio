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
    public void DeleteMethodCorrectly_ShouldReturnNoContent()
    {
        var methodId = Guid.NewGuid();
        _mockmethodAdapter?.Setup(m => m.DeleteMethod(methodId));

        var result = _attributeController?.DeleteMethod(methodId);
        _mockmethodAdapter?.Verify(m => m.DeleteMethod(methodId), Times.Once);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public void CreateVariableCorrectly_ShouldReturnCreated()
    {
        var methodId = Guid.NewGuid();
        var variableRequest = new VariableRequest { MethodId = methodId, Name = "testVariable" };
        var variableResponse = new VariableResponse { Id = Guid.NewGuid(), Name = "test" };
        var expectedResponse = new CreatedVariableResponse { Message = "Variable created successfully", Variable = variableResponse };

        _mockmethodAdapter?.Setup(m => m.CreateVariable(methodId, variableRequest)).Returns(expectedResponse);

        var result = _attributeController?.CreateVariables(variableRequest, methodId);
        _mockmethodAdapter?.Verify(m => m.CreateVariable(methodId, variableRequest), Times.Once);

        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.AreEqual(expectedResponse.Variable.Id, createdResult?.RouteValues["id"]);
        Assert.AreEqual(expectedResponse, createdResult?.Value);
    }

    [TestMethod]
    public void CreateParameterCorrectly_ShouldReturnCreated()
    {
        var methodId = Guid.NewGuid();
        var parameterRequest = new ParameterRequest { MethodId = methodId, Name = "testParameter" };
        var parameterResponse = new ParameterResponse { Id = Guid.NewGuid(), Name = "test" };
        var expectedResponse = new CreatedParameterResponse { Message = "Parameter created successfully", Parameter = parameterResponse };

        _mockmethodAdapter?.Setup(m => m.CreateParameter(methodId, parameterRequest)).Returns(expectedResponse);

        var result = _attributeController?.CreateParameter(parameterRequest, methodId);
        _mockmethodAdapter?.Verify(m => m.CreateParameter(methodId, parameterRequest), Times.Once);

        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.AreEqual(expectedResponse.Parameter.Id, createdResult?.RouteValues["id"]);
        Assert.AreEqual(expectedResponse, createdResult?.Value);
    }
}
