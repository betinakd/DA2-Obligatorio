using BussinesLogic;
using Domain;
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
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(baseClassId)).Returns(baseClass);

        var result = _simClassService.CreateSimClass("TestClass", false, false, baseClassId);

        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(baseClassId), Times.Once);
        Assert.IsNotNull(result);
        Assert.AreEqual("TestClass", result.Name);
        Assert.AreEqual(baseClass, result.BaseClass);
        Assert.AreNotEqual(Guid.Empty, result.Id);
    }
}
