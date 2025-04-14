using Domain;
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
            Type = new SimClassResponse(new SimClass { Name = "DummyType" }),
            Privacity = SimPrivacity.Public,
            RelatedClass = new SimClassResponse(new SimClass { Name = "DummyRelatedClass" })
        };
        var expectedResponse = new AttributeResponse()
        {
            Id = id,
            Name = "DummyAttribute",
            Type = new SimClassResponse(new SimClass { Name = "DummyType" }),
            Privacity = SimPrivacity.Public,
            RelatedClass = new SimClassResponse(new SimClass { Name = "DummyRelatedClass" })
        };
        var expectedCreatedResponse = new CreatedAttributeResponse() { Message = "Attribute was created successfully", Privacity = expectedResponse };
        _mockAttributeAdapter?.Setup(a => a.CreateAttribute(id, request)).Returns(expectedCreatedResponse);

        var result = _attributeController?.CreateAttribute(id, request);
        _mockAttributeAdapter?.Verify(a => a.CreateAttribute(id, request), Times.Once);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }
}
