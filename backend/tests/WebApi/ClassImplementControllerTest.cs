using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class ClassImplementControllerTest
{
    private Mock<ISimClassAdapter>? _mockSimClassAdapter;
    private ClassImplementController? _implementController;

    [TestInitialize]
    public void Setup()
    {
        _mockSimClassAdapter = new Mock<ISimClassAdapter>();
        _implementController = new ClassImplementController(_mockSimClassAdapter.Object);
    }

    [TestMethod]
    public void ImplementInterface_ShouldReturnCreatedAtRoute()
    {
        var classId = Guid.NewGuid();
        var interfaceId = Guid.NewGuid();

        var implementRequest = new ImplementRequest
        {
            IdInterface = interfaceId.ToString()
        };

        var mockResponse = new UpdateSimClassResponse
        {
            Message = "Interface implemented successfully",
            SimClass = new SimClassResponse
            {
                Id = classId,
                Name = "TestClass"
            }
        };

        _mockSimClassAdapter!
            .Setup(m => m.ImplementInterface(classId, implementRequest))
            .Returns(mockResponse);

        var result = _implementController!.ImplementInterface(classId, implementRequest);
        _mockSimClassAdapter.Verify(m => m.ImplementInterface(classId, implementRequest), Times.Once);

        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));

        var createdResult = result as CreatedAtRouteResult;

        Assert.AreEqual("GetReferenceClass", createdResult!.RouteName);

        Assert.AreEqual(classId, createdResult.RouteValues!["classId"]);

        Assert.AreEqual(mockResponse, createdResult.Value);
    }
}