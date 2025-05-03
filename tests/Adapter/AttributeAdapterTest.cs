using Adapter;
using Adapter.Exceptions;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
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
    public void CreateAttribute_WithValidData_CreatesAndReturnsResponse()
    {
        var attributeId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdClassType = typeId.ToString()
        };

        var relatedClass = new SimClass { Id = relatedClassId, Name = "RelatedClass" };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };

        var createdSimAttribute = new SimAttribute()
        {
            Id = attributeId,
            Name = attributeRequest.Name,
            Privacity = SimPrivacity.Public,
            RelatedClass = relatedClass,
            RelatedClassId = relatedClassId,
            Type = typeClass,
            TypeId = typeId
        };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(relatedClassId))
            .Returns(relatedClass);
        _mockSimClassService!
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(typeClass);

        _mockSimAttributeService!
            .Setup(s => s.CreateAttribute(relatedClassId, It.IsAny<SimAttribute>()))
            .Returns(createdSimAttribute);

        var result = _simAttributeAdapter!.CreateAttribute(relatedClassId, attributeRequest);

        _mockSimClassService.Verify(s => s.GetSimClassById(relatedClassId), Times.Once);
        _mockSimClassService.Verify(s => s.GetSimClassById(typeId), Times.Once);
        _mockSimAttributeService.Verify(s => s.CreateAttribute(relatedClassId, It.IsAny<SimAttribute>()), Times.Once);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Attribute);
        Assert.AreEqual(attributeId, result.Attribute.Id);
        Assert.AreEqual("TestAttribute", result.Attribute.Name);
        Assert.AreEqual(Models.Enums.SimModelsPrivacity.Public, result.Attribute.Privacity);
        Assert.AreEqual(relatedClassId, result.Attribute.RelatedClassId);
        Assert.AreEqual(typeId, result.Attribute.TypeId);
        Assert.AreEqual("Attribute created successfully.", result.Message);
    }

    [TestMethod]
    public void DeleteAttribute_WhenInUseValueLogic_ThrowsInUseValueAdapter()
    {
        var attributeId = Guid.NewGuid();
        _mockSimAttributeService!
            .Setup(s => s.DeleteAttribute(attributeId))
            .Throws(new InUseValueLogic("Attribute in use"));

        var ex = Assert.ThrowsException<InUseValueAdapter>(() =>
            _simAttributeAdapter!.DeleteAttribute(attributeId));

        Assert.AreEqual("Attribute in use", ex.Message);
    }

    [TestMethod]
    public void DeleteAttribute_WhenNonExistentValueLogic_ThrowsNonExistentValueAdapter()
    {
        var attributeId = Guid.NewGuid();
        _mockSimAttributeService!
            .Setup(s => s.DeleteAttribute(attributeId))
            .Throws(new NonExistentValueLogic("Attribute not found"));

        var ex = Assert.ThrowsException<NonExistentValueAdapter>(() =>
            _simAttributeAdapter!.DeleteAttribute(attributeId));

        Assert.AreEqual("Attribute not found", ex.Message);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueAdapter))]
    public void CreateAttribute_ThrowsInUseValueAdapter_WhenServiceThrowsInUseValueLogic()
    {
        var classId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdClassType = typeId.ToString()
        };

        var relatedClass = new SimClass { Id = classId };
        var typeClass = new SimClass { Id = typeId };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(classId))
            .Returns(relatedClass);
        _mockSimClassService
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(typeClass);
        _mockSimAttributeService!
            .Setup(s => s.CreateAttribute(classId, It.IsAny<SimAttribute>()))
            .Throws(new InUseValueLogic("Exception message"));

        _simAttributeAdapter!.CreateAttribute(classId, attributeRequest);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void CreateAttribute_ThrowsNonExistentValueAdapter_WhenServiceThrowsNonExistentValueLogic()
    {
        var classId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdClassType = typeId.ToString()
        };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(classId))
            .Throws(new NonExistentValueLogic("Exception message"));

        _simAttributeAdapter!.CreateAttribute(classId, attributeRequest);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateAttribute_ThrowsInvalidAttributeAdapter_WhenServiceThrowsInvalidAttributeDomain()
    {
        var classId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdClassType = typeId.ToString(),
        };

        var relatedClass = new SimClass { Id = classId };
        var typeClass = new SimClass { Id = typeId };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(classId))
            .Returns(relatedClass);
        _mockSimClassService
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(typeClass);
        _mockSimAttributeService!
            .Setup(s => s.CreateAttribute(classId, It.IsAny<SimAttribute>()))
            .Throws(new InvalidAttributeDomain("Exception message"));

        _simAttributeAdapter!.CreateAttribute(classId, attributeRequest);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void UpdateAttribute_ThrowsNonExistentValueAdapter_WhenServiceThrowsNonExistentValueLogic()
    {
        var attributeId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var attributeRequest = new AttributeRequestUpdate
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdRelatedClass = relatedClassId.ToString(),
            IdType = typeId.ToString()
        };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(relatedClassId))
            .Throws(new NonExistentValueLogic("Class not found"));

        _simAttributeAdapter!.UpdateAttribute(attributeRequest);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void UpdateAttribute_ThrowsInvalidAttributeAdapter_WhenServiceThrowsInvalidAttributeDomain()
    {
        var attributeId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var attributeRequest = new AttributeRequestUpdate
        {
            Id = attributeId.ToString(),
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdRelatedClass = classId.ToString(),
            IdType = typeId.ToString()
        };

        var relatedClass = new SimClass { Id = classId, Name = "TestClass" };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };

        _mockSimClassService!.Setup(s => s.GetSimClassById(classId)).Returns(relatedClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(typeId)).Returns(typeClass);

        _mockSimAttributeService!.Setup(s => s.UpdateAttribute(attributeId, It.IsAny<SimAttribute>()))
            .Throws(new InvalidAttributeDomain("Test domain validation error"));

        _simAttributeAdapter!.UpdateAttribute(attributeRequest);
    }

    [TestMethod]
    public void GetAttribute_ReturnsCorrectResponse_WhenAttributeExists()
    {
        var id = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();

        var simAttribute = new SimAttribute
        {
            Id = id,
            Name = "TestAttribute",
            Privacity = SimPrivacity.Public,
            Type = new SimClass { Id = typeId, Name = "string" },
            TypeId = typeId,
            RelatedClass = new SimClass { Id = relatedClassId, Name = "Owner" },
            RelatedClassId = relatedClassId
        };

        _mockSimAttributeService!
            .Setup(s => s.GetSimAttribute(id))
            .Returns(simAttribute);

        var result = _simAttributeAdapter!.GetAttribute(id);

        Assert.IsNotNull(result);
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("TestAttribute", result.Name);
        Assert.AreEqual(Models.Enums.SimModelsPrivacity.Public, result.Privacity);
        Assert.AreEqual(relatedClassId, result.RelatedClassId);
        Assert.AreEqual(typeId, result.TypeId);

        _mockSimAttributeService.Verify(s => s.GetSimAttribute(id), Times.Once);
    }

    [TestMethod]
    public void GetAttribute_ThrowsNonExistentValueAdapter_WhenAttributeDoesNotExist()
    {
        var id = Guid.NewGuid();
        _mockSimAttributeService!
            .Setup(s => s.GetSimAttribute(id))
            .Throws(new NonExistentValueLogic("Attribute not found"));

        var ex = Assert.ThrowsException<NonExistentValueAdapter>(() => _simAttributeAdapter!.GetAttribute(id));
        Assert.AreEqual("Attribute not found", ex.Message);
    }
}
