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
        var attributeToUpdate = new AttributeRequestUpdate()
        {
            Id = idToUpdate.ToString(),
            Name = "UpdatedAttribute",
            IdReference = Guid.NewGuid().ToString(),
            Privacity = SimModelsPrivacity.Public,
            IdRelatedClass = Guid.NewGuid().ToString()
        };
        var expectedResponse = new AttributeResponse()
        {
            Id = idToUpdate,
            Name = "UpdatedAttribute",
            ReferenceId = Guid.NewGuid(),
            Privacity = SimModelsPrivacity.Public,
            RelatedClassId = Guid.NewGuid()
        };
        var updatedExpectedResponse = new UpdatedAttributeResponse() { Message = "Attribute updated succesfully.", Attribute = expectedResponse };

        _mockAttributeAdapter?.Setup(a => a.UpdateAttribute(attributeToUpdate)).Returns(updatedExpectedResponse);

        var result = _attributeController?.UpdateAttribute(attributeToUpdate);
        _mockAttributeAdapter?.Verify(a => a.UpdateAttribute(attributeToUpdate), Times.Once);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }

    [TestMethod]
    public void GetAttributeControllerWithValidId_ShouldReturnOkResultWithAttribute()
    {
        // Arrange
        var idToGet = Guid.NewGuid();
        var expectedAttribute = new AttributeResponse()
        {
            Id = idToGet,
            Name = "TestAttribute",
            ReferenceId = Guid.NewGuid(),
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            RelatedClassId = Guid.NewGuid()
        };

        _mockAttributeAdapter?.Setup(a => a.GetAttribute(idToGet)).Returns(expectedAttribute);

        // Act
        var result = _attributeController?.GetAttribute(idToGet);

        // Assert
        _mockAttributeAdapter?.Verify(a => a.GetAttribute(idToGet), Times.Once);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(expectedAttribute, okResult.Value);
    }

    [TestMethod]
    public void GetAttributeControllerWithNonExistingId_ShouldReturnOkResultWithNull()
    {
        var idToGet = Guid.NewGuid();
        _mockAttributeAdapter?.Setup(a => a.GetAttribute(idToGet)).Returns((AttributeResponse?)null);

        var result = _attributeController?.GetAttribute(idToGet);

        _mockAttributeAdapter?.Verify(a => a.GetAttribute(idToGet), Times.Once);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsNull(okResult.Value);
    }

    [TestMethod]
    public void GetAttributeController_ReturnsOkObjectResult_WhenAttributeFound()
    {
        var id = Guid.NewGuid();
        var expectedAttribute = new AttributeResponse
        {
            Id = id,
            Name = "Test Attribute",
            ReferenceId = Guid.NewGuid(),
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            RelatedClassId = Guid.NewGuid()
        };

        _mockAttributeAdapter?.Setup(a => a.GetAttribute(id)).Returns(expectedAttribute);

        var result = _attributeController?.GetAttribute(id);

        _mockAttributeAdapter?.Verify(a => a.GetAttribute(id), Times.Once);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(expectedAttribute, okResult.Value);
    }

    [TestMethod]
    public void GetAttributeController_ReturnsOkObjectResult_WithNull_WhenAttributeNotFound()
    {
        var id = Guid.NewGuid();
        _mockAttributeAdapter?.Setup(a => a.GetAttribute(id)).Returns((AttributeResponse?)null);

        var result = _attributeController?.GetAttribute(id);

        _mockAttributeAdapter?.Verify(a => a.GetAttribute(id), Times.Once);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsNull(okResult.Value);
    }
}
