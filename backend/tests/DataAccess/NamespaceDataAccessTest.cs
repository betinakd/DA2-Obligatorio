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
        var simNamespace2 = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace2", BaseNamespaceId = simNamespace1Id };

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

    [TestMethod]
    public void GetNamespaceById_ShouldReturnNamespace_WhenNamespaceExists()
    {
        var simNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "ExistingNamespace", BaseNamespaceId = null };
        _namespaceDataAccess.CreateNamespace(simNamespace);

        var retrievedNamespace = _namespaceDataAccess.GetNamespaceById(simNamespace.Id);

        Assert.IsNotNull(retrievedNamespace);
        Assert.AreEqual(simNamespace.Id, retrievedNamespace.Id);
        Assert.AreEqual(simNamespace.Name, retrievedNamespace.Name);
        Assert.AreEqual(simNamespace.BaseNamespaceId, retrievedNamespace.BaseNamespaceId);
    }

    [TestMethod]
    public void GetNamespaceById_ShouldReturnNull_WhenNamespaceNonExists()
    {
        var nonExistentId = Guid.NewGuid();

        var retrievedNamespace = _namespaceDataAccess.GetNamespaceById(nonExistentId);

        Assert.IsNull(retrievedNamespace);
    }

    [TestMethod]
    public void GetAllNamespaces_ShouldOrderAndLoadReferences_ForAllReferenceTypes()
    {
        var simNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "TestNamespace" };
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass", Namespace = simNamespace };
        simNamespace.Elements.Add(simClass);

        var param1 = new Parameter { Id = Guid.NewGuid(), Name = "param1", Index = 1 };
        var param0 = new Parameter { Id = Guid.NewGuid(), Name = "param0", Index = 0 };

        var localVar = new LocalVariable
        {
            Id = Guid.NewGuid(),
            Name = "localVar",
            Reference = new SimClass { Id = Guid.NewGuid(), Name = "VarType" },
            Instance = new SimClass { Id = Guid.NewGuid(), Name = "VarInstance" }
        };

        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "attribute",
            Reference = new SimClass { Id = Guid.NewGuid(), Name = "AttrType" },
            Instance = new SimClass { Id = Guid.NewGuid(), Name = "AttrInstance" }
        };

        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        simClass.BaseClass = baseClass;

        var method = new SimMethod { Id = Guid.NewGuid(), Name = "TestMethod", IsStatic = false };
        method.Parameters.Add(param1);
        method.Parameters.Add(param0);
        method.LocalVariables.Add(localVar);

        var sigParam1 = new ParameterSignature { Id = Guid.NewGuid(), Name = "sigParam1", Index = 1 };
        var sigParam0 = new ParameterSignature { Id = Guid.NewGuid(), Name = "sigParam0", Index = 0 };
        var signature = new Signature
        {
            Id = Guid.NewGuid(),
            Name = "SignatureMethod",
            Parameters = [sigParam1, sigParam0]
        };

        var invocations = new List<Invocation>
    {
        new Invocation
        {
            Id = Guid.NewGuid(),
            Index = 2,
            Reference = new ReferenceParameter { Reference = param1 },
            Signature = signature
        },
        new Invocation
        {
            Id = Guid.NewGuid(),
            Index = 1,
            Reference = new ReferenceVariable { Reference = localVar },
            Signature = signature
        },
        new Invocation
        {
            Id = Guid.NewGuid(),
            Index = 4,
            Reference = new ReferenceAttribute { Reference = attribute },
            Signature = signature
        },
        new Invocation
        {
            Id = Guid.NewGuid(),
            Index = 0,
            Reference = new ReferenceBase { Reference = baseClass },
            Signature = signature
        },
        new Invocation
        {
            Id = Guid.NewGuid(),
            Index = 3,
            Reference = new ReferenceThis { Reference = simClass },
            Signature = signature
        },
        new Invocation
        {
            Id = Guid.NewGuid(),
            Index = 7,
            Reference = new ReferenceParameter { Reference = param0 },
            Signature = signature
        }
    };

        foreach(var inv in invocations)
        {
            method.Invocations.Add(inv);
        }

        simClass.Methods.Add(method);
        simClass.Attributes.Add(attribute);

        _context.SimNamespaces.Add(simNamespace);
        _context.SaveChanges();

        var result = _namespaceDataAccess.GetAllNamespaces();
        var ns = result.FirstOrDefault(n => n.Id == simNamespace.Id);
        var cls = ns.Elements.FirstOrDefault(c => c.Id == simClass.Id);
        var mth = cls.Methods.FirstOrDefault(m => m.Id == method.Id);

        Assert.IsNotNull(result);
        Assert.IsNotNull(ns);
        Assert.IsNotNull(cls);
        Assert.IsNotNull(mth);

        Assert.AreEqual(0, mth.Parameters[0].Index);
        Assert.AreEqual(1, mth.Parameters[1].Index);
        Assert.AreEqual(0, mth.Invocations[0].Index);
        Assert.AreEqual(1, mth.Invocations[1].Index);
    }
}
