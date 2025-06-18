using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Tests.DataAccess;

[TestClass]
public class ApiKeyDataAccessTest
{
    private SimulatorDbContext? _context;
    private ApikeyDataAccess? _executionDataAccess;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: "Simulator")
            .Options;

        _context = new SimulatorDbContext(options);
        _executionDataAccess = new ApikeyDataAccess(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void ApiKeyExists_ShouldReturnTrue_WhenApiKeyExists()
    {
        var apiKey = new ApiKey { KeyValue = Guid.NewGuid(), Name = "TestKey" };
        _context.ApiKeys.Add(apiKey);
        _context.SaveChanges();

        var exists = _executionDataAccess!.ApiKeyExists(apiKey.KeyValue);
        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void ApiKeyExists_ShouldReturnFalse_WhenApiKeyDontExists()
    {
        var apiKey = new ApiKey { KeyValue = Guid.NewGuid() };

        var exists = _executionDataAccess!.ApiKeyExists(apiKey.KeyValue);
        Assert.IsFalse(exists);
    }
}
