using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Request;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class ClassAttributeControllerTest
{
    private Mock<IAttributeAdapter>? _mockAttributeAdapter;
    private ClassAttributeController? _attributeController;

    [TestInitialize]
    public void Setup()
    {
        _mockAttributeAdapter = new Mock<IAttributeAdapter>();
        _attributeController = new ClassAttributeController(_mockAttributeAdapter.Object);
    }

    [TestMethod]
    public void CreatedClassAttributeControllerWithCorrectId_ShouldThrowOk()
    {
        var id = Guid.NewGuid();
        var request = new AttributeRequest()
        {
            Id = id,
            Name = "DummyAttribute",
            TypeId = Guid.NewGuid(),
            Privacity = SimModelsPrivacity.Public,
            RelatedClassId = Guid.NewGuid()
        };
        var expectedResponse = new AttributeResponse()
        {
            Id = id,
            Name = "DummyAttribute",
            TypeId = Guid.NewGuid(),
            Privacity = SimModelsPrivacity.Public,
            RelatedClassId = Guid.NewGuid()
        };
        var expectedCreatedResponse = new CreatedAttributeResponse() { Message = "Attribute was created successfully", Attribute = expectedResponse };
        _mockAttributeAdapter?.Setup(a => a.CreateAttribute(id, request)).Returns(expectedCreatedResponse);

        var result = _attributeController?.CreateAttribute(id, request);
        _mockAttributeAdapter?.Verify(a => a.CreateAttribute(id, request), Times.Once);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }
}
