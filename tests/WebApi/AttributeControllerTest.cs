using Domain;
using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Response;
using Moq;
using WebApi.Controllers;

[TestClass]
public class AttributeControllerTest
{
    private Mock<IAttributeAdapter>? _mockAttributeAdapter;
    private AttributeController? _attributeController;

    [TestInitialize]
    public void Setup()
    {
        _mockAttributeAdapter = new Mock<IAttributeAdapter>();
        _attributeController = new AttributeController(_mockAttributeAdapter.Object);
    }

    [TestMethod]
    public void DeletAttributeControllerWithCorrectId_ShouldDeleteOk()
    {
        var idToDelete = Guid.NewGuid();
        var expectedResponse = new AttributeResponse()
        {
            Id = idToDelete,
            Name = "DummyAttribute",
            Type = new SimClassResponse(new SimClass { Name = "DummyType" }),
            Accesibility = SimAccesibility.Public,
            RelatedClass = new SimClassResponse(new SimClass { Name = "DummyRelatedClass" })
        };
        _mockAttributeAdapter?.Setup(a => a.DeleteAttribute(idToDelete)).Returns(expectedResponse);

        var result = _attributeController?.DeleteAttribute(idToDelete);
        _mockAttributeAdapter?.Verify(a => a.DeleteAttribute(idToDelete), Times.Once);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }
}