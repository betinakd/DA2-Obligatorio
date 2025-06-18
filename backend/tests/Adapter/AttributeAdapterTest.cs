using Adapter;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
using IAdapter.Exceptions;
using IBusinessLogic;
using IBusinessLogic.Exceptions;
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
        var instanceId = Guid.NewGuid();

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdReference = typeId.ToString(),
            IdInstance = instanceId.ToString(),
            IsStatic = false,
        };

        var relatedClass = new SimClass { Id = relatedClassId, Name = "RelatedClass" };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        var instanceClass = new SimClass { Id = instanceId, Name = "InstanceClass" };

        var createdSimAttribute = new SimAttribute()
        {
            Id = attributeId,
            Name = attributeRequest.Name,
            Privacity = SimPrivacity.Public,
            RelatedClass = relatedClass,
            RelatedClassId = relatedClassId,
            Reference = typeClass,
            ReferenceId = typeId,
            Instance = instanceClass,
            InstanceId = instanceId,
        };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(relatedClassId))
            .Returns(relatedClass);
        _mockSimClassService!
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(typeClass);
        _mockSimClassService!
            .Setup(s => s.GetSimClassById(instanceId))
            .Returns(instanceClass);
        _mockSimAttributeService!
            .Setup(s => s.CreateAttribute(relatedClassId, It.IsAny<SimAttribute>()))
            .Returns(createdSimAttribute);
        _mockSimClassService!
            .Setup(s => s.ValidPolymorphism(It.IsAny<SimClass>(), It.IsAny<SimClass>()));

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
        Assert.AreEqual(typeId, result.Attribute.ReferenceId);
        Assert.AreEqual("Attribute created successfully.", result.Message);
        Assert.AreEqual(instanceId, result.Attribute.InstanceId);
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
    public void CreateAttribute_ThrowsInUseValueAdapter_WhenServiceThrowsInUseValueLogic()
    {
        var classId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();

        var instanceClass = new SimClass { Id = instanceId, Name = "InstanceClass" };

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdReference = typeId.ToString(),
            IdInstance = instanceId.ToString(),
        };

        var relatedClass = new SimClass { Id = classId };
        var typeClass = new SimClass { Id = typeId };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(instanceId))
            .Returns(instanceClass);
        _mockSimClassService!
            .Setup(s => s.GetSimClassById(classId))
            .Returns(relatedClass);
        _mockSimClassService!
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(typeClass);
        _mockSimAttributeService!
            .Setup(s => s.CreateAttribute(classId, It.IsAny<SimAttribute>()))
            .Throws(new InUseValueLogic("Exception message"));
        _mockSimClassService!
            .Setup(s => s.ValidPolymorphism(It.IsAny<SimClass>(), It.IsAny<SimClass>()));

        var ex = Assert.ThrowsException<InUseValueAdapter>(() =>
            _simAttributeAdapter!.CreateAttribute(classId, attributeRequest));

        Assert.AreEqual("Exception message", ex.Message);
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
            IdReference = typeId.ToString(),
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
        var instanceId = Guid.NewGuid();

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdReference = typeId.ToString(),
            IdInstance = instanceId.ToString(),
        };

        var relatedClass = new SimClass { Id = classId };
        var typeClass = new SimClass { Id = typeId };
        var instanceClass = new SimClass { Id = instanceId };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(classId))
            .Returns(relatedClass);
        _mockSimClassService!
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(typeClass);
        _mockSimClassService!
            .Setup(s => s.GetSimClassById(instanceId))
            .Returns(instanceClass);
        _mockSimAttributeService!
            .Setup(s => s.CreateAttribute(classId, It.IsAny<SimAttribute>()))
            .Throws(new InvalidAttributeDomain("Exception message"));
        _mockSimClassService!
            .Setup(s => s.ValidPolymorphism(It.IsAny<SimClass>(), It.IsAny<SimClass>()));

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
            IdReference = typeId.ToString(),
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
        var instanceId = Guid.NewGuid();

        var attributeRequest = new AttributeRequestUpdate
        {
            Id = attributeId.ToString(),
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdRelatedClass = classId.ToString(),
            IdReference = typeId.ToString(),
            IdInstance = instanceId.ToString(),
        };

        var relatedClass = new SimClass { Id = classId, Name = "TestClass" };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        var instanceClass = new SimClass { Id = instanceId, Name = "InstanceClass" };

        _mockSimClassService!.Setup(s => s.GetSimClassById(classId)).Returns(relatedClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(typeId)).Returns(typeClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(instanceId)).Returns(instanceClass);
        _mockSimClassService!
            .Setup(s => s.ValidPolymorphism(It.IsAny<SimClass>(), It.IsAny<SimClass>()));

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
            Reference = new SimClass { Id = typeId, Name = "string" },
            ReferenceId = typeId,
            RelatedClass = new SimClass { Id = relatedClassId, Name = "Owner" },
            RelatedClassId = relatedClassId,
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
        Assert.AreEqual(typeId, result.ReferenceId);

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

    [TestMethod]
    public void UpdateAttribute_WithValidData_UpdatesAndReturnsResponse()
    {
        var attributeId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();

        var attributeRequest = new AttributeRequestUpdate
        {
            Id = attributeId.ToString(),
            Name = "UpdatedAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Protected,
            IdRelatedClass = relatedClassId.ToString(),
            IdReference = typeId.ToString(),
            IdInstance = instanceId.ToString(),
        };

        var relatedClass = new SimClass { Id = relatedClassId, Name = "RelatedClass" };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        var instanceClass = new SimClass { Id = instanceId, Name = "InstanceClass" };

        var updatedSimAttribute = new SimAttribute()
        {
            Id = attributeId,
            Name = "UpdatedAttribute",
            Privacity = SimPrivacity.Protected,
            RelatedClass = relatedClass,
            RelatedClassId = relatedClassId,
            Reference = typeClass,
            ReferenceId = typeId,
            Instance = instanceClass,
            InstanceId = instanceId,
        };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(relatedClassId))
            .Returns(relatedClass);
        _mockSimClassService!
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(typeClass);
        _mockSimClassService!
            .Setup(s => s.GetSimClassById(instanceId))
            .Returns(instanceClass);

        _mockSimAttributeService!
            .Setup(s => s.UpdateAttribute(attributeId, It.IsAny<SimAttribute>()))
            .Returns(updatedSimAttribute);

        _mockSimClassService!
            .Setup(s => s.ValidPolymorphism(It.IsAny<SimClass>(), It.IsAny<SimClass>()));

        var result = _simAttributeAdapter!.UpdateAttribute(attributeRequest);

        _mockSimClassService.Verify(s => s.GetSimClassById(relatedClassId), Times.Once);
        _mockSimClassService.Verify(s => s.GetSimClassById(typeId), Times.Once);
        _mockSimClassService.Verify(s => s.GetSimClassById(instanceId), Times.Once);
        _mockSimAttributeService.Verify(s => s.UpdateAttribute(attributeId, It.IsAny<SimAttribute>()), Times.Once);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Attribute);
        Assert.AreEqual(attributeId, result.Attribute.Id);
        Assert.AreEqual("UpdatedAttribute", result.Attribute.Name);
        Assert.AreEqual(Models.Enums.SimModelsPrivacity.Protected, result.Attribute.Privacity);
        Assert.AreEqual(relatedClassId, result.Attribute.RelatedClassId);
        Assert.AreEqual(typeId, result.Attribute.ReferenceId);
        Assert.AreEqual(instanceId, result.Attribute.InstanceId);
        Assert.AreEqual("Attribute updated successfully.", result.Message);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueAdapter))]
    public void UpdateAttribute_ThrowsInUseValueAdapter_WhenServiceThrowsInUseValueLogic()
    {
        var attributeId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();

        var attributeRequest = new AttributeRequestUpdate
        {
            Id = attributeId.ToString(),
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdRelatedClass = relatedClassId.ToString(),
            IdReference = typeId.ToString(),
            IdInstance = instanceId.ToString(),
        };

        var relatedClass = new SimClass { Id = relatedClassId, Name = "TestClass" };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        var instanceClass = new SimClass { Id = instanceId, Name = "InstanceClass" };

        _mockSimClassService!.Setup(s => s.GetSimClassById(relatedClassId)).Returns(relatedClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(typeId)).Returns(typeClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(instanceId)).Returns(instanceClass);
        _mockSimClassService!
            .Setup(s => s.ValidPolymorphism(It.IsAny<SimClass>(), It.IsAny<SimClass>()));
        _mockSimAttributeService!.Setup(s => s.UpdateAttribute(attributeId, It.IsAny<SimAttribute>()))
            .Throws(new InUseValueLogic("Attribute in use"));

        _simAttributeAdapter!.UpdateAttribute(attributeRequest);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void UpdateAttribute_ThrowsInvalidAttributeAdapter_WhenServiceThrowsInvalidAttributeLogic()
    {
        var attributeId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();

        var attributeRequest = new AttributeRequestUpdate
        {
            Id = attributeId.ToString(),
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdRelatedClass = relatedClassId.ToString(),
            IdReference = typeId.ToString(),
            IdInstance = instanceId.ToString(),
        };

        var relatedClass = new SimClass { Id = relatedClassId, Name = "TestClass" };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        var instanceClass = new SimClass { Id = instanceId, Name = "InstanceClass" };

        _mockSimClassService!.Setup(s => s.GetSimClassById(relatedClassId)).Returns(relatedClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(typeId)).Returns(typeClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(instanceId)).Returns(instanceClass);
        _mockSimClassService!.Setup(s => s.ValidPolymorphism(It.IsAny<SimClass>(), It.IsAny<SimClass>()));
        _mockSimAttributeService!.Setup(s => s.UpdateAttribute(attributeId, It.IsAny<SimAttribute>()))
            .Throws(new InvalidAttributeLogic("Test logic validation error"));

        _simAttributeAdapter!.UpdateAttribute(attributeRequest);
    }

    [TestMethod]
    public void CreateAttribute_ThrowsInvalidAttributeAdapter_WhenServiceThrowsInvalidAttributeLogic()
    {
        var classId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();

        var attributeRequest = new AttributeRequest
        {
            Name = "TestAttribute",
            Privacity = Models.Enums.SimModelsPrivacity.Public,
            IdReference = typeId.ToString(),
            IdInstance = instanceId.ToString(),
            IsStatic = false,
        };

        var relatedClass = new SimClass { Id = classId, Name = "RelatedClass" };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        var instanceClass = new SimClass { Id = instanceId, Name = "InstanceClass" };

        _mockSimClassService!.Setup(s => s.GetSimClassById(classId)).Returns(relatedClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(typeId)).Returns(typeClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(instanceId)).Returns(instanceClass);
        _mockSimClassService!.Setup(s => s.ValidPolymorphism(typeClass, instanceClass));
        _mockSimAttributeService!
            .Setup(s => s.CreateAttribute(classId, It.IsAny<SimAttribute>()))
            .Throws(new InvalidAttributeLogic("Test logic validation error"));

        var ex = Assert.ThrowsException<InvalidAttributeAdapter>(() =>
            _simAttributeAdapter!.CreateAttribute(classId, attributeRequest));

        Assert.AreEqual("Test logic validation error", ex.Message);
    }
}
