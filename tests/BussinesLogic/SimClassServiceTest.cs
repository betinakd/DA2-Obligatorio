using BussinesLogic;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
using IDataAccess;
using Moq;

namespace Tests.BussinesLogic;

[TestClass]
public class SimClassServiceTest
{
    private Mock<ISimClassDataAccess>? _mockSimClassDataAccess;
    private SimClassService? _simClassService;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimClassDataAccess = new Mock<ISimClassDataAccess>(MockBehavior.Strict);
        _simClassService = new SimClassService(_mockSimClassDataAccess.Object);
    }

    [TestMethod]
    public void CreateSimClass_ShouldReturnSimClassWithCorrectProperties()
    {
        var baseClassId = Guid.NewGuid();
        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass" };
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassName("TestClass")).Returns(false);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(baseClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(baseClassId)).Returns(baseClass);
        _mockSimClassDataAccess.Setup(da => da.CreateSimClass(It.IsAny<SimClass>()));

        var result = _simClassService.CreateSimClass("TestClass", SimAccesibility.Normal, baseClassId);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassName("TestClass"), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.CreateSimClass(It.Is<SimClass>(
            s => s.Name == "TestClass" &&
                 s.BaseClass == baseClass &&
                 s.Id != Guid.Empty &&
                 s.State == SimAccesibility.Normal)), Times.Once);

        Assert.IsNotNull(result);
        Assert.AreEqual("TestClass", result.Name);
        Assert.AreEqual(baseClass, result.BaseClass);
        Assert.AreNotEqual(Guid.Empty, result.Id);
    }

    [TestMethod]
    public void CreateSimClass_ShouldThrowDuplicateValueLogic_WhenNameAlreadyExists()
    {
        var baseClassId = Guid.NewGuid();
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassName("TestClass")).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(baseClassId)).Returns(true);

        Assert.ThrowsException<DuplicateValueLogic>(() =>
            _simClassService.CreateSimClass("TestClass", SimAccesibility.Normal, baseClassId));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassName("TestClass"), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void CreateSimClass_ShouldThrowNonExistentValueLogic_WhenBaseClassDoesNotExist()
    {
        var baseClassId = Guid.NewGuid();
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassName("TestClass")).Returns(false);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(baseClassId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simClassService.CreateSimClass("TestClass", SimAccesibility.Normal, baseClassId));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassName("TestClass"), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void CreateSimClass_ShouldThrowInvalidAttributeLogic_WhenSimClassInvalidAttributeIsThrown()
    {
        var baseClassId = Guid.NewGuid();
        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass" };
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassName("TestClass")).Returns(false);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(baseClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(baseClassId)).Returns(baseClass);
        _mockSimClassDataAccess.Setup(da => da.CreateSimClass(It.IsAny<SimClass>())).Throws(new InvalidAttributeDomain("Invalid attribute"));

        Assert.ThrowsException<InvalidAttributeLogic>(() =>
            _simClassService.CreateSimClass("TestClass", SimAccesibility.Normal, baseClassId));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassName("TestClass"), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.CreateSimClass(It.IsAny<SimClass>()), Times.Once);
    }

    [TestMethod]
    public void CreateSimClass_ShouldCreateAndReturnSimClass_WhenAllIsValid()
    {
        var baseClassId = Guid.NewGuid();
        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass" };
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassName("TestClass")).Returns(false);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(baseClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(baseClassId)).Returns(baseClass);
        _mockSimClassDataAccess.Setup(da => da.CreateSimClass(It.IsAny<SimClass>()));

        var result = _simClassService.CreateSimClass("TestClass", SimAccesibility.Normal, baseClassId);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassName("TestClass"), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.CreateSimClass(It.Is<SimClass>(
            s => s.Name == "TestClass" &&
                 s.BaseClass == baseClass &&
                 s.Id != Guid.Empty &&
                 s.State == SimAccesibility.Normal)), Times.Once);

        Assert.IsNotNull(result);
        Assert.AreEqual("TestClass", result.Name);
        Assert.AreEqual(baseClass, result.BaseClass);
        Assert.AreNotEqual(Guid.Empty, result.Id);
    }

    [TestMethod]
    public void DeleteSimClass_ShouldDelete_WhenSimClassExistsAndNotInUse()
    {
        var simClassId = Guid.NewGuid();
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClassId)).Returns(false);
        _mockSimClassDataAccess.Setup(da => da.DeleteSimClass(simClassId));

        _simClassService.DeleteSimClass(simClassId);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.InUseByOther(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.DeleteSimClass(simClassId), Times.Once);
    }

    [TestMethod]
    public void DeleteSimClass_ShouldThrowNonExistentValueLogic_WhenSimClassDoesNotExist()
    {
        var simClassId = Guid.NewGuid();
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClassId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simClassService.DeleteSimClass(simClassId));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.InUseByOther(It.IsAny<Guid>()), Times.Never);
        _mockSimClassDataAccess.Verify(da => da.DeleteSimClass(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void GetAllSimClasses_ShouldReturnAllSimClasses()
    {
        var simClasses = new List<SimClass>
        {
            new SimClass { Id = Guid.NewGuid(), Name = "Class1" },
            new SimClass { Id = Guid.NewGuid(), Name = "Class2" }
        };
        _mockSimClassDataAccess.Setup(da => da.GetAllSimClasses()).Returns(simClasses);

        var result = _simClassService.GetAllSimClasses();

        _mockSimClassDataAccess.Verify(da => da.GetAllSimClasses(), Times.Once);
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(simClasses, result.ToList());
    }

    [TestMethod]
    public void GetSimClassById_ShouldReturnSimClass()
    {
        var simClassId = Guid.NewGuid();
        var simClass = new SimClass { Id = simClassId, Name = "TestClass" };
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(simClassId)).Returns(simClass);

        var result = _simClassService.GetSimClassById(simClassId);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(simClassId), Times.Once);
        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
    }

    [TestMethod]
    public void GetSimClassById_ShouldThrowNonExistentValueLogic_WhenNotExists()
    {
        var simClassId = Guid.NewGuid();
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClassId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simClassService.GetSimClassById(simClassId));
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldUpdateAndReturnSimClass_WhenExists()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "UpdatedClass" };
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClass.Id)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.UpdateSimClass(simClass));
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClass.Id)).Returns(false);
        var result = _simClassService.UpdateSimClass(simClass);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClass.Id), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.UpdateSimClass(simClass), Times.Once);
        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldThrowNonExistentValueLogic_WhenNotExists()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "UpdatedClass" };
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClass.Id)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simClassService.UpdateSimClass(simClass));
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClass.Id), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.UpdateSimClass(It.IsAny<SimClass>()), Times.Never);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldThrowInUseValueLogic_WhenSimClassIsInUse()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClass.Id)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClass.Id)).Returns(true);

        Assert.ThrowsException<InUseValueLogic>(() =>
            _simClassService.UpdateSimClass(simClass));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClass.Id), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.InUseByOther(simClass.Id), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.UpdateSimClass(It.IsAny<SimClass>()), Times.Never);
    }

    [TestMethod]
    public void DeleteSimClass_ShouldThrowInUseValueLogic_WhenSimClassIsInUse()
    {
        var simClassId = Guid.NewGuid();
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClassId)).Returns(true);

        Assert.ThrowsException<InUseValueLogic>(() =>
            _simClassService.DeleteSimClass(simClassId));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.InUseByOther(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.DeleteSimClass(It.IsAny<Guid>()), Times.Never);
    }
}
