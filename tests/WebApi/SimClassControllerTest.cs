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
    public void CreateClassCorrectly_ShouldReturnCreatedAtActionResultWithClass()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "ClassC" };
        var request = new SimClassRequest { Name = "ClassC" };
        var simClassResponse = new SimClassResponse(simClass);
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
        Assert.AreEqual(nameof(SimClassController.CreateSimClass), createdAtResult.ActionName);

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
        simClass.SetState(new StateSealed());

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
}
