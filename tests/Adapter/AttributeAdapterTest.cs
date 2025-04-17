using Adapter;
using Adapter.Exceptions;
using Domain;
using Domain.Enums;
using IBussinesLogic;
using Models.Request;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class AttributeAdapterTest
{
    private Mock<ISimAttributeService>? _mockSimAttributeService;
    private AttributeAdapter? _simAttributeAdapter;
    private Mock<ISimClassService>? _mockSimClassService;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimAttributeService = new Mock<ISimAttributeService>(MockBehavior.Strict);
        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _simAttributeAdapter = new AttributeAdapter(_mockSimAttributeService.Object, _mockSimClassService.Object);
    }

    [TestMethod]
    public void DeleteAttribute_CallsServiceDeleteAttribute_WithCorrectId()
    {
        var attributeId = Guid.NewGuid();
        _mockSimAttributeService!.Setup(s => s.DeleteAttribute(attributeId)).Verifiable();

        _simAttributeAdapter!.DeleteAttribute(attributeId);

        _mockSimAttributeService.Verify(s => s.DeleteAttribute(attributeId), Times.Once);
    }

    [TestMethod]
    public void DeleteAttribute_WhenServiceThrowsException_ThrowsInvalidOperationException()
    {
        var attributeId = Guid.NewGuid();
        var exceptionMessage = "Error al eliminar";

        _mockSimAttributeService!
            .Setup(s => s.DeleteAttribute(attributeId))
            .Throws(new Exception(exceptionMessage));

        var ex = Assert.ThrowsException<InvalidOperationException>(() =>
            _simAttributeAdapter!.DeleteAttribute(attributeId));

        Assert.AreEqual(exceptionMessage, ex.Message);
    }

    [TestMethod]
    public void UpdateAttribute_WithValidData_UpdatesAndReturnsResponse()
    {
        var attributeId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            RelatedClassId = relatedClassId,
            TypeId = typeId,
            Id = attributeId
        };

        var relatedClass = new SimClass { Id = relatedClassId, Name = "RelatedClass" };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };

        var attributeSimClass = new SimAttribute()
        {
            Id = attributeId,
            Name = attributeRequest.Name,
            Privacity = SimPrivacity.Public,
            RelatedClass = relatedClass,
            Type = typeClass
        };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(relatedClassId))
            .Returns(relatedClass);
        _mockSimClassService!
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(typeClass);

        _mockSimAttributeService!
            .Setup(s => s.UpdateAttribute(attributeId, It.IsAny<SimAttribute>()))
            .Returns(attributeSimClass);

        var result = _simAttributeAdapter!.UpdateAttribute(attributeId, attributeRequest);

        _mockSimClassService.Verify(s => s.GetSimClassById(relatedClassId), Times.Once);
        _mockSimClassService.Verify(s => s.GetSimClassById(typeId), Times.Once);
        _mockSimAttributeService.Verify(s => s.UpdateAttribute(attributeId, It.IsAny<SimAttribute>()), Times.Once);

        Assert.IsNotNull(result);
        Assert.AreEqual(attributeId, result.Attribute.Id);
        Assert.AreEqual(attributeRequest.Name, result.Attribute.Name);
        Assert.AreEqual(attributeRequest.Privacity, result.Attribute.Privacity);
        Assert.AreEqual(attributeRequest.RelatedClassId, result.Attribute.RelatedClassId);
        Assert.AreEqual(attributeRequest.TypeId, result.Attribute.TypeId);
        Assert.AreEqual("Attribute updated successfully.", result.Message);
    }

    [TestMethod]
    public void UpdateAttribute_WhenServiceThrowsException_ThrowsObjectNotFoundException()
    {
        var attributeId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            RelatedClassId = relatedClassId,
            TypeId = typeId
        };

        var exceptionMessage = "Related class not found";

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(relatedClassId))
            .Throws(new Exception(exceptionMessage));

        var ex = Assert.ThrowsException<ObjectNotFoundException>(() =>
            _simAttributeAdapter!.UpdateAttribute(attributeId, attributeRequest));

        Assert.AreEqual(exceptionMessage, ex.Message);
    }

    [TestMethod]
    public void CreateAttribute_WithValidData_CreatesAndReturnsResponse()
    {
        var attributeId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            RelatedClassId = relatedClassId,
            TypeId = typeId,
            Id = attributeId
        };

        var relatedClass = new SimClass { Id = relatedClassId, Name = "RelatedClass" };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };

        var createdSimAttribute = new SimAttribute()
        {
            Id = attributeId,
            Name = attributeRequest.Name,
            Privacity = SimPrivacity.Public,
            RelatedClass = relatedClass,
            Type = typeClass
        };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(relatedClassId))
            .Returns(relatedClass);
        _mockSimClassService!
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(typeClass);

        _mockSimAttributeService!
            .Setup(s => s.CreateAttribute(attributeId, It.IsAny<SimAttribute>()))
            .Returns(createdSimAttribute);

        var result = _simAttributeAdapter!.CreateAttribute(attributeId, attributeRequest);

        _mockSimClassService.Verify(s => s.GetSimClassById(relatedClassId), Times.Once);
        _mockSimClassService.Verify(s => s.GetSimClassById(typeId), Times.Once);
        _mockSimAttributeService.Verify(s => s.CreateAttribute(attributeId, It.IsAny<SimAttribute>()), Times.Once);

        Assert.IsNotNull(result);
        Assert.AreEqual(attributeId, result.Attribute.Id);
        Assert.AreEqual(attributeRequest.Name, result.Attribute.Name);
        Assert.AreEqual(attributeRequest.Privacity, result.Attribute.Privacity);
        Assert.AreEqual(attributeRequest.RelatedClassId, result.Attribute.RelatedClassId);
        Assert.AreEqual(attributeRequest.TypeId, result.Attribute.TypeId);
        Assert.AreEqual("Attribute created successfully.", result.Message);
    }

    [TestMethod]
    public void CreateAttribute_WhenServiceThrowsException_ThrowsObjectNotFoundException()
    {
        var attributeId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            RelatedClassId = relatedClassId,
            TypeId = typeId,
            Id = attributeId
        };

        var exceptionMessage = "Related class not found";

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(relatedClassId))
            .Throws(new Exception(exceptionMessage));

        var ex = Assert.ThrowsException<ObjectNotFoundException>(() =>
            _simAttributeAdapter!.CreateAttribute(attributeId, attributeRequest));

        Assert.AreEqual(exceptionMessage, ex.Message);
    }
}
