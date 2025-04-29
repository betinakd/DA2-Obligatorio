using DataAccess;
using DataAccess.Context;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Tests.DataAccess;

[TestClass]
public class SimMethodDataAccessTest
{
    private SimulatorDbContext? _context;
    private SimMethodDataAccess? _simMethodDataAccess;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: "Simulator")
            .Options;

        _context = new SimulatorDbContext(options);
        _simMethodDataAccess = new SimMethodDataAccess(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void AddLocalVariable_ShouldAddLocalVariableToDatabase()
    {
        var typeId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var localVariable = new LocalVariable()
        {
            Id = Guid.NewGuid(),
            TypeId = typeId,
            Name = "TestLocalVariable",
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "TestType" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };

        var result = _simMethodDataAccess.AddLocalVariable(methodId, localVariable);

        var variableInDb = _context.LocalVariables.FirstOrDefault(v => v.Id == localVariable.Id);
        Assert.IsNotNull(variableInDb);
        Assert.AreEqual(localVariable.Name, variableInDb.Name);
        Assert.AreEqual(localVariable.Type, variableInDb.Type);
        Assert.AreEqual(localVariable.RelatedMethodId, variableInDb.RelatedMethodId);
        Assert.AreEqual(localVariable.Id, result.Id);
    }

    [TestMethod]
    public void AddMethodParameter_ShouldAddParameterToDatabase()
    {
        var methodId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "TestParameter",
            TypeId = typeId,
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "TestType" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };

        var result = _simMethodDataAccess.AddMethodParameter(methodId, parameter);

        var parameterInDb = _context.Parameters.FirstOrDefault(p => p.Id == parameter.Id);
        Assert.IsNotNull(parameterInDb);
        Assert.AreEqual(parameter.Name, parameterInDb.Name);
        Assert.AreEqual(parameter.TypeId, parameterInDb.TypeId);
        Assert.AreEqual(parameter.RelatedMethodId, parameterInDb.RelatedMethodId);
        Assert.AreEqual(parameter.Id, result.Id);
    }

    [TestMethod]
    public void CreateInvocation_ShouldAddInvocationToDatabase()
    {
        var referenceId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var reference = new ReferenceThis { Reference = new SimClass() { Id = referenceId } };
        var signature = new Signature { Name = "TestSignature", Parameters = [] };
        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Reference = reference,
            Signature = signature,
            RelatedMethodId = methodId,
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };

        var result = _simMethodDataAccess.CreateInvocation(methodId, invocation);

        var invocationInDb = _context.Invocations.FirstOrDefault(i => i.Id == invocation.Id);
        Assert.IsNotNull(invocationInDb);
        Assert.AreEqual(invocation.RelatedMethodId, invocationInDb.RelatedMethodId);
        Assert.AreEqual(invocation.Id, result.Id);
    }

    [TestMethod]
    public void CreateMethod_ShouldAddMethodToDatabase()
    {
        var simClassId = Guid.NewGuid();
        var simMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            RelatedClassId = simClassId,
            Name = "Test Method"
        };

        var result = _simMethodDataAccess.CreateMethod(simClassId, simMethod);

        var methodInDb = _context.SimMethods.FirstOrDefault(m => m.Id == simMethod.Id);
        Assert.IsNotNull(methodInDb);
        Assert.AreEqual(simMethod.Name, methodInDb.Name);
        Assert.AreEqual(simMethod.RelatedClassId, methodInDb.RelatedClassId);
        Assert.AreEqual(simMethod.Id, result.Id);
    }

    [TestMethod]
    public void DeleteMethod_ShouldRemoveMethodFromDatabase()
    {
        var simClassId = Guid.NewGuid();
        var simMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            RelatedClassId = simClassId,
            Name = "MethodToDelete"
        };
        _context.SimMethods.Add(simMethod);
        _context.SaveChanges();

        _simMethodDataAccess.DeleteMethod(simMethod.Id);

        var methodInDb = _context.SimMethods.Find(simMethod.Id);
        Assert.IsNull(methodInDb);
    }

    [TestMethod]
    public void ExistInvocationById_ShouldReturnTrue_WhenInvocationExists()
    {
        var invocationId = Guid.NewGuid();
        var methodId = Guid.NewGuid();

        var signature = new Signature { Name = "TestSignature", Parameters = [] };
        var invocation = new Invocation
        {
            Id = invocationId,
            RelatedMethodId = methodId,
            Signature = signature
        };
        _context.Invocations.Add(invocation);
        _context.SaveChanges();

        var exists = _simMethodDataAccess.ExistInvocationById(invocationId);

        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void ExistInvocationById_ShouldReturnFalse_WhenInvocationDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();

        var exists = _simMethodDataAccess.ExistInvocationById(nonExistentId);

        Assert.IsFalse(exists);
    }

    [TestMethod]
    public void ExistMethodById_ShouldReturnTrue_WhenMethodExists()
    {
        var simClassId = Guid.NewGuid();
        var simMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            RelatedClassId = simClassId,
            Name = "ExistingMethod"
        };
        _context.SimMethods.Add(simMethod);
        _context.SaveChanges();

        var exists = _simMethodDataAccess.ExistMethodById(simMethod.Id);

        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void ExistMethodById_ShouldReturnFalse_WhenMethodDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();

        var exists = _simMethodDataAccess.ExistMethodById(nonExistentId);

        Assert.IsFalse(exists);
    }

    [TestMethod]
    public void ExistParameter_ShouldReturnTrue_WhenParameterExists()
    {
        var methodId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "ExistingParameter",
            TypeId = typeId,
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "TestType" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };
        _context.Parameters.Add(parameter);
        _context.SaveChanges();

        var exists = _simMethodDataAccess.ExistParameter(parameter.Id);

        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void ExistParameter_ShouldReturnFalse_WhenParameterDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();

        var exists = _simMethodDataAccess.ExistParameter(nonExistentId);

        Assert.IsFalse(exists);
    }

    [TestMethod]
    public void MethodVariableRepeatedValues_ReturnsTrue_WhenVariableExists()
    {
        var typeId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var localVariable = new LocalVariable()
        {
            Id = Guid.NewGuid(),
            TypeId = typeId,
            Name = "TestLocalVariable",
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "TestType" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };
        var localVariableRepeated = new LocalVariable()
        {
            Id = Guid.NewGuid(),
            TypeId = typeId,
            Name = "TestLocalVariable",
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "TestType" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };
        _context.LocalVariables.Add(localVariable);
        _context.SaveChanges();

        var result = _simMethodDataAccess.MethodVariableRepeatedValues(methodId, localVariableRepeated);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void MethodParameterRepeatedValues_ReturnsTrue_WhenParameterExists()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter { Name = "param1", RelatedMethodId = methodId };
        _context.Parameters.Add(parameter);
        _context.SaveChanges();

        var result = _simMethodDataAccess.MethodParameterRepeatedValues(methodId, new Parameter { Name = "param1" });

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetVariableById_ReturnsVariable_WhenExists()
    {
        var variableId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var localVariable = new LocalVariable()
        {
            Id = variableId,
            TypeId = typeId,
            Name = "TestLocalVariable",
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "TestType" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };
        _context.LocalVariables.Add(localVariable);
        _context.SaveChanges();

        var result = _simMethodDataAccess.GetVariableById(variableId);

        Assert.IsNotNull(result);
        Assert.AreEqual(variableId, result.Id);
        Assert.AreEqual("TestLocalVariable", result.Name);
    }

    [TestMethod]
    public void GetParameterById_ReturnsParameter_WhenExists()
    {
        var parameterId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = parameterId,
            Name = "TestParameter",
            RelatedMethodId = methodId
        };
        _context.Parameters.Add(parameter);
        _context.SaveChanges();

        var result = _simMethodDataAccess.GetParameterById(parameterId);

        Assert.IsNotNull(result);
        Assert.AreEqual(parameterId, result.Id);
        Assert.AreEqual("TestParameter", result.Name);
    }

    [TestMethod]
    public void GetMethodById_ReturnsMethod_WhenExists()
    {
        var methodId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();
        var simMethod = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = relatedClassId,
            RelatedClass = new SimClass() { Id = relatedClassId, Name = "TestClass" }
        };
        _context.SimMethods.Add(simMethod);
        _context.SaveChanges();

        var result = _simMethodDataAccess.GetMethodById(methodId);

        Assert.IsNotNull(result);
        Assert.AreEqual(methodId, result.Id);
        Assert.AreEqual("TestMethod", result.Name);
    }

    [TestMethod]
    public void GetInvocationById_ReturnsInvocation_WhenExists()
    {
        var invocationId = Guid.NewGuid();
        var signature = new Signature()
        {
            Name = "TestInvocation"
        };
        var invocation = new Invocation
        {
            Id = invocationId,
            Signature = signature
        };
        _context.Invocations.Add(invocation);
        _context.SaveChanges();

        var result = _simMethodDataAccess.GetInvocationById(invocationId);

        Assert.IsNotNull(result);
        Assert.AreEqual(invocationId, result.Id);
        Assert.AreEqual("TestInvocation", result.Signature.Name);
    }

    [TestMethod]
    public void ExistVariableById_ReturnsTrue_WhenVariableExists()
    {
        var variableId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var localVariable = new LocalVariable()
        {
            Id = variableId,
            TypeId = typeId,
            Name = "TestLocalVariable",
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "TestType" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };
        _context.LocalVariables.Add(localVariable);
        _context.SaveChanges();

        var result = _simMethodDataAccess.ExistVariableById(variableId);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ExistVariableById_ReturnsFalse_WhenVariableDoesNotExist()
    {
        var variableId = Guid.NewGuid();

        var result = _simMethodDataAccess.ExistVariableById(variableId);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ExistsMethodInClass_ReturnsTrue_WhenMethodWithSameSignatureExists()
    {
        var classId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = classId,
            Parameters =
        [
            new Parameter { Id = Guid.NewGuid(), Name = "param1", TypeId = typeId },
            new Parameter { Id = Guid.NewGuid(), Name = "param2", TypeId = typeId },
            new Parameter { Id = Guid.NewGuid(), Name = "param3", TypeId = typeId },
        ]
        };

        _context.SimMethods.Add(method);
        _context.Parameters.AddRange(method.Parameters);
        _context.SaveChanges();

        var methodToCheck = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
        [
            new Parameter { Id = Guid.NewGuid(), Name = "param1", TypeId = typeId },
            new Parameter { Id = Guid.NewGuid(), Name = "param2", TypeId = typeId },
            new Parameter { Id = Guid.NewGuid(), Name = "param3", TypeId = typeId },
        ]
        };

        var result = _simMethodDataAccess.ExistsMethodInClass(classId, methodToCheck);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ExistsMethodInClass_ShortCircuitEvaluation_WhenMultipleMethodsMatch()
    {
        var classId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var method1 = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = classId,
            Parameters =
            [
                new Parameter { Id = Guid.NewGuid(), Name = "param1", TypeId = typeId }
            ]
        };

        var method2 = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = classId,
            Parameters =
            [
                new Parameter { Id = Guid.NewGuid(), Name = "param1", TypeId = typeId }
            ]
        };

        _context.SimMethods.Add(method1);
        _context.SimMethods.Add(method2);
        _context.Parameters.AddRange(method1.Parameters);
        _context.Parameters.AddRange(method2.Parameters);
        _context.SaveChanges();

        var methodToCheck = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Id = Guid.NewGuid(), Name = "param1", TypeId = typeId }
            ]
        };

        var result = _simMethodDataAccess.ExistsMethodInClass(classId, methodToCheck);

        Assert.IsTrue(result);
    }
}
