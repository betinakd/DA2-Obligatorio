using Adapter;
using IBussinesLogic;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class AttributeAdapterTest
{
    private Mock<ISimAttributeService>? _mockSimAttributeService;
    private AttributeAdapter? _simAttributeAdapter;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimAttributeService = new Mock<ISimAttributeService>(MockBehavior.Strict);
        _simAttributeAdapter = new AttributeAdapter(_mockSimAttributeService.Object);
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
}
