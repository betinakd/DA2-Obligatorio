using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Request;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

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
    public void DeleteAttributeControllerWithCorrectId_ShouldReturnNoContent()
    {
        var idToDelete = Guid.NewGuid();
        _mockAttributeAdapter?.Setup(a => a.DeleteAttribute(idToDelete));

        var result = _attributeController?.DeleteAttribute(idToDelete);
        _mockAttributeAdapter?.Verify(a => a.DeleteAttribute(idToDelete), Times.Once);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public void UpdateAttributeControllerWithCorrectBody_ShouldUpdateOk()
    {
        var idToUpdate = Guid.NewGuid();
        var attributeToUpdate = new AttributeRequest()
        {
            Id = idToUpdate,
            Name = "UpdatedAttribute",
            TypeId = Guid.NewGuid(),
            Privacity = SimModelsPrivacity.Public,
            RelatedClassId = Guid.NewGuid()
        };
        var expectedResponse = new AttributeResponse()
        {
            Id = idToUpdate,
            Name = "UpdatedAttribute",
            TypeId = Guid.NewGuid(),
            Privacity = SimModelsPrivacity.Public,
            RelatedClassId = Guid.NewGuid()
        };
        var updatedExpectedResponse = new UpdatedAttributeResponse() { Message = "Attribute updated succesfully.", Attribute = expectedResponse };

        _mockAttributeAdapter?.Setup(a => a.UpdateAttribute(idToUpdate, attributeToUpdate)).Returns(updatedExpectedResponse);

        var result = _attributeController?.UpdateAttribute(idToUpdate, attributeToUpdate);
        _mockAttributeAdapter?.Verify(a => a.UpdateAttribute(idToUpdate, attributeToUpdate), Times.Once);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }
}
