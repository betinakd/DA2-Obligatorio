using DataAccess;
using DataAccess.Context;
using Domain;
using Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.DataAccess;

[TestClass]
public class ExecutionDataAccessTest
{
    private SimulatorDbContext? _context;
    private ExecutionDataAccess? _executionDataAccess;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: "Simulator")
            .Options;

        _context = new SimulatorDbContext(options);
        _executionDataAccess = new ExecutionDataAccess(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void TestFindMethodInHierarchy_ThroughPublicMethods()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var intType = new SimClass { Name = "int" };
        _context.SimClasses.Add(intType);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Public,
            Accesibility = SimAccesibility.Sealed
        };

        var parameter = new Parameter
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType,
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod
        };
        baseMethod.Parameters.Add(parameter);

        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var signature = new Signature { Name = "TestMethod" };
        var paramSignature = new ParameterSignature
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType,
            SignatureId = signature.Id
        };
        signature.Parameters.Add(paramSignature);

        var result = _executionDataAccess.FindMethodInHierarchy(childClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
    }
}
