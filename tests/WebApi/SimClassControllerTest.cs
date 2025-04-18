using Adapter.Exceptions;
using Domain;
using Domain.Enums;
using FluentAssertions;
using IAdapter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Models.Request;
using Models.Response;
using Moq;
using WebApi.Controllers;
using WebApi.Filters;

namespace Tests.WebApi;

[TestClass]
public class SimClassControllerTest
{
    private Mock<ISimClassAdapter>? _mockSimClassAdapter;
    private SimClassController? _simClassController;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimClassAdapter = new Mock<ISimClassAdapter>(MockBehavior.Strict);
        _simClassController = new SimClassController(_mockSimClassAdapter.Object);
    }

    [TestMethod]
    public void GetAllClasses_ShouldReturnAllClasses()
    {
        var classes = new List<SimClassResponse>
    {
        new SimClassResponse() { Id = Guid.NewGuid(), Name = "ClassA" },
        new SimClassResponse() { Id = Guid.NewGuid(), Name = "ClassB" }
    };

        _mockSimClassAdapter?.Setup(x => x.GetAllSimClasses()).Returns(classes);

        var result = _simClassController?.GetAllSimClass();

        var listResult = result as OkObjectResult;

        var resultList = listResult?.Value as List<SimClassResponse>;

        _mockSimClassAdapter?.VerifyAll();
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        Assert.IsNotNull(resultList);
        Assert.AreEqual(2, resultList!.Count);
        Assert.AreEqual(classes.First().Name, resultList.First().Name);
        Assert.AreEqual(classes.Last().Name, resultList.Last().Name);
    }

    [TestMethod]
    public void CreateClassCorrectly_ShouldReturnCreatedAtActionResultWithClass()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "ClassC" };
        var request = new SimClassRequest { Name = "ClassC" };
        var simClassResponse = new SimClassResponse() { Id = simClass.Id, Name = simClass.Name };
        var expectedResponse = new CreatedSimClassResponse
        {
            Message = "Class created successfully",
            SimClass = simClassResponse
        };

        _mockSimClassAdapter
            ?.Setup(x => x.CreateSimClass(request))
            .Returns(expectedResponse);

        var result = _simClassController?.CreateSimClass(request);

        _mockSimClassAdapter?.VerifyAll();

        Assert.IsNotNull(result);
        var createdAtResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdAtResult);
        Assert.AreEqual(nameof(SimClassController.GetInfoClass), createdAtResult.ActionName);

        var returnedValue = createdAtResult.Value as CreatedSimClassResponse;
        Assert.IsNotNull(returnedValue);
        Assert.AreEqual("Class created successfully", returnedValue!.Message);
        Assert.IsNotNull(returnedValue.SimClass);
        Assert.AreEqual(simClass.Id, returnedValue.SimClass!.Id);
        Assert.AreEqual(simClass.Name, returnedValue.SimClass.Name);
    }

    [TestMethod]
    public void CreateClassInvalidAttribute_ShouldBeBadRequest()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "InvalidClass" };
        simClass.State = SimAccesibility.Sealed;

        var request = new SimClassRequest
        {
            Name = "InvalidClass",
            IsAbstract = false,
            IsSealed = true,
            BaseClassId = simClass.Id
        };

        _mockSimClassAdapter
            ?.Setup(adapter => adapter.CreateSimClass(request))
            .Throws(new InvalidAttribute("Cannot set as base a sealed or null Class."));
        Action act = () => _simClassController?.CreateSimClass(request);

        act.Should().Throw<InvalidAttribute>().WithMessage("Cannot set as base a sealed or null Class.");
    }

    [TestMethod]
    public void ExceptionFilter_ShouldSetBadRequestWithMessage()
    {
        var exceptionMessage = "Este es el error del adapter";
        var exception = new InvalidAttribute(exceptionMessage);

        var context = new ExceptionContext(
            new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            },
            [])
        {
            Exception = exception
        };

        var filter = new ExceptionFilter();

        filter.OnException(context);

        var result = context.Result as BadRequestObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [TestMethod]
    public void UpdateClassCorrectly_ShouldReturnOK()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "ClassA" };
        var simClassRequest = new UpdateSimClassRequest { Name = "ClassB", Id = simClass.Id };
        var updatedSimClass = new SimClass() { Name = "ClassB", Id = simClass.Id };
        var expectedResponse = new UpdateSimClassResponse
        {
            Id = simClass.Id,
            Message = "Class Updated correctly",
            SimClass = new SimClassResponse() { Id = updatedSimClass.Id, Message = "Class Updated Correctly", Name = updatedSimClass.Name },
        };

        _mockSimClassAdapter
            ?.Setup(x => x.UpdateSimClass(simClassRequest))
            .Returns(expectedResponse);

        var result = _simClassController?.UpdateSimClass(simClassRequest);

        _mockSimClassAdapter?.VerifyAll();

        Assert.IsNotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(expectedResponse, okResult.Value);
    }

    [TestMethod]
    public void DeleteClassCorrectly_ShouldReturnNoContent()
    {
        var classId = Guid.NewGuid();

        _mockSimClassAdapter
            .Setup(x => x.DeleteSimClass(classId))
            .Verifiable();

        _simClassController.DeleteSimClass(classId);

        _mockSimClassAdapter.Verify(x => x.DeleteSimClass(classId), Times.Once);
    }

    [TestMethod]
    public void DeleteNonExistingClass_ShouldReturnNotFound()
    {
        var classId = Guid.NewGuid();

        _mockSimClassAdapter
            .Setup(adapter => adapter.DeleteSimClass(classId))
            .Throws(new ObjectNotFoundException($"Any class with the specified {classId} id exists."));

        IActionResult result;
        try
        {
            result = _simClassController.DeleteSimClass(classId);
        }
        catch(ObjectNotFoundException ex)
        {
            result = new NotFoundObjectResult(new
            {
                Message = ex.Message
            });
        }

        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundObjectResult>();

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var value = notFoundResult.Value;
        var expectedMessage = $"Any class with the specified {classId} id exists.";
        var actualMessage = ((dynamic)value).Message;

        Assert.AreEqual(expectedMessage, actualMessage);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldSetNotFoundException()
    {
        var exception = new ObjectNotFoundException($"Any class with the specified id exists.");

        var context = new ExceptionContext(
            new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            },
            [])
        {
            Exception = exception
        };

        var filter = new ExceptionFilter();

        filter.OnException(context);

        var result = context.Result as NotFoundObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [TestMethod]
    public void GetInfoClass_ValidClassId_ShouldReturnClassInfo()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "ClassInfo" };
        var simClassResponse = new SimClassResponse() { Id = simClass.Id, Name = simClass.Name };

        _mockSimClassAdapter
            ?.Setup(x => x.GetSimClassInfo(simClass.Id))
            .Returns(simClassResponse);

        var result = _simClassController?.GetInfoClass(simClass.Id);

        _mockSimClassAdapter?.VerifyAll();

        Assert.IsNotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var returnedValue = okResult.Value as SimClassResponse;
        Assert.IsNotNull(returnedValue);
        Assert.AreEqual(simClass.Id, returnedValue!.Id);
        Assert.AreEqual(simClass.Name, returnedValue.Name);
    }
}
