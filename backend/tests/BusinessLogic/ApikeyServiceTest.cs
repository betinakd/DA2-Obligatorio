using BusinessLogic;
using IDataAccess;
using Moq;

namespace Tests.BusinessLogic;

[TestClass]
public class ApikeyServiceTest
{
    private Mock<IApikeyDataAccess>? _mockApikeyDataAccess;
    private ApikeyService? _apikeyService;

    [TestInitialize]
    public void Initialize()
    {
        _mockApikeyDataAccess = new Mock<IApikeyDataAccess>(MockBehavior.Strict);
        _apikeyService = new ApikeyService(_mockApikeyDataAccess.Object);
    }

    [TestMethod]
    public void IsAuthorizedUser_ValidApiKey_ReturnsTrue()
    {
        var apiKey = Guid.NewGuid();

        _mockApikeyDataAccess!
            .Setup(m => m.ApiKeyExists(apiKey))
            .Returns(true);

        var result = _apikeyService!.IsAuthorizedUser(apiKey);

        Assert.IsTrue(result);
        _mockApikeyDataAccess.Verify(m => m.ApiKeyExists(apiKey), Times.Once);
    }

    [TestMethod]
    public void IsAuthorizedUser_NonExistentApiKey_ReturnsFalse()
    {
        var apiKey = Guid.NewGuid();

        _mockApikeyDataAccess!
            .Setup(m => m.ApiKeyExists(apiKey))
            .Returns(false);

        var result = _apikeyService!.IsAuthorizedUser(apiKey);

        Assert.IsFalse(result);
        _mockApikeyDataAccess.Verify(m => m.ApiKeyExists(apiKey), Times.Once);
    }

    [TestMethod]
    public void IsAuthorizedUser_EmptyGuid_ReturnsFalse()
    {
        var apiKey = Guid.Empty;

        _mockApikeyDataAccess!
            .Setup(m => m.ApiKeyExists(apiKey))
            .Returns(true);

        var result = _apikeyService!.IsAuthorizedUser(apiKey);

        Assert.IsFalse(result);
        _mockApikeyDataAccess.Verify(m => m.ApiKeyExists(apiKey), Times.Once);
    }
}
