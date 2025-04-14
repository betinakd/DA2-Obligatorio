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
            Privacity = SimPrivacity.Public,
            RelatedClass = new SimClassResponse(new SimClass { Name = "DummyRelatedClass" })
        };
        var expectedDeletedResponse = new DeletedAttributeResponse() { Message = "Attribute was deleted succesfully", Attribute = expectedResponse };
        _mockAttributeAdapter?.Setup(a => a.DeleteAttribute(idToDelete)).Returns(expectedDeletedResponse);

        var result = _attributeController?.DeleteAttribute(idToDelete);
        _mockAttributeAdapter?.Verify(a => a.DeleteAttribute(idToDelete), Times.Once);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }

    [TestMethod]
    public void UpdateAttributeControllerWithCorrectBody_ShouldUpdateOk()
    {
        var idToUpdate = Guid.NewGuid();
        var attributeToUpdate = new AttributeRequest()
        {
            Id = idToUpdate,
            Name = "UpdatedAttribute",
            Type = new SimClassResponse(new SimClass { Name = "UpdatedType" }),
            Privacity = SimPrivacity.Public,
            RelatedClass = new SimClassResponse(new SimClass { Name = "UpdatedRelatedClass" })
        };
        var expectedResponse = new AttributeResponse()
        {
            Id = idToUpdate,
            Name = "UpdatedAttribute",
            Type = new SimClassResponse(new SimClass { Name = "UpdatedType" }),
            Privacity = SimPrivacity.Public,
            RelatedClass = new SimClassResponse(new SimClass { Name = "UpdatedRelatedClass" })
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
