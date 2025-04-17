using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Request;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class ClassMethodControllerTest
{
    private Mock<IMethodAdapter>? _mockmethodAdapter;
    private ClassMethodController? _attributeController;

    [TestInitialize]
    public void Setup()
    {
        _mockmethodAdapter = new Mock<IMethodAdapter>();
        _attributeController = new ClassMethodController(_mockmethodAdapter.Object);
    }

    [TestMethod]
    public void CreateMethodCorrectly_ShouldReturnCreated()
    {
        var id = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var methodRequest = new MethodRequest() { Id = id, Name = "Test Method", Accesibility = SimModelsAccesibility.Normal, Privacity = SimModelsPrivacity.Public, IdClassOwner = classId };
        var expectedResponse = new CreatedMethodResponse() { Id = id, Message = "Method created successfully", MethodResponse = new MethodResponse() { Id = id, Accesibility = SimModelsAccesibility.Normal, IdClassOwner = classId, Privacity = SimModelsPrivacity.Public, Name = "Test Method" } };
        _mockmethodAdapter?.Setup(m => m.CreateMethod(classId, methodRequest)).Returns(expectedResponse);

        var result = _attributeController?.CreateMethod(classId, methodRequest);
        _mockmethodAdapter?.Verify(m => m.CreateMethod(classId, methodRequest), Times.Once);

        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.AreEqual(expectedResponse, createdResult?.Value);
    }
}
