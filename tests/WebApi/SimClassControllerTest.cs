using Adapter.Exceptions;
using Domain;
using Domain.Enums;
using FluentAssertions;
using IAdapter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Request;
using Models.Response;
using Moq;
using WebApi.Controllers;

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
        new SimClassResponse() { Id = Guid.NewGuid(), Name = "ClassB" },
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
        var request = new SimClassRequestUpdate { Name = "ClassC" };
        var simClassResponse = new SimClassResponse() { Id = simClass.Id, Name = simClass.Name };
        var expectedResponse = new CreatedSimClassResponse
        {
            Message = "Class created successfully",
            SimClass = simClassResponse,
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

        var request = new SimClassRequestUpdate
        {
            Name = "InvalidClass",
            State = (Models.Enums.SimModelsAccesibility?)SimAccesibility.Sealed,
            IdBaseClass = simClass.Id.ToString(),
        };

        _mockSimClassAdapter
            ?.Setup(adapter => adapter.CreateSimClass(request))
            .Throws(new InvalidAttributeAdapter("Cannot set as base a sealed or null Class."));
        Action act = () => _simClassController?.CreateSimClass(request);

        act.Should().Throw<InvalidAttributeAdapter>().WithMessage("Cannot set as base a sealed or null Class.");
    }

    [TestMethod]
    public void UpdateSimClass_WithValidRequest_ShouldReturnOkWithUpdatedClass()
    {
        var classId = Guid.NewGuid();

        var updateRequest = new SimClassRequestCreate
        {
            Id = classId,
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = Guid.NewGuid().ToString(),
            Methods = [],
            Attributes = []
        };

        var expectedResponse = new UpdateSimClassResponse
        {
            Id = classId,
            Message = "Class updated successfully",
            SimClass = new SimClassResponse()
            {
                Id = classId,
                Name = "UpdatedClass",
                State = SimModelsAccesibility.Normal
            }
        };

        _mockSimClassAdapter!
            .Setup(adapter => adapter.UpdateSimClass(updateRequest, classId))
            .Returns(expectedResponse);

        var result = _simClassController!.UpdateSimClass(updateRequest);

        _mockSimClassAdapter.Verify(adapter => adapter.UpdateSimClass(updateRequest, classId), Times.Once);

        Assert.IsNotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult!.StatusCode);

        var responseValue = okResult.Value as UpdateSimClassResponse;
        Assert.IsNotNull(responseValue);
        Assert.AreEqual(expectedResponse.Id, responseValue!.Id);
        Assert.AreEqual(expectedResponse.Message, responseValue.Message);
        Assert.AreEqual(expectedResponse.SimClass.Id, responseValue.SimClass!.Id);
        Assert.AreEqual(expectedResponse.SimClass.Name, responseValue.SimClass.Name);
        Assert.AreEqual(expectedResponse.SimClass.State, responseValue.SimClass.State);
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
            .Throws(new NonExistentValueAdapter($"Any class with the specified {classId} id exists."));

        IActionResult result;
        try
        {
            result = _simClassController.DeleteSimClass(classId);
        }
        catch(NonExistentValueAdapter ex)
        {
            result = new NotFoundObjectResult(new
            {
                Message = ex.Message,
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
