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
        var databaseName = $"TestDB_{Guid.NewGuid()}";

        var options = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        _context = new SimulatorDbContext(options);
        _simMethodDataAccess = new SimMethodDataAccess(_context);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
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
    public void AddMethodParameter_ShouldAssignCorrectIndices()
    {
        var classId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var type1Id = Guid.NewGuid();
        var type2Id = Guid.NewGuid();
        var type3Id = Guid.NewGuid();

        var simClass = new SimClass
        {
            Id = classId,
            Name = "TestClass"
        };
        _context.SimClasses.Add(simClass);

        var type1 = new SimClass { Id = type1Id, Name = "IntType" };
        var type2 = new SimClass { Id = type2Id, Name = "StringType" };
        var type3 = new SimClass { Id = type3Id, Name = "BoolType" };
        _context.SimClasses.Add(type1);
        _context.SimClasses.Add(type2);
        _context.SimClasses.Add(type3);

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId,
            RelatedClass = simClass,
            Parameters = []
        };
        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var param1 = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            TypeId = type1Id
        };

        var param2 = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param2",
            TypeId = type2Id
        };

        var param3 = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param3",
            TypeId = type3Id
        };

        var result1 = _simMethodDataAccess.AddMethodParameter(methodId, param1);

        var updatedMethod = _context.SimMethods
            .Include(m => m.Parameters)
            .ThenInclude(p => p.Type)
            .FirstOrDefault(m => m.Id == methodId);
        var paramsOrdered = updatedMethod.Parameters.OrderBy(p => p.Index).ToList();

        Assert.AreEqual(0, result1.Index, "Primer parámetro debe tener índice 0");
        Assert.IsNotNull(updatedMethod, "El método debe existir en la base de datos");
        Assert.AreEqual("param1", paramsOrdered[0].Name, "El primer parámetro debe ser param1");
        Assert.AreEqual(type1Id, paramsOrdered[0].TypeId, "El primer parámetro debe ser de tipo IntType");
    }

    [TestMethod]
    public void CreateInvocation_ShouldAddInvocationToDatabase()
    {
        var classId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();

        var simClass = new SimClass
        {
            Id = classId,
            Name = "TestClass"
        };
        _context.SimClasses.Add(simClass);

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId,
            RelatedClass = simClass,
            Invocations = []
        };
        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var reference = new ReferenceThis
        {
            Id = Guid.NewGuid(),
            Reference = simClass,
            ReferenceId = classId,
        };
        var signature = new Signature
        {
            Id = Guid.NewGuid(),
            Name = "TestSignature",
            Parameters = []
        };
        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Reference = reference,
            Signature = signature,
            RelatedMethodId = methodId
        };

        var result = _simMethodDataAccess.CreateInvocation(methodId, invocation);

        var invocationInDb = _context.Invocations
            .Include(i => i.Reference)
            .Include(i => i.Signature)
            .FirstOrDefault(i => i.Id == invocation.Id);

        Assert.IsNotNull(invocationInDb);
        Assert.AreEqual(invocation.Id, result.Id);
        Assert.AreEqual(methodId, invocationInDb.RelatedMethodId);
        Assert.AreEqual(0, invocationInDb.Index);
        Assert.IsNotNull(invocationInDb.Reference);
        Assert.IsNotNull(invocationInDb.Signature);
        Assert.AreEqual("TestSignature", invocationInDb.Signature.Name);

        var updatedMethod = _context.SimMethods
            .Include(m => m.Invocations)
            .FirstOrDefault(m => m.Id == methodId);

        Assert.IsNotNull(updatedMethod);
        Assert.AreEqual(2, updatedMethod.Invocations.Count);
        Assert.AreEqual(invocation.Id, updatedMethod.Invocations.First().Id);
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

    [TestMethod]
    public void InUseByOther_WithNullReference_HandlesCorrectly()
    {
        var dbContextOptions = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
            .Options;

        using var context = new SimulatorDbContext(dbContextOptions);
        var testId = Guid.NewGuid();

        var nullReference = new ReferenceThis
        {
            Id = Guid.NewGuid(),
            Reference = null
        };

        context.References.Add(nullReference);
        context.SaveChanges();

        var dataAccess = new SimClassDataAccess(context);

        var result = dataAccess.InUseByOther(testId);

        Assert.IsFalse(result, "Cuando Reference es null, no debería considerarse en uso");
    }

    [TestMethod]
    public void GetMethodById_ShouldOrderSignatureParametersAndLoadReferences()
    {
        var methodId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        _context.SimClasses.Add(typeClass);

        var simClass = new SimClass { Id = classId, Name = "TestClass" };
        _context.SimClasses.Add(simClass);

        var param = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param",
            Index = 0,
            RelatedMethodId = methodId,
            TypeId = typeId,
            Type = typeClass
        };
        _context.Parameters.Add(param);

        var localVar = new LocalVariable
        {
            Id = Guid.NewGuid(),
            Name = "localVar",
            RelatedMethodId = methodId,
            TypeId = typeId,
            Type = typeClass
        };
        _context.LocalVariables.Add(localVar);

        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "testAttr",
            RelatedClassId = classId,
            TypeId = typeId,
            Type = typeClass
        };
        _context.SimAttributes.Add(attribute);

        var signature = new Signature { Id = Guid.NewGuid(), Name = "TestSignature" };
        _context.Signatures.Add(signature);

        var sigParam1 = new ParameterSignature
        {
            Id = Guid.NewGuid(),
            Name = "sigParam1",
            Index = 2,
            SignatureId = signature.Id,
            TypeId = typeId,
            Type = typeClass
        };
        var sigParam2 = new ParameterSignature
        {
            Id = Guid.NewGuid(),
            Name = "sigParam2",
            Index = 0,
            SignatureId = signature.Id,
            TypeId = typeId,
            Type = typeClass
        };
        var sigParam3 = new ParameterSignature
        {
            Id = Guid.NewGuid(),
            Name = "sigParam3",
            Index = 1,
            SignatureId = signature.Id,
            TypeId = typeId,
            Type = typeClass
        };

        _context.ParameterSignatures.AddRange(sigParam1, sigParam2, sigParam3);
        signature.Parameters = [sigParam1, sigParam2, sigParam3];

        var refParam = new ReferenceParameter { Id = Guid.NewGuid(), ReferenceId = param.Id, Reference = param };
        var refVar = new ReferenceVariable { Id = Guid.NewGuid(), ReferenceId = localVar.Id, Reference = localVar };
        var refAttr = new ReferenceAttribute { Id = Guid.NewGuid(), ReferenceId = attribute.Id, Reference = attribute };
        var refBase = new ReferenceBase { Id = Guid.NewGuid(), ReferenceId = classId, Reference = simClass };
        var refThis = new ReferenceThis { Id = Guid.NewGuid(), ReferenceId = classId, Reference = simClass };

        _context.References.AddRange(refParam, refVar, refAttr, refBase, refThis);

        var inv1 = new Invocation
        {
            Id = Guid.NewGuid(),
            Index = 0,
            RelatedMethodId = methodId,
            Reference = refParam
        };
        var inv2 = new Invocation
        {
            Id = Guid.NewGuid(),
            Index = 1,
            RelatedMethodId = methodId,
            Reference = refVar
        };
        var inv3 = new Invocation
        {
            Id = Guid.NewGuid(),
            Index = 2,
            RelatedMethodId = methodId,
            Reference = refAttr
        };
        var inv4 = new Invocation
        {
            Id = Guid.NewGuid(),
            Index = 3,
            RelatedMethodId = methodId,
            Reference = refBase
        };
        var inv5 = new Invocation
        {
            Id = Guid.NewGuid(),
            Index = 4,
            RelatedMethodId = methodId,
            Reference = refThis,
            Signature = signature
        };

        _context.Invocations.AddRange(inv1, inv2, inv3, inv4, inv5);

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId,
            RelatedClass = simClass,
            Parameters = [param],
            Invocations = [inv1, inv2, inv3, inv4, inv5]
        };

        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var result = _simMethodDataAccess.GetMethodById(methodId);

        Assert.IsNotNull(result);
        Assert.AreEqual(methodId, result.Id);
        Assert.AreEqual("TestMethod", result.Name);
        Assert.AreEqual(5, result.Invocations.Count);
    }

    [TestMethod]
    public void MethodIsInUse_ReturnsTrue_WhenMethodHasLocalVariables()
    {
        var methodId = Guid.NewGuid();
        var method = new SimMethod
        {
            Id = methodId,
            Name = "MethodWithLocalVariables"
        };
        _context.SimMethods.Add(method);

        var localVariable = new LocalVariable
        {
            Id = Guid.NewGuid(),
            Name = "TestLocalVariable",
            RelatedMethodId = methodId
        };
        _context.LocalVariables.Add(localVariable);
        _context.SaveChanges();

        var result = _simMethodDataAccess.MethodIsInUse(methodId);

        Assert.IsTrue(result, "Method with local variables should be considered in use");
    }

    [TestMethod]
    public void GetInvocationById_WithReferenceParameter_LoadsCorrectly()
    {
        var methodId = Guid.NewGuid();
        var paramId = Guid.NewGuid();
        var invocationId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        _context!.SimClasses.Add(typeClass);
        _context.SaveChanges();

        var parameter = new Parameter
        {
            Id = paramId,
            Name = "TestParam",
            TypeId = typeId,
            Type = typeClass
        };
        _context.Parameters.Add(parameter);

        var referenceParameter = new ReferenceParameter
        {
            Id = Guid.NewGuid(),
            Reference = parameter
        };

        var signature = new Signature
        {
            Id = Guid.NewGuid(),
            Name = "TestSignature",
            Parameters = []
        };

        var invocation = new Invocation
        {
            Id = invocationId,
            RelatedMethodId = methodId,
            Reference = referenceParameter,
            Signature = signature
        };
        _context.Invocations.Add(invocation);
        _context.SaveChanges();

        var result = _simMethodDataAccess!.GetInvocationById(invocationId);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Reference, typeof(ReferenceParameter));
        var refParam = result.Reference as ReferenceParameter;
        Assert.IsNotNull(refParam!.Reference);
        Assert.AreEqual(parameter.Name, refParam.Reference.Name);
        Assert.IsNotNull(refParam.Reference.Type);
        Assert.AreEqual("TypeClass", refParam.Reference.Type.Name);
    }

    [TestMethod]
    public void GetInvocationById_WithReferenceAttribute_LoadsCorrectly()
    {
        var methodId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var invocationId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        _context!.SimClasses.Add(typeClass);

        var containingClass = new SimClass { Id = classId, Name = "ContainingClass" };
        _context.SimClasses.Add(containingClass);
        _context.SaveChanges();

        var attribute = new SimAttribute
        {
            Id = attributeId,
            Name = "TestAttr",
            TypeId = typeId,
            Type = typeClass,
            RelatedClassId = classId,
            RelatedClass = containingClass
        };
        _context.SimAttributes.Add(attribute);

        var referenceAttribute = new ReferenceAttribute
        {
            Id = Guid.NewGuid(),
            Reference = attribute
        };

        var signature = new Signature
        {
            Id = Guid.NewGuid(),
            Name = "TestSignature"
        };

        var invocation = new Invocation
        {
            Id = invocationId,
            RelatedMethodId = methodId,
            Reference = referenceAttribute,
            Signature = signature
        };
        _context.Invocations.Add(invocation);
        _context.SaveChanges();

        var result = _simMethodDataAccess!.GetInvocationById(invocationId);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Reference, typeof(ReferenceAttribute));
        var refAttr = result.Reference as ReferenceAttribute;
        Assert.IsNotNull(refAttr!.Reference);
        Assert.AreEqual(attribute.Name, refAttr.Reference.Name);
        Assert.IsNotNull(refAttr.Reference.Type);
        Assert.AreEqual("TypeClass", refAttr.Reference.Type.Name);
    }

    [TestMethod]
    public void GetInvocationById_WithReferenceBase_LoadsCorrectly()
    {
        var methodId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();
        var invocationId = Guid.NewGuid();

        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass" };
        _context!.SimClasses.Add(baseClass);

        var simClass = new SimClass
        {
            Id = classId,
            Name = "ChildClass",
            BaseClassId = baseClassId,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var referenceBase = new ReferenceBase
        {
            Id = Guid.NewGuid(),
            Reference = simClass
        };

        var signature = new Signature
        {
            Id = Guid.NewGuid(),
            Name = "TestSignature"
        };

        var invocation = new Invocation
        {
            Id = invocationId,
            RelatedMethodId = methodId,
            Reference = referenceBase,
            Signature = signature
        };
        _context.Invocations.Add(invocation);
        _context.SaveChanges();

        var result = _simMethodDataAccess!.GetInvocationById(invocationId);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Reference, typeof(ReferenceBase));
        var refBase = result.Reference as ReferenceBase;
        Assert.IsNotNull(refBase!.Reference);
        Assert.AreEqual(simClass.Name, refBase.Reference.Name);
        Assert.IsNotNull(refBase.Reference.BaseClass);
        Assert.AreEqual("BaseClass", refBase.Reference.BaseClass.Name);
    }

    [TestMethod]
    public void GetInvocationById_WithReferenceThis_LoadsCorrectly()
    {
        var methodId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var invocationId = Guid.NewGuid();

        var simClass = new SimClass { Id = classId, Name = "ThisClass" };
        _context!.SimClasses.Add(simClass);
        _context.SaveChanges();

        var referenceThis = new ReferenceThis
        {
            Id = Guid.NewGuid(),
            Reference = simClass
        };

        var signature = new Signature
        {
            Id = Guid.NewGuid(),
            Name = "TestSignature"
        };

        var invocation = new Invocation
        {
            Id = invocationId,
            RelatedMethodId = methodId,
            Reference = referenceThis,
            Signature = signature
        };
        _context.Invocations.Add(invocation);
        _context.SaveChanges();

        var result = _simMethodDataAccess!.GetInvocationById(invocationId);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Reference, typeof(ReferenceThis));
        var refThis = result.Reference as ReferenceThis;
        Assert.IsNotNull(refThis!.Reference);
        Assert.AreEqual(simClass.Name, refThis.Reference.Name);
    }
}
