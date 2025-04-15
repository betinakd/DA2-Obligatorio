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
    public void AddMethodParameterCorrectly_ShouldReturnOK()
    {
        var methodId = Guid.NewGuid();
        var parameterName = "param1";
        var parameterType = "string";

        var request = new MethodElementsRequest
        {
            MethodId = methodId,
            Name = parameterName,
            Type = parameterType
        };

        _mockmethodAdapter
            .Setup(m => m.AddParameter(methodId, request));

        var result = _attributeController?.AddMethodParameter(methodId, request);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        _mockmethodAdapter.Verify(m => m.AddParameter(methodId, request), Times.Once);
    }

    [TestMethod]
    public void AddParameterWithNullAttributes_ShouldReturnBadRequest()
    {
        var methodId = Guid.NewGuid();
        var parameterName = " ";
        var parameterType = " ";

        var request = new MethodElementsRequest
        {
            MethodId = methodId,
            Name = parameterName,
            Type = parameterType
        };
        _mockmethodAdapter
            .Setup(m => m.AddParameter(methodId, request))
            .Throws(new InvalidAttribute("Parameter information cant be empty"));

        IActionResult result;
        try
        {
            result = _attributeController.AddMethodParameter(methodId, request);
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

    [TestMethod]
    public void AddMethodLocalVariablesCorrectly_ShouldReturnOK()
    {
        var methodId = Guid.NewGuid();
        var variableName = "var";
        var variableType = "string";

        var request = new MethodElementsRequest
        {
            MethodId = methodId,
            Name = variableName,
            Type = variableType
        };

        _mockmethodAdapter
            .Setup(m => m.AddLocalVariable(methodId, request));

        var result = _attributeController?.AddMethodLocalVariable(methodId, request);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        _mockmethodAdapter.Verify(m => m.AddLocalVariable(methodId, request), Times.Once);
    }

    [TestMethod]
    public void AddLocalVariableWithNullAttributes_ShouldReturnBadRequest()
    {
        var methodId = Guid.NewGuid();
        var parameterName = " ";
        var parameterType = " ";

        var request = new MethodElementsRequest
        {
            MethodId = methodId,
            Name = parameterName,
            Type = parameterType
        };
        _mockmethodAdapter
            .Setup(m => m.AddLocalVariable(methodId, request))
            .Throws(new InvalidAttribute("Local varible information cant be empty"));

        IActionResult result;
        try
        {
            result = _attributeController.AddMethodLocalVariable(methodId, request);
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
}
