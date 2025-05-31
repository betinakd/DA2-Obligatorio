using DataAccess;
using DataAccess.Context;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Tests.DataAccess;

[TestClass]
public class NamespaceDataAccessTest
{
    private SimulatorDbContext? _context;
    private NamespaceDataAccess? _namespaceDataAccess;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: "Simulator")
            .Options;

        _context = new SimulatorDbContext(options);
        _namespaceDataAccess = new NamespaceDataAccess(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void GetAllNamespaces_ShouldReturnEmptyList_WhenNoNamespacesExist()
    {
        var namespaces = _namespaceDataAccess.GetAllNamespaces();

        Assert.IsNotNull(namespaces);
        Assert.AreEqual(0, namespaces.Count);
    }

    [TestMethod]
    public void GetAllNamespace_s_ShouldReturnNamespaces_WhenNamespacesExist()
    {
        var simNamespace1Id = Guid.NewGuid();
        var simNamespace1 = new SimNamespace { Id = simNamespace1Id, Name = "Namespace1", BaseNamespaceId = null };
        var simNamespace2 = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace2" , BaseNamespaceId = simNamespace1Id };

        _context.SimNamespaces.Add(simNamespace1);
        _context.SimNamespaces.Add(simNamespace2);
        _context.SaveChanges();

        var namespaces = _namespaceDataAccess.GetAllNamespaces();

        Assert.IsNotNull(namespaces);
        Assert.AreEqual(2, namespaces.Count);
        Assert.IsTrue(namespaces.Any(ns => ns.Id == simNamespace1.Id));
        Assert.IsTrue(namespaces.Any(ns => ns.Id == simNamespace2.Id));
    }

    [TestMethod]
    public void CreateNamespace_ShouldAddNamespace_WhenValidNamespaceProvided()
    {
        var simNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "NewNamespace", BaseNamespaceId = null };

        _namespaceDataAccess.CreateNamespace(simNamespace);

        var namespaces = _namespaceDataAccess.GetAllNamespaces();

        Assert.IsNotNull(namespaces);
        Assert.AreEqual(1, namespaces.Count);
        Assert.AreEqual(simNamespace.Id, namespaces[0].Id);
        Assert.AreEqual(simNamespace.Name, namespaces[0].Name);
    }

    [TestMethod]
    public void NamespaceExistsById_ShouldReturnTrue_WhenNamespaceExists()
    {
        var simNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "ExistingNamespace", BaseNamespaceId = null };
        _namespaceDataAccess.CreateNamespace(simNamespace);

        var exists = _namespaceDataAccess.NamespaceExistsById(simNamespace.Id);

        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void NamespaceExistsById_ShouldReturnFalse_WhenNamespaceNonExists()
    {
        var simNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "ExistingNamespace", BaseNamespaceId = null };

        var exists = _namespaceDataAccess.NamespaceExistsById(simNamespace.Id);

        Assert.IsFalse(exists);
    }
}
