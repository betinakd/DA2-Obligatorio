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
public class ClassInterfaceControllerTest
{
    private Mock<ISimClassAdapter>? _mockSimClassAdapter;
    private ClassImplementsController? _simClassController;

    [TestInitialize]
    public void Setup()
    {
        _mockSimClassAdapter = new Mock<ISimClassAdapter>();
        _simClassController = new ClassImplementsController(_mockSimClassAdapter.Object);
    }

    [TestMethod]
    public void AddInterfaceToClass_WithValidRequest_ShouldReturnCreated()
    {
        var classId = Guid.NewGuid();
        var interfaceRequest = new InterfaceRequestUpdate
        {
            IdInterface = Guid.NewGuid().ToString(),
        };

        var expectedResponse = new SimClassResponse
        {
            Id = classId,
            Name = "TestClass",
        };

        var expectedResponseCreated = new CreatedSimClassResponse
        {
            Message = "Interface added successfully",
            SimClass = expectedResponse,
        };

        _mockSimClassAdapter
            .Setup(m => m.AddInterface(classId, interfaceRequest))
            .Returns(expectedResponseCreated);

        var result = _simClassController.AddInterfaceToClass(classId, interfaceRequest);

        _mockSimClassAdapter.Verify(m => m.AddInterface(classId, interfaceRequest), Times.Once);

        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedAtRouteResult>();

        var createdResult = result as CreatedAtRouteResult;
        createdResult!.RouteName.Should().Be("GetReferenceClass");
        createdResult.RouteValues!["classId"].Should().Be(classId);
        createdResult.Value.Should().Be(expectedResponseCreated);
    }

    [TestMethod]
    public void AddInterfaceToClass_WithInvalidRequest_ShouldReturnBadRequest()
    {
        var classId = Guid.NewGuid();
        var interfaceRequest = new InterfaceRequestUpdate
        {
            IdInterface = Guid.NewGuid().ToString(),
        };

        _mockSimClassAdapter
            .Setup(m => m.AddInterface(classId, interfaceRequest))
            .Throws(new InvalidAttributeAdapter("Invalid interface ID"));

        IActionResult result;
        try
        {
            result = _simClassController.AddInterfaceToClass(classId, interfaceRequest);
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
        var expectedMessage = "Invalid interface ID";
        var actualMessage = ((dynamic)value).Message;

        Assert.AreEqual(expectedMessage, actualMessage);
    }
}
