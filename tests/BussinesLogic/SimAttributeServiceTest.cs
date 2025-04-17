using BussinesLogic;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Enums;
using IDataAccess;
using Moq;

namespace Tests.BussinesLogic;

[TestClass]
public class SimAttributeServiceTest
{
    private Mock<ISimAttributeDataAccess>? _mockSimAttributeDataAccess;
    private Mock<ISimClassDataAccess>? _mockSimClassDataAccess;
    private SimAttributeService? _simAttributeService;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimAttributeDataAccess = new Mock<ISimAttributeDataAccess>(MockBehavior.Strict);
        _mockSimClassDataAccess = new Mock<ISimClassDataAccess>(MockBehavior.Strict);
        _simAttributeService = new SimAttributeService(_mockSimAttributeDataAccess.Object, _mockSimClassDataAccess.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void CreateAttribute_ShouldThrowException_WhenClassDoesNotExist()
    {
        var classId = Guid.NewGuid();
        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "TestAttribute",
            RelatedClass = new SimClass { Id = classId, Name = "TestClass" },
            Type = new SimClass { Id = Guid.NewGuid(), Name = "TypeClass" },
            Privacity = SimPrivacity.Public
        };

        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(classId)).Returns(false);

        _simAttributeService.CreateAttribute(classId, attribute);
    }

    [TestMethod]
    public void CreateAttribute_ShouldReturnCreatedAttribute_WhenClassExists()
    {
        var classId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var relatedClass = new SimClass { Id = classId, Name = "TestClass" };
        var typeClass = new SimClass { Id = Guid.NewGuid(), Name = "TypeClass" };

        var attribute = new SimAttribute
        {
            Id = attributeId,
            Name = "TestAttribute",
            RelatedClass = relatedClass,
            Type = typeClass,
            Privacity = SimPrivacity.Public
        };

        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(classId)).Returns(true);
        _mockSimAttributeDataAccess.Setup(da => da.CreateAttribute(classId, attribute)).Returns(attribute);
        _mockSimAttributeDataAccess.Setup(da => da.ExistAttributeName(classId, attribute.Name)).Returns(false);

        var result = _simAttributeService.CreateAttribute(classId, attribute);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(classId), Times.Once);
        _mockSimAttributeDataAccess.Verify(da => da.CreateAttribute(classId, attribute), Times.Once);

        Assert.IsNotNull(result);
        Assert.AreEqual(attributeId, result.Id);
        Assert.AreEqual("TestAttribute", result.Name);
        Assert.AreEqual(SimPrivacity.Public, result.Privacity);
        Assert.AreEqual(relatedClass, result.RelatedClass);
        Assert.AreEqual(typeClass, result.Type);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void CreateAttribute_ShouldThrowException_WhenAttributeNameAlreadyExists()
    {
        var classId = Guid.NewGuid();
        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "ExistingAttribute",
            RelatedClass = new SimClass { Id = classId, Name = "TestClass" },
            Type = new SimClass { Id = Guid.NewGuid(), Name = "TypeClass" },
            Privacity = SimPrivacity.Public
        };

        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(classId)).Returns(true);
        _mockSimAttributeDataAccess.Setup(da => da.ExistAttributeName(classId, attribute.Name)).Returns(true);

        _simAttributeService.CreateAttribute(classId, attribute);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void DeleteAttribute_ShouldThrowException_WhenAttributeDoesNotExist()
    {
        var attributeId = Guid.NewGuid();
        _mockSimAttributeDataAccess.Setup(da => da.ExistAttributeById(attributeId)).Returns(false);

        _simAttributeService.DeleteAttribute(attributeId);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void DeleteAttribute_ShouldThrowException_WhenAttributeIsInUseByAnotherEntity()
    {
        var attributeId = Guid.NewGuid();
        _mockSimAttributeDataAccess.Setup(da => da.ExistAttributeById(attributeId)).Returns(true);
        _mockSimAttributeDataAccess.Setup(da => da.InUseByOther(attributeId)).Returns(true);

        _simAttributeService.DeleteAttribute(attributeId);
    }

    [TestMethod]
    public void DeleteAttribute_ShouldSucceed_WhenAttributeExistsAndNotInUse()
    {
        var attributeId = Guid.NewGuid();
        _mockSimAttributeDataAccess.Setup(da => da.ExistAttributeById(attributeId)).Returns(true);
        _mockSimAttributeDataAccess.Setup(da => da.InUseByOther(attributeId)).Returns(false);
        _mockSimAttributeDataAccess.Setup(da => da.DeleteAttribute(attributeId)).Verifiable();

        _simAttributeService.DeleteAttribute(attributeId);

        _mockSimAttributeDataAccess.Verify(da => da.ExistAttributeById(attributeId), Times.Once);
        _mockSimAttributeDataAccess.Verify(da => da.InUseByOther(attributeId), Times.Once);
        _mockSimAttributeDataAccess.Verify(da => da.DeleteAttribute(attributeId), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void UpdateAttribute_ShouldThrowException_WhenAttributeDoesNotExist()
    {
        var attributeId = Guid.NewGuid();
        var attribute = new SimAttribute
        {
            Id = attributeId,
            Name = "TestAttribute",
            RelatedClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" },
            Type = new SimClass { Id = Guid.NewGuid(), Name = "TypeClass" },
            Privacity = SimPrivacity.Public
        };

        _mockSimAttributeDataAccess.Setup(da => da.ExistAttributeById(attributeId)).Returns(false);
        _mockSimAttributeDataAccess.Setup(da => da.InUseByOther(attributeId)).Returns(false);

        _simAttributeService.UpdateAttribute(attributeId, attribute);
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueLogic))]
    public void UpdateAttribute_ShouldThrowException_WhenAttributeIsInUseByAnotherEntity()
    {
        var attributeId = Guid.NewGuid();
        var attribute = new SimAttribute
        {
            Id = attributeId,
            Name = "TestAttribute",
            RelatedClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" },
            Type = new SimClass { Id = Guid.NewGuid(), Name = "TypeClass" },
            Privacity = SimPrivacity.Public
        };

        _mockSimAttributeDataAccess.Setup(da => da.ExistAttributeById(attributeId)).Returns(true);
        _mockSimAttributeDataAccess.Setup(da => da.InUseByOther(attributeId)).Returns(true);

        _simAttributeService.UpdateAttribute(attributeId, attribute);
    }
}
