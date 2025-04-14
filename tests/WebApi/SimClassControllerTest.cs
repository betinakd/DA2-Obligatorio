using Adapter.Exceptions;
using Domain;
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
        new SimClassResponse(new SimClass { Id = Guid.NewGuid(), Name = "ClassA" }),
        new SimClassResponse(new SimClass { Id = Guid.NewGuid(), Name = "ClassB" })
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
    public void CreateClassCorrectly_ShouldReturnCreatedResultWithClass()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "ClassC" };
        var request = new SimClassRequest { Name = "ClassC" };
        var expectedResponse = new SimClassResponse(simClass);

        _mockSimClassAdapter?.Setup(x => x.CreateSimClass(request)).Returns(expectedResponse);

        var result = _simClassController?.CreateSimClass(request);

        var createdResult = result as CreatedResult;
        var resultClass = createdResult?.Value as SimClassResponse;

        _mockSimClassAdapter?.Verify();
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        Assert.IsNotNull(resultClass);
        Assert.AreEqual(expectedResponse.Name, resultClass!.Name);
        Assert.AreEqual(expectedResponse.Id, resultClass.Id);
    }

    [TestMethod]
    public void DeleteClassCorrectly_ShouldReturnNoContent()
    {
        var classId = Guid.NewGuid();
        var classes = new List<SimClassResponse>
    {
        new SimClassResponse(new SimClass { Id = classId, Name = "ClassA" }),
        new SimClassResponse(new SimClass { Id = Guid.NewGuid(), Name = "ClassB" })
    };

        _mockSimClassAdapter?.Setup(x => x.DeleteSimClass(classId)).Verifiable();
        var result = _simClassController?.DeleteSimClass(classId);
        classes.RemoveAll(c => c.Id == classId);

        _mockSimClassAdapter?.Verify(x => x.DeleteSimClass(classId), Times.Once);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        Assert.IsFalse(classes.Any(c => c.Id == classId));
    }

    [TestMethod]
    public void DeleteNonExistingClass_ShouldReturnNotFound()
    {
        var classId = Guid.NewGuid();

        _mockSimClassAdapter
            .Setup(act => act.DeleteSimClass(classId))
            .Throws(new ObjectNotFoundException($"Any class with the specified {classId} id exists."));

        Action act = () => _simClassController.DeleteSimClass(classId);
        act.Should().Throw<ObjectNotFoundException>();
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
}
