using Adapter.Exceptions;
using FluentAssertions;
using IAdapter;
using Microsoft.AspNetCore.Http;
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
        var variableRequest = new VariableRequest { MethodId = methodId, Name = "testVariable", ClassTypeId = Guid.NewGuid() };
        var variableResponse = new VariableResponse { Id = Guid.NewGuid(), Name = "test", MethodId = methodId, ClassTypeId = Guid.NewGuid() };
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
        var parameterRequest = new ParameterRequest { MethodId = methodId, Name = "testParameter", ClassTypeId = Guid.NewGuid() };
        var parameterResponse = new ParameterResponse { Id = Guid.NewGuid(), Name = "test", MethodId = methodId, ClassTypeId = Guid.NewGuid() };
        var expectedResponse = new CreatedParameterResponse { Message = "Parameter created successfully", Parameter = parameterResponse };

        _mockmethodAdapter?.Setup(m => m.CreateParameter(methodId, parameterRequest)).Returns(expectedResponse);

        var result = _attributeController?.CreateParameter(parameterRequest, methodId);
        _mockmethodAdapter?.Verify(m => m.CreateParameter(methodId, parameterRequest), Times.Once);

        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.AreEqual(expectedResponse.Parameter.Id, createdResult?.RouteValues["id"]);
        Assert.AreEqual(expectedResponse, createdResult?.Value);
    }

    [TestMethod]
    public void CreateMethodInvocationCorrectly_ShoulReturnCreated()
    {
        var methodId = Guid.NewGuid();

        var invocationRequest = new InvocationRequest { IdReference = methodId, MethodName = "testInvocation", Parametros = [] };
        var invocationResponse = new InvocationResponse { Id = Guid.NewGuid() };
        var expectedResponse = new CreatedInvocationResponse { Message = "Invocation created successfully", InvocationResponse = invocationResponse };

        _mockmethodAdapter?.Setup(m => m.CreateInvocation(methodId, invocationRequest)).Returns(expectedResponse);

        var result = _attributeController?.CreateInvocation(invocationRequest, methodId);
        _mockmethodAdapter?.Verify(m => m.CreateInvocation(methodId, invocationRequest), Times.Once);

        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.AreEqual(expectedResponse.InvocationResponse.Id, createdResult?.RouteValues["id"]);
        Assert.AreEqual(expectedResponse, createdResult?.Value);
    }

    [TestMethod]
    public void AddLocalVariableWithNullAttributes_ShouldReturnBadRequest()
    {
        var methodId = Guid.NewGuid();
        var parameterName = " ";
        var parameterType = " ";

        var request = new VariableRequest
        {
            MethodId = methodId,
            Name = parameterName,
            ClassTypeId = Guid.Empty,
        };
        _mockmethodAdapter
            .Setup(m => m.CreateVariable(methodId, request))
            .Throws(new InvalidAttribute("Local varible information cant be empty"));

        IActionResult result;
        try
        {
            result = _attributeController?.CreateVariables(request, methodId);
        }
        catch(InvalidAttribute ex)
        {
            // Simula el comportamiento del filtro
            result = new BadRequestObjectResult(new { Message = ex.Message });
        }

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var value = badRequestResult.Value;
        var expectedMessage = $"Local varible information cant be empty";
        var actualMessage = ((dynamic)value).Message;

        Assert.AreEqual(expectedMessage, actualMessage);
    }

    [TestMethod]
    public void AddParameterWithNullAttributes_ShouldReturnBadRequest()
    {
        var methodId = Guid.NewGuid();
        var parameterName = " ";
        var parameterType = " ";

        var request = new ParameterRequest
        {
            MethodId = methodId,
            Name = parameterName,
            ClassTypeId = Guid.Empty
        };
        _mockmethodAdapter
            .Setup(m => m.CreateParameter(methodId, request))
            .Throws(new InvalidAttribute("Parameter information cant be empty"));

        IActionResult result;
        try
        {
            result = _attributeController.CreateParameter(request, methodId);
        }
        catch(InvalidAttribute ex)
        {
            // Simula el comportamiento del filtro
            result = new BadRequestObjectResult(new { Message = ex.Message });
        }

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var value = badRequestResult.Value;
        var expectedMessage = $"Parameter information cant be empty";
        var actualMessage = ((dynamic)value).Message;

        Assert.AreEqual(expectedMessage, actualMessage);
    }
}
