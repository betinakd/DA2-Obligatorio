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
    public void TestFindMethodInHierarchyPublicOrProtected_ThroughPublicMethods()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        var typeId = Guid.NewGuid();
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
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
            Accesibility = SimAccesibility.Sealed,
            ReturnTypeId = typeId,
        };

        var parameter = new Parameter
        {
            Name = "param1",
            ReferenceId = intType.Id,
            Reference = intType,
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod,
        };
        baseMethod.Parameters.Add(parameter);

        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var signature = new Signature { Name = "TestMethod", ReturnTypeId = typeId };
        var paramSignature = new ParameterSignature
        {
            Name = "param1",
            ReferenceId = intType.Id,
            Reference = intType,
            SignatureId = signature.Id,
        };
        signature.Parameters.Add(paramSignature);

        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(childClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
    }

    [TestMethod]
    public void TestFindMethodInHierarchyPublicOrProtected_ReturnsNull_WhenSimClassIsNull()
    {
        var signature = new Signature { Name = "AnyMethod" };
        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(null, signature);
        result.Should().BeNull();
    }

    [TestMethod]
    public void TestFindMethodInHierarchyPublicOrProtected_ReturnsNull_WhenBaseClassNotFound()
    {
        var simClass = new SimClass { Name = "Child", BaseClassId = Guid.NewGuid() };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var signature = new Signature { Name = "AnyMethod" };
        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(simClass, signature);
        result.Should().BeNull();
    }

    [TestMethod]
    public void GetAllInheritingClasses_ReturnsDirectInheritors()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass1 = new SimClass
        {
            Name = "ChildClass1",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        var childClass2 = new SimClass
        {
            Name = "ChildClass2",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.AddRange(childClass1, childClass2);
        _context.SaveChanges();

        var result = _executionDataAccess.GetAllInheritingClasses(baseClass.Id);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "ChildClass1");
        result.Should().Contain(c => c.Name == "ChildClass2");
    }

    [TestMethod]
    public void GetAllInheritingClasses_ReturnsIndirectInheritors()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var grandChildClass = new SimClass
        {
            Name = "GrandChildClass",
            BaseClassId = childClass.Id,
            BaseClass = childClass,
        };
        _context.SimClasses.Add(grandChildClass);
        _context.SaveChanges();

        var result = _executionDataAccess.GetAllInheritingClasses(baseClass.Id);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "ChildClass");
        result.Should().Contain(c => c.Name == "GrandChildClass");
    }

    [TestMethod]
    public void GetAllInheritingClasses_ReturnsEmpty_WhenNoInheritors()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var result = _executionDataAccess.GetAllInheritingClasses(baseClass.Id);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void GetAllInheritingClasses_ReturnsEmpty_WhenBaseClassDoesNotExist()
    {
        var nonExistentBaseClassId = Guid.NewGuid();

        var result = _executionDataAccess.GetAllInheritingClasses(nonExistentBaseClassId);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsFalse_WhenMethodIsNotUsedInInvocation()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var childMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [],
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeFalse();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsTrue_WhenMethodIsUsedInInvocation()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        var typeId = Guid.NewGuid();
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            ReturnTypeId = typeId,
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Protected,
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature { ReturnTypeId = typeId, Id = Guid.NewGuid(), Name = "TestMethod" },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod,
        };

        var childMethod = new SimMethod
        {
            ReturnTypeId = typeId,
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [invocation],
            Privacity = SimPrivacity.Public,
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeTrue();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsFalse_WhenMethodInInheritingIsPrivate()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Private,
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature { Id = Guid.NewGuid(), Name = "TestMethod" },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod,
        };

        var childMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [invocation],
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeFalse();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsTrue_WhenMethodInInheritingIsProtected()
    {
        var typeId = Guid.NewGuid();
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            ReturnTypeId = typeId,
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Protected,
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature
            {
                ReturnTypeId = typeId,
                Id = Guid.NewGuid(),
                Name = "TestMethod"
            },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod,
        };

        var childMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [invocation],
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeTrue();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsTrue_WhenMethodInInheritingIsPublic()
    {
        var typeId = Guid.NewGuid();
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            ReturnTypeId = typeId,
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Public,
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature { ReturnTypeId = typeId, Id = Guid.NewGuid(), Name = "TestMethod" },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod,
        };

        var childMethod = new SimMethod
        {
            ReturnTypeId = typeId,
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [invocation],
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeTrue();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsTrue_WhenOwnerClassMethodIsPrivate()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(returnType);
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Private,
            ReturnTypeId = typeId,
            ReturnType = returnType
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature
            {
                Id = Guid.NewGuid(),
                Name = "TestMethod",
                ReturnTypeId = typeId,
                ReturnType = returnType
            },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod,
        };

        var ownerMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "OwnerMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Private,
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Invocations = [invocation],
        };
        baseClass.Methods.Add(ownerMethod);
        _context.SimMethods.Add(ownerMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeTrue();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsTrue_WhenOwnerClassMethodIsPublic()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(returnType);
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Public,
            ReturnTypeId = typeId,
            ReturnType = returnType
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature
            {
                Id = Guid.NewGuid(),
                Name = "TestMethod",
                ReturnTypeId = typeId,
                ReturnType = returnType
            },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod,
        };

        var ownerMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "OwnerMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Public,
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Invocations = [invocation],
        };
        baseClass.Methods.Add(ownerMethod);
        _context.SimMethods.Add(ownerMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeTrue("Public methods in owner class should be considered in use by inheriting");
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsFalse_WhenMethodInInheritingIsProtectedAndNoMatchesSignature()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Protected,
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature { Id = Guid.NewGuid(), Name = "TestMethodnotMatches" },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod,
        };

        var childMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [invocation],
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeFalse();
    }

    [TestMethod]
    public void SaveExecutionLog_ShouldSaveLogToDatabase()
    {
        var executionLog = new ExecutionLog
        {
            Id = Guid.NewGuid(),
            Execution = "Test execution",
            Reference = "Test reference",
            ObjectCreate = "Test object creation",
        };

        _executionDataAccess!.SaveExecutionLog(executionLog);

        var savedLog = _context!.ExecutionLogs.FirstOrDefault(l => l.Id == executionLog.Id);
        savedLog.Should().NotBeNull("The execution log should be saved to the database");
        savedLog!.Execution.Should().Be("Test execution");
        savedLog.Reference.Should().Be("Test reference");
        savedLog.ObjectCreate.Should().Be("Test object creation");
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtected_WithReferenceParameter_LoadsParameterTypesCorrectly()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(returnType);
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var intType = new SimClass { Name = "int" };
        _context.SimClasses.Add(intType);
        _context.SaveChanges();

        var methodParameter = new Parameter
        {
            Name = "methodParam",
            ReferenceId = intType.Id,
            Reference = intType,
            Index = 1,
        };

        var method = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Parameters = [methodParameter],
        };
        methodParameter.RelatedMethod = method;
        methodParameter.RelatedMethodId = method.Id;

        var invocationParameter = new Parameter
        {
            Name = "invocationParam",
            ReferenceId = intType.Id,
            Reference = intType,
            Index = 0,
        };

        var referenceParameter = new ReferenceParameter
        {
            Reference = invocationParameter,
        };

        var invocationSignature = new Signature
        {
            Name = "InvokedMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Parameters = [],
        };

        var invocation = new Invocation
        {
            Reference = referenceParameter,
            Signature = invocationSignature,
            Index = 0,
        };
        method.Invocations.Add(invocation);

        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "TestMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Parameters = [new ParameterSignature { Name = "methodParam", ReferenceId = intType.Id, Reference = intType }]
        };
        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(simClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Invocations.Should().HaveCount(1);
        result.Invocations[0].Reference.Should().BeOfType<ReferenceParameter>();
        var refParam = result.Invocations[0].Reference as ReferenceParameter;
        refParam.Reference.Should().NotBeNull();
        refParam.Reference.Reference.Should().NotBeNull();
        refParam.Reference.Reference.Name.Should().Be("int");
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtected_WithReferenceVariable_LoadsVariableTypesCorrectly()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(returnType);
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var stringType = new SimClass { Name = "string" };
        _context.SimClasses.Add(stringType);
        _context.SaveChanges();

        var variable = new LocalVariable
        {
            Name = "testVar",
            ReferenceId = stringType.Id,
            Reference = stringType,
        };
        _context.LocalVariables.Add(variable);
        _context.SaveChanges();

        var referenceVariable = new ReferenceVariable
        {
            Reference = variable,
        };

        var invocationSignature = new Signature
        {
            Name = "InvokedMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Parameters = [],
        };

        var invocation = new Invocation
        {
            Reference = referenceVariable,
            Signature = invocationSignature,
            Index = 0,
        };

        var method = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Invocations = [invocation],
        };

        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "TestMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Parameters = []
        };
        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(simClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Invocations.Should().HaveCount(1);
        result.Invocations[0].Reference.Should().BeOfType<ReferenceVariable>();
        var refVar = result.Invocations[0].Reference as ReferenceVariable;
        refVar.Reference.Should().NotBeNull();
        refVar.Reference.Reference.Should().NotBeNull();
        refVar.Reference.Reference.Name.Should().Be("string");
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtected_WithReferenceAttribute_LoadsAttributeTypesCorrectly()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(returnType);
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var boolType = new SimClass { Name = "bool" };
        _context.SimClasses.Add(boolType);
        _context.SaveChanges();

        var attribute = new SimAttribute
        {
            Name = "testAttr",
            ReferenceId = boolType.Id,
            Reference = boolType,
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
        };
        _context.SimAttributes.Add(attribute);
        _context.SaveChanges();

        var referenceAttribute = new ReferenceAttribute
        {
            Reference = attribute,
        };

        var invocationSignature = new Signature
        {
            Name = "InvokedMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Parameters = [],
        };

        var invocation = new Invocation
        {
            Reference = referenceAttribute,
            Signature = invocationSignature,
            Index = 0,
        };

        var method = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Invocations = [invocation],
        };

        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "TestMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Parameters = []
        };
        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(simClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Invocations.Should().HaveCount(1);
        result.Invocations[0].Reference.Should().BeOfType<ReferenceAttribute>();
        var refAttr = result.Invocations[0].Reference as ReferenceAttribute;
        refAttr.Reference.Should().NotBeNull();
        refAttr.Reference.Reference.Should().NotBeNull();
        refAttr.Reference.Reference.Name.Should().Be("bool");
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtected_WithReferenceBase_LoadsBaseClassCorrectly()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(returnType);
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var referenceBase = new ReferenceBase
        {
            Reference = childClass,
        };

        var invocationSignature = new Signature
        {
            Name = "InvokedMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Parameters = [],
        };

        var invocation = new Invocation
        {
            Reference = referenceBase,
            Signature = invocationSignature,
            Index = 0,
        };

        var method = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Invocations = [invocation],
        };

        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "TestMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Parameters = []
        };
        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(childClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Invocations.Should().HaveCount(1);
        result.Invocations[0].Reference.Should().BeOfType<ReferenceBase>();
        var refBase = result.Invocations[0].Reference as ReferenceBase;
        refBase.Reference.Should().NotBeNull();
        refBase.Reference.BaseClass.Should().NotBeNull();
        refBase.Reference.BaseClass.Name.Should().Be("BaseClass");
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtected_WithReferenceThis_LoadsThisReferenceCorrectly()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(returnType);
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var referenceThis = new ReferenceThis
        {
            Reference = simClass,
        };

        var invocationSignature = new Signature
        {
            Name = "InvokedMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Parameters = [],
        };

        var invocation = new Invocation
        {
            Reference = referenceThis,
            Signature = invocationSignature,
            Index = 0,
        };

        var method = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Invocations = [invocation],
        };

        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "TestMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType,
            Parameters = []
        };
        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(simClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Invocations.Should().HaveCount(1);
        result.Invocations[0].Reference.Should().BeOfType<ReferenceThis>();
        var refThis = result.Invocations[0].Reference as ReferenceThis;
        refThis.Reference.Should().NotBeNull();
        refThis.Reference.Name.Should().Be("TestClass");
    }

    [TestMethod]
    public void IsMethodUsedInClass_WithNullClass_ReturnsFalse()
    {
        var method = new SimMethod { Id = Guid.NewGuid(), Name = "TestMethod" };

        var methodInfo = typeof(ExecutionDataAccess).GetMethod("IsMethodUsedInClass",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var result = (bool)methodInfo.Invoke(_executionDataAccess, [null, method]);

        result.Should().BeFalse("IsMethodUsedInClass should return false for null class");
    }

    [TestMethod]
    public void IsMethodUsedInInheritingClass_WithNullClass_ReturnsFalse()
    {
        var method = new SimMethod { Id = Guid.NewGuid(), Name = "TestMethod" };

        var methodInfo = typeof(ExecutionDataAccess).GetMethod("IsMethodUsedInInheritingClass",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var result = (bool)methodInfo.Invoke(_executionDataAccess, [null, method]);

        result.Should().BeFalse("IsMethodUsedInInheritingClass should return false for null class");
    }

    [TestMethod]
    public void LoadReferenceDetails_WithNullInvocation_DoesNotThrowException()
    {
        var methodInfo = typeof(ExecutionDataAccess).GetMethod("LoadReferenceDetails",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        methodInfo.Invoke(_executionDataAccess, [null]);
    }

    [TestMethod]
    public void OrderMethodsInClass_WithNullClass_DoesNotThrowException()
    {
        var methodInfo = typeof(ExecutionDataAccess).GetMethod("OrderMethodsInClass",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        methodInfo.Invoke(_executionDataAccess, [null]);
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtectedPublicOrProtected_ReturnsNull_WhenSimClassIsNull()
    {
        var signature = new Signature { Name = "AnyMethod" };
        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(null, signature);
        result.Should().BeNull();
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtectedPublicOrProtected_FindsAnyMethod_InOriginalClass()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        // ...existing code...
        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(returnType);
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var privateMethod = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            Privacity = SimPrivacity.Private,
            ReturnTypeId = typeId,
            ReturnType = returnType,
        };
        simClass.Methods.Add(privateMethod);
        _context.SimMethods.Add(privateMethod);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters = [],
            ReturnTypeId = typeId,
            ReturnType = returnType
        };

        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(simClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Privacity.Should().Be(SimPrivacity.Private);
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtectedPublicOrProtected_FindsPublicMethod_InBaseClass()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        // ...existing code...
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(returnType);
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var publicMethod = new SimMethod
        {
            Name = "PublicMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Public,
            ReturnTypeId = typeId,
            ReturnType = returnType,
        };
        baseClass.Methods.Add(publicMethod);
        _context.SimMethods.Add(publicMethod);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "PublicMethod",
            Parameters = [],
            ReturnTypeId = typeId,
            ReturnType = returnType
        };

        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(childClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("PublicMethod");
        result.Privacity.Should().Be(SimPrivacity.Public);
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtectedPublicOrProtected_FindsProtectedMethod_InBaseClass()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(returnType);
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var protectedMethod = new SimMethod
        {
            Name = "ProtectedMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Protected,
            ReturnTypeId = typeId,
            ReturnType = returnType,
        };
        baseClass.Methods.Add(protectedMethod);
        _context.SimMethods.Add(protectedMethod);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "ProtectedMethod",
            Parameters = [],
            ReturnTypeId = typeId,
            ReturnType = returnType
        };

        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(childClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("ProtectedMethod");
        result.Privacity.Should().Be(SimPrivacity.Protected);
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtectedPublicOrProtected_DoesNotFindPrivateMethod_InBaseClass()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var privateMethod = new SimMethod
        {
            Name = "PrivateMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Private,
        };
        baseClass.Methods.Add(privateMethod);
        _context.SimMethods.Add(privateMethod);
        _context.SaveChanges();

        var signature = new Signature { Name = "PrivateMethod", Parameters = [] };

        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(childClass, signature);

        result.Should().BeNull("Private methods in base classes should not be accessible");
    }

    [TestMethod]
    public void CanOverride_ReturnsFalse_WhenMethodToOverrideIsNull()
    {
        var classId = Guid.NewGuid();

        var result = _executionDataAccess.CanOverrideFromBaseClass(classId, null);

        result.Should().BeFalse("Method should not be overridable when method parameter is null");
    }

    [TestMethod]
    public void CanOverride_ReturnsFalse_WhenClassDoesNotExist()
    {
        var nonExistentClassId = Guid.NewGuid();
        var method = new SimMethod
        {
            Name = "TestMethod",
            IsVirtual = true,
            IsOverride = true,
            Privacity = SimPrivacity.Public,
        };

        var result = _executionDataAccess.CanOverrideFromBaseClass(nonExistentClassId, method);

        result.Should().BeFalse("Method should not be overridable when class doesn't exist");
    }

    [TestMethod]
    public void CanOverride_ReturnsFalse_WhenClassHasNoBaseClass()
    {
        var simClass = new SimClass { Name = "ClassWithoutBase" };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var method = new SimMethod
        {
            Name = "TestMethod",
            IsVirtual = true,
            IsOverride = true,
        };

        var result = _executionDataAccess.CanOverrideFromBaseClass(simClass.Id, method);

        result.Should().BeFalse("Method should not be overridable when class has no base class");
    }

    [TestMethod]
    public void ReturnsOverrideMethod_FromInstanceHierarchy()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var baseClass = new SimClass { Name = "Base" };
        var childClass = new SimClass { Name = "Child", BaseClass = baseClass };
        _context.SimClasses.AddRange(returnType, baseClass, childClass);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "TestMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType
        };

        var overrideMethod = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            IsOverride = true,
            ReturnTypeId = typeId,
            ReturnType = returnType
        };
        _context.SimMethods.Add(overrideMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.FindOverrideOrReferenceMethod(childClass, baseClass, signature);

        result.Should().NotBeNull();
        result.Id.Should().Be(overrideMethod.Id);
    }

    [TestMethod]
    public void ReturnsReferenceMethod_WhenNoOverrideFound()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var baseClass = new SimClass { Name = "Base" };
        var childClass = new SimClass { Name = "Child", BaseClass = baseClass };
        _context.SimClasses.AddRange(returnType, baseClass, childClass);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "TestMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType
        };

        var referenceMethod = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            IsOverride = false,
            ReturnTypeId = typeId,
            ReturnType = returnType
        };
        _context.SimMethods.Add(referenceMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.FindOverrideOrReferenceMethod(childClass, baseClass, signature);

        result.Should().NotBeNull();
        result.Id.Should().Be(referenceMethod.Id);
    }

    [TestMethod]
    public void ReturnsNull_WhenNoMethodFoundAnywhere()
    {
        var baseClass = new SimClass { Name = "Base" };
        var childClass = new SimClass { Name = "Child", BaseClass = baseClass };
        _context.SimClasses.AddRange(baseClass, childClass);
        _context.SaveChanges();

        var signature = new Signature { Name = "NonExistentMethod" };

        var result = _executionDataAccess.FindOverrideOrReferenceMethod(childClass, baseClass, signature);

        result.Should().BeNull();
    }

    [TestMethod]
    public void ReturnsOverrideMethod_FromIntermediateBaseClass()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var grandparent = new SimClass { Name = "Grandparent" };
        var parent = new SimClass { Name = "Parent", BaseClass = grandparent };
        var child = new SimClass { Name = "Child", BaseClass = parent };
        _context.SimClasses.AddRange(returnType, grandparent, parent, child);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "TestMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType
        };

        var overrideMethod = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = parent.Id,
            RelatedClass = parent,
            IsOverride = true,
            ReturnTypeId = typeId,
            ReturnType = returnType
        };
        _context.SimMethods.Add(overrideMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.FindOverrideOrReferenceMethod(child, grandparent, signature);

        result.Should().NotBeNull();
        result.Id.Should().Be(overrideMethod.Id);
    }

    [TestMethod]
    public void FindOverrideOrReferenceMethod_BreaksWhenNoBaseClassId()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var instanceClass = new SimClass { Name = "InstanceClass", BaseClass = null, BaseClassId = null };
        var referenceClass = new SimClass { Name = "ReferenceClass", BaseClass = null, BaseClassId = null };
        _context.SimClasses.AddRange(returnType, instanceClass, referenceClass);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "TestMethod",
            ReturnTypeId = typeId,
            ReturnType = returnType
        };

        var referenceMethod = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = referenceClass.Id,
            RelatedClass = referenceClass,
            IsOverride = false,
            IsVirtual = true,
            ReturnTypeId = typeId,
            ReturnType = returnType
        };
        _context.SimMethods.Add(referenceMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.FindOverrideOrReferenceMethod(instanceClass, referenceClass, signature);

        result.Should().NotBeNull();
        result.Id.Should().Be(referenceMethod.Id);
    }

    [TestMethod]
    public void CanOverrideFromBaseClass_ReturnsFalse_WhenMethodIsNull()
    {
        var baseClassId = Guid.NewGuid();
        var result = _executionDataAccess!.CanOverrideFromBaseClass(baseClassId, null);
        result.Should().BeFalse();
    }

    [TestMethod]
    public void CanOverrideFromBaseClass_ReturnsTrue_WhenMethodIsNotOverride()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context!.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var method = new SimMethod { Name = "TestMethod", IsOverride = false };
        var result = _executionDataAccess!.CanOverrideFromBaseClass(baseClass.Id, method);
        result.Should().BeTrue();
    }

    [TestMethod]
    public void CanOverrideFromBaseClass_ReturnsFalse_WhenBaseClassNotFound()
    {
        var baseClassId = Guid.NewGuid();
        var method = new SimMethod { Name = "TestMethod", IsOverride = true };
        var result = _executionDataAccess!.CanOverrideFromBaseClass(baseClassId, method);
        result.Should().BeFalse();
    }

    [TestMethod]
    public void CanOverrideFromBaseClass_ReturnsTrue_WhenBaseMethodIsVirtualOrAbstractOrInterfaceAndAccessible()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context!.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var method = new SimMethod
        {
            Name = "TestMethod",
            IsOverride = true,
            IsVirtual = true,
            Accesibility = SimAccesibility.Normal,
            Privacity = SimPrivacity.Public
        };
        var baseMethod = new SimMethod
        {
            Name = "TestMethod",
            IsVirtual = true,
            Accesibility = SimAccesibility.Abstract,
            Privacity = SimPrivacity.Public,
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var result = _executionDataAccess!.CanOverrideFromBaseClass(baseClass.Id, method);
        result.Should().BeTrue();
    }

    [TestMethod]
    public void CanOverrideFromBaseClass_RecursesToParent_WhenNotFoundInCurrentBase()
    {
        var grandparent = new SimClass { Name = "Grandparent" };
        _context!.SimClasses.Add(grandparent);
        _context.SaveChanges();

        var parent = new SimClass { Name = "Parent", BaseClassId = grandparent.Id, BaseClass = grandparent };
        _context.SimClasses.Add(parent);
        _context.SaveChanges();

        var method = new SimMethod
        {
            Name = "TestMethod",
            IsOverride = true,
            IsVirtual = true,
            Accesibility = SimAccesibility.Normal,
            Privacity = SimPrivacity.Public
        };
        var baseMethod = new SimMethod
        {
            Name = "TestMethod",
            IsVirtual = true,
            Accesibility = SimAccesibility.Abstract,
            Privacity = SimPrivacity.Public,
            RelatedClassId = grandparent.Id,
            RelatedClass = grandparent
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var result = _executionDataAccess!.CanOverrideFromBaseClass(parent.Id, method);
        result.Should().BeTrue();
    }

    [TestMethod]
    public void FindSealedMethodInHierarchyFromBaseClass_ReturnsNull_WhenClassNotFound()
    {
        var baseClassId = Guid.NewGuid();
        var method = new SimMethod { Name = "TestMethod", IsVirtual = true };
        var result = _executionDataAccess!.FindSealedMethodInHierarchyFromBaseClass(baseClassId, method);
        result.Should().BeNull();
    }

    [TestMethod]
    public void FindSealedMethodInHierarchyFromBaseClass_ReturnsNull_WhenMethodIsNotVirtual()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context!.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var method = new SimMethod { Name = "TestMethod", IsVirtual = false };
        var result = _executionDataAccess!.FindSealedMethodInHierarchyFromBaseClass(baseClass.Id, method);
        result.Should().BeNull();
    }

    [TestMethod]
    public void FindSealedMethodInHierarchyFromBaseClass_ReturnsSealedMethod_WhenFound()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context!.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var method = new SimMethod { Name = "TestMethod", IsVirtual = true };
        var sealedMethod = new SimMethod
        {
            Name = "TestMethod",
            IsVirtual = true,
            Accesibility = SimAccesibility.Sealed,
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass
        };
        _context.SimMethods.Add(sealedMethod);
        _context.SaveChanges();

        var result = _executionDataAccess!.FindSealedMethodInHierarchyFromBaseClass(baseClass.Id, method);
        result.Should().NotBeNull();
        result.Accesibility.Should().Be(SimAccesibility.Sealed);
    }

    [TestMethod]
    public void FindSealedMethodInHierarchyFromBaseClass_RecursesToParent_WhenNotFoundInCurrentBase()
    {
        var grandparent = new SimClass { Name = "Grandparent" };
        _context!.SimClasses.Add(grandparent);
        _context.SaveChanges();

        var parent = new SimClass { Name = "Parent", BaseClassId = grandparent.Id, BaseClass = grandparent };
        _context.SimClasses.Add(parent);
        _context.SaveChanges();

        var method = new SimMethod { Name = "TestMethod", IsVirtual = true };
        var sealedMethod = new SimMethod
        {
            Name = "TestMethod",
            IsVirtual = true,
            Accesibility = SimAccesibility.Sealed,
            RelatedClassId = grandparent.Id,
            RelatedClass = grandparent
        };
        _context.SimMethods.Add(sealedMethod);
        _context.SaveChanges();

        var result = _executionDataAccess!.FindSealedMethodInHierarchyFromBaseClass(parent.Id, method);
        result.Should().NotBeNull();
        result.Accesibility.Should().Be(SimAccesibility.Sealed);
    }

    [TestMethod]
    public void CanOverrideFromImplementedInterfaces_ReturnsFalse_WhenMethodToOverrideIsNull()
    {
        var simClass = new SimClass { Name = "TestClass" };
        _context!.SimClasses.Add(simClass);
        _context.SaveChanges();

        var result = _executionDataAccess!.CanOverrideFromImplementedInterfaces(simClass, null);

        result.Should().BeFalse("Method should return false when methodToOverride is null");
    }

    [TestMethod]
    public void CanOverrideFromImplementedInterfaces_ReturnsFalse_WhenClassHasNoImplements()
    {
        var simClass = new SimClass { Name = "TestClass", Implements = [] };
        _context!.SimClasses.Add(simClass);
        _context.SaveChanges();

        var method = new SimMethod { Name = "TestMethod" };

        var result = _executionDataAccess!.CanOverrideFromImplementedInterfaces(simClass, method);

        result.Should().BeFalse("Method should return false when class implements no interfaces");
    }

    [TestMethod]
    public void CanOverrideFromImplementedInterfaces_ReturnsTrue_WhenDirectInterfaceHasMatchingMethod()
    {
        var interfaceClass = new SimClass
        {
            Name = "ITestInterface",
            State = SimAccesibility.Interface
        };
        _context!.SimClasses.Add(interfaceClass);
        _context.SaveChanges();

        var interfaceMethod = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = interfaceClass.Id,
            RelatedClass = interfaceClass
        };
        _context.SimMethods.Add(interfaceMethod);
        _context.SaveChanges();

        var simClass = new SimClass
        {
            Name = "TestClass",
            Implements = [interfaceClass]
        };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var methodToOverride = new SimMethod { Name = "TestMethod" };

        var result = _executionDataAccess!.CanOverrideFromImplementedInterfaces(simClass, methodToOverride);

        result.Should().BeTrue("Method should return true when a direct interface has a matching method");
    }

    [TestMethod]
    public void CanOverrideFromImplementedInterfaces_ReturnsFalse_WhenInterfacesHaveNoMatchingMethod()
    {
        var interfaceClass = new SimClass
        {
            Name = "ITestInterface",
            State = SimAccesibility.Interface
        };
        _context!.SimClasses.Add(interfaceClass);
        _context.SaveChanges();

        var interfaceMethod = new SimMethod
        {
            Name = "DifferentMethod",
            RelatedClassId = interfaceClass.Id,
            RelatedClass = interfaceClass
        };
        _context.SimMethods.Add(interfaceMethod);
        _context.SaveChanges();

        var simClass = new SimClass
        {
            Name = "TestClass",
            Implements = [interfaceClass]
        };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var methodToOverride = new SimMethod { Name = "TestMethod" };

        var result = _executionDataAccess!.CanOverrideFromImplementedInterfaces(simClass, methodToOverride);

        result.Should().BeFalse("Method should return false when interfaces don't have matching method");
    }

    [TestMethod]
    public void CanOverrideFromImplementedInterfaces_ContinuesToNextInterface_WhenInterfaceClassIsNull()
    {
        var realInterface = new SimClass
        {
            Name = "IRealInterface",
            State = SimAccesibility.Interface
        };
        _context!.SimClasses.Add(realInterface);
        _context.SaveChanges();

        var interfaceMethod = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = realInterface.Id,
            RelatedClass = realInterface,
            Accesibility = SimAccesibility.Interface
        };
        _context.SimMethods.Add(interfaceMethod);
        _context.SaveChanges();

        var nonExistentInterfaceId = Guid.NewGuid();
        var nonExistentInterface = new SimClass
        {
            Id = nonExistentInterfaceId,
            Name = "INonExistentInterface",
            State = SimAccesibility.Interface,
        };

        var simClass = new SimClass
        {
            Name = "TestClass",
            Implements = [nonExistentInterface, realInterface]
        };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var methodToOverride = new SimMethod { Name = "TestMethod" };

        var result = _executionDataAccess!.CanOverrideFromImplementedInterfaces(simClass, methodToOverride);

        result.Should().BeTrue("Method should continue to check other interfaces when an interface class is null");
    }

    [TestMethod]
    public void CanOverrideFromImplementedInterfaces_ContinuesToNextInterface_WhenInterfaceMethodsIsNull()
    {
        var interfaceWithoutMethods = new SimClass
        {
            Name = "IWithoutMethods",
            State = SimAccesibility.Interface,
            Methods = null
        };
        var interfaceWithMethods = new SimClass
        {
            Name = "IWithMethods",
            State = SimAccesibility.Interface
        };
        _context!.SimClasses.AddRange(interfaceWithoutMethods, interfaceWithMethods);
        _context.SaveChanges();

        var interfaceMethod = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = interfaceWithMethods.Id,
            RelatedClass = interfaceWithMethods
        };
        _context.SimMethods.Add(interfaceMethod);
        _context.SaveChanges();

        var simClass = new SimClass
        {
            Name = "TestClass",
            Implements = [interfaceWithoutMethods, interfaceWithMethods]
        };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var methodToOverride = new SimMethod { Name = "TestMethod" };

        var result = _executionDataAccess!.CanOverrideFromImplementedInterfaces(simClass, methodToOverride);

        result.Should().BeTrue("Method should continue to check other interfaces when Methods is null");
    }

    [TestMethod]
    public void CanOverrideFromImplementedInterfaces_ReturnsTrue_WhenOneOfMultipleInterfacesHasMatchingMethod()
    {
        var interface1 = new SimClass
        {
            Name = "IInterface1",
            State = SimAccesibility.Interface
        };
        var interface2 = new SimClass
        {
            Name = "IInterface2",
            State = SimAccesibility.Interface
        };
        _context!.SimClasses.AddRange(interface1, interface2);
        _context.SaveChanges();

        var interface1Method = new SimMethod
        {
            Name = "Method1",
            RelatedClassId = interface1.Id,
            RelatedClass = interface1
        };
        var interface2Method = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = interface2.Id,
            RelatedClass = interface2
        };
        _context.SimMethods.AddRange(interface1Method, interface2Method);
        _context.SaveChanges();

        var simClass = new SimClass
        {
            Name = "TestClass",
            Implements = [interface1, interface2]
        };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var methodToOverride = new SimMethod { Name = "TestMethod" };

        var result = _executionDataAccess!.CanOverrideFromImplementedInterfaces(simClass, methodToOverride);

        result.Should().BeTrue("Method should return true when one of multiple interfaces has a matching method");
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtected_DoesNotFindStaticMethod_InBaseClass()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(returnType);
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass,
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var staticMethod = new SimMethod
        {
            Name = "StaticMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Public,
            IsStatic = true,
            ReturnTypeId = typeId,
            ReturnType = returnType,
        };
        baseClass.Methods.Add(staticMethod);
        _context.SimMethods.Add(staticMethod);
        _context.SaveChanges();

        var signature = new Signature
        {
            Name = "StaticMethod",
            Parameters = [],
            ReturnTypeId = typeId,
            ReturnType = returnType
        };

        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(childClass, signature);

        result.Should().BeNull("Static methods in base classes should not be found in hierarchy");
    }
}
