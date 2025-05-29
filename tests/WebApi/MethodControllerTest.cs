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
        var variableRequest = new VariablesRequest { Name = "testVariable", IdReference = Guid.NewGuid().ToString() };
        var variableResponse = new VariableResponse { Id = Guid.NewGuid(), Name = "test", MethodId = methodId, ReferenceId = Guid.NewGuid() };
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
        var parameterRequest = new ParameterRequest { Name = "testParameter", IdReference = Guid.NewGuid().ToString() };
        var parameterResponse = new ParameterResponse { Id = Guid.NewGuid(), Name = "test", MethodId = methodId, ReferenceId = Guid.NewGuid() };
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

        var invocationRequest = new InvocationRequest { MethodName = "testInvocation", Parameters = [] };
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

        var request = new VariablesRequest
        {
            Name = parameterName,
            IdReference = " ",
        };
        _mockmethodAdapter
            .Setup(m => m.CreateVariable(methodId, request))
            .Throws(new InvalidAttributeAdapter("Local varible information cant be empty"));

        IActionResult result;
        try
        {
            result = _attributeController?.CreateVariables(request, methodId);
        }
        catch(InvalidAttributeAdapter ex)
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
            Name = parameterName,
            IdReference = Guid.Empty.ToString()
        };
        _mockmethodAdapter
            .Setup(m => m.CreateParameter(methodId, request))
            .Throws(new InvalidAttributeAdapter("Parameter information cant be empty"));

        IActionResult result;
        try
        {
            result = _attributeController.CreateParameter(request, methodId);
        }
        catch(InvalidAttributeAdapter ex)
        {
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

    [TestMethod]
    public void CreateInvocation_WithValidResponse_ShouldReturnCreated()
    {
        var methodId = Guid.NewGuid();
        var invocationRequest = new InvocationRequest { MethodName = "testInvocation", Parameters = [] };
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
    public void CreateInvocation_WithNullResponse_ShouldReturnCreatedWithNullId()
    {
        var methodId = Guid.NewGuid();
        var invocationRequest = new InvocationRequest { MethodName = "testInvocation", Parameters = [] };
        CreatedInvocationResponse? nullResponse = null;

        _mockmethodAdapter?.Setup(m => m.CreateInvocation(methodId, invocationRequest)).Returns((CreatedInvocationResponse?)null);

        var result = _attributeController?.CreateInvocation(invocationRequest, methodId);

        _mockmethodAdapter?.Verify(m => m.CreateInvocation(methodId, invocationRequest), Times.Once);
        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.IsNull(createdResult?.RouteValues["id"]);
        Assert.AreEqual(nullResponse, createdResult?.Value);
    }

    [TestMethod]
    public void CreateInvocation_WithNullInvocationResponse_ShouldReturnCreatedWithNullId()
    {
        var methodId = Guid.NewGuid();
        var invocationRequest = new InvocationRequest { MethodName = "testInvocation", Parameters = [] };
        var expectedResponse = new CreatedInvocationResponse { Message = "Invocation created successfully", InvocationResponse = null };

        _mockmethodAdapter?.Setup(m => m.CreateInvocation(methodId, invocationRequest)).Returns(expectedResponse);

        var result = _attributeController?.CreateInvocation(invocationRequest, methodId);

        _mockmethodAdapter?.Verify(m => m.CreateInvocation(methodId, invocationRequest), Times.Once);
        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.IsNull(createdResult?.RouteValues["id"]);
        Assert.AreEqual(expectedResponse, createdResult?.Value);
    }

    [TestMethod]
    public void CreateParameter_WithNullResponse_ShouldReturnCreatedWithNullId()
    {
        var methodId = Guid.NewGuid();
        var parameterRequest = new ParameterRequest { IdReference = Guid.NewGuid().ToString(), Name = "testParameter" };
        CreatedParameterResponse? nullResponse = null;

        _mockmethodAdapter?.Setup(m => m.CreateParameter(methodId, parameterRequest)).Returns((CreatedParameterResponse?)null);

        var result = _attributeController?.CreateParameter(parameterRequest, methodId);

        _mockmethodAdapter?.Verify(m => m.CreateParameter(methodId, parameterRequest), Times.Once);
        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.IsNull(createdResult?.RouteValues["id"]);
        Assert.AreEqual(nullResponse, createdResult?.Value);
    }

    [TestMethod]
    public void CreateParameter_WithNullParameter_ShouldReturnCreatedWithNullId()
    {
        var methodId = Guid.NewGuid();
        var parameterRequest = new ParameterRequest { IdReference = Guid.NewGuid().ToString(), Name = "testParameter" };
        var expectedResponse = new CreatedParameterResponse { Message = "Parameter created successfully", Parameter = null };

        _mockmethodAdapter?.Setup(m => m.CreateParameter(methodId, parameterRequest)).Returns(expectedResponse);

        var result = _attributeController?.CreateParameter(parameterRequest, methodId);

        _mockmethodAdapter?.Verify(m => m.CreateParameter(methodId, parameterRequest), Times.Once);
        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.IsNull(createdResult?.RouteValues["id"]);
        Assert.AreEqual(expectedResponse, createdResult?.Value);
    }

    [TestMethod]
    public void CreateVariable_WithNullResponse_ShouldReturnCreatedWithNullId()
    {
        var methodId = Guid.NewGuid();
        var variableRequest = new VariablesRequest { Name = "testVariable", IdReference = Guid.NewGuid().ToString() };
        CreatedVariableResponse? nullResponse = null;

        _mockmethodAdapter?.Setup(m => m.CreateVariable(methodId, variableRequest)).Returns((CreatedVariableResponse?)null);

        var result = _attributeController?.CreateVariables(variableRequest, methodId);

        _mockmethodAdapter?.Verify(m => m.CreateVariable(methodId, variableRequest), Times.Once);
        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.IsNull(createdResult?.RouteValues["id"]);
        Assert.AreEqual(nullResponse, createdResult?.Value);
    }

    [TestMethod]
    public void CreateVariable_WithNullVariable_ShouldReturnCreatedWithNullId()
    {
        var methodId = Guid.NewGuid();
        var variableRequest = new VariablesRequest { Name = "testVariable", IdReference = Guid.NewGuid().ToString() };
        var expectedResponse = new CreatedVariableResponse { Message = "Variable created successfully", Variable = null };

        _mockmethodAdapter?.Setup(m => m.CreateVariable(methodId, variableRequest)).Returns(expectedResponse);

        var result = _attributeController?.CreateVariables(variableRequest, methodId);

        _mockmethodAdapter?.Verify(m => m.CreateVariable(methodId, variableRequest), Times.Once);
        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.IsNull(createdResult?.RouteValues["id"]);
        Assert.AreEqual(expectedResponse, createdResult?.Value);
    }
}
