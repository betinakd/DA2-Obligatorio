using BussinesLogic;
using Domain;
using Domain.Enums;
using IDataAccess;
using Moq;

namespace Tests.BussinesLogic;

[TestClass]
public class SimAttributeServiceTest
{
    private Mock<ISimAttributeDataAccess>? _mockSimAttributeDataAccess;
    private SimAttributeService? _simAttributeService;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimAttributeDataAccess = new Mock<ISimAttributeDataAccess>(MockBehavior.Strict);
        _simAttributeService = new SimAttributeService(_mockSimAttributeDataAccess.Object);
    }

    [TestMethod]
    public void CreateAttribute_ShouldReturnCreatedAttribute_WhenAttributeIsValid()
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

        _mockSimAttributeDataAccess.Setup(da => da.CreateAttribute(classId, attribute)).Returns(attribute);

        var result = _simAttributeService.CreateAttribute(classId, attribute);

        _mockSimAttributeDataAccess.Verify(da => da.CreateAttribute(classId, attribute), Times.Once);

        Assert.IsNotNull(result);
        Assert.AreEqual(attributeId, result.Id);
        Assert.AreEqual("TestAttribute", result.Name);
        Assert.AreEqual(SimPrivacity.Public, result.Privacity);
        Assert.AreEqual(relatedClass, result.RelatedClass);
        Assert.AreEqual(typeClass, result.Type);
    }
}
