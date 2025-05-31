using BusinessLogic;
using BusinessLogic.Exceptions;
using Domain;
using Domain.Enums;
using IDataAccess;
using Moq;

namespace Tests.BusinessLogic;

[TestClass]
public class ExecutionServiceTest
{
    private Mock<IExecutionDataAccess>? _mockExecuteDataAccess;
    private Mock<IApikeyDataAccess>? _mockApikeyDataAccess;
    private ExecutionService? _executionService;

    [TestInitialize]
    public void Initialize()
    {
        _mockExecuteDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Strict);
        _mockApikeyDataAccess = new Mock<IApikeyDataAccess>(MockBehavior.Strict);
        _executionService = new ExecutionService(_mockExecuteDataAccess.Object, _mockApikeyDataAccess.Object);
    }

    [TestMethod]
    public void ExecuteMethod_BasicMethodWithoutInvocations_ReturnsFormattedOutput()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClass = simClass,
            Invocations = []
        };
        var signature = new Signature { Name = "TestMethod", Parameters = [] };

        var mockRef = new Mock<Reference>();
        mockRef.Setup(r => r.GetReferenceClass()).Returns(simClass);
        mockRef.Setup(r => r.GetSignature(signature)).Returns("TestClass.TestMethod()");
        mockRef.Setup(r => r.GetSignatureWithClassName(signature)).Returns("TestClass.TestMethod()");
        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature, 0))
            .Returns(method);

        var result = _executionService!.ExecuteMethod(
            simClass,
            simClass,
            mockRef.Object,
            signature,
            null,
            0
        );
        Assert.AreEqual("TestClass.TestMethod() -> TestClass.TestMethod()\n", result);
    }

    [TestMethod]
    public void ExecuteMethod_NestedInvocations_FormatsProperly()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };

        var innerMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "InnerMethod",
            RelatedClass = simClass,
            RelatedClassId = simClass.Id,
            Invocations = []
        };

        var outerMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "OuterMethod",
            RelatedClass = simClass,
            RelatedClassId = simClass.Id,
            Invocations = []
        };

        var innerSignature = new Signature { Name = "InnerMethod", Parameters = [] };
        var outerSignature = new Signature { Name = "OuterMethod", Parameters = [] };

        var thisRef = new Mock<ReferenceThis>();
        thisRef.Setup(r => r.GetReferenceClass()).Returns(simClass);
        thisRef.Setup(r => r.GetSignature(innerSignature)).Returns("this.InnerMethod()");
        thisRef.Setup(r => r.GetSignatureWithClassName(outerSignature)).Returns("TestClass.OuterMethod()");

        var invocation = new Invocation
        {
            Reference = thisRef.Object,
            Signature = innerSignature
        };

        outerMethod.Invocations.Add(invocation);

        _mockExecuteDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Loose);

        _mockExecuteDataAccess
            .Setup(m => m.FindMethodInHierarchy(simClass, outerSignature, It.IsAny<int>()))
            .Returns(outerMethod);

        _mockExecuteDataAccess
            .Setup(m => m.FindMethodInHierarchy(simClass, innerSignature, It.IsAny<int>()))
            .Returns(innerMethod);

        _executionService = new ExecutionService(_mockExecuteDataAccess.Object, _mockApikeyDataAccess.Object);

        var result = _executionService.ExecuteMethod(
            simClass,
            simClass,
            thisRef.Object,
            outerSignature,
            null,
            0
        );
        Assert.IsTrue(result.Contains("TestClass.OuterMethod()"));
        Assert.IsTrue(result.Contains("this.InnerMethod()"));

        StringAssert.Contains(result, "    this.InnerMethod()");
    }

    [TestMethod]
    public void ExecuteMethod_RecursiveCall_DetectsRecursion()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "Recursive" };
        var methodId = Guid.NewGuid();
        var signature = new Signature { Name = "RecursiveMethod", Parameters = [] };

        var method = new SimMethod
        {
            Id = methodId,
            Name = "RecursiveMethod",
            RelatedClass = simClass,
            Invocations = []
        };

        var recursiveInvocation = new Invocation
        {
            Reference = new ReferenceThis { Reference = simClass },
            Signature = signature
        };
        method.Invocations.Add(recursiveInvocation);

        var mockRef = new Mock<Reference>();
        mockRef.Setup(r => r.GetReferenceClass()).Returns(simClass);
        mockRef.Setup(r => r.GetSignature(signature)).Returns("Recursive.RecursiveMethod()");

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature, 0))
            .Returns(method);

        var result = _executionService!.ExecuteMethod(
            simClass,
            simClass,
            mockRef.Object,
            signature,
            null,
            0
        );
        Assert.IsTrue(result.Contains("Recursive.RecursiveMethod()\n"));
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void ValidateMethodExistsInClass_MethodDoesNotExist_Throws()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        var signature = new Signature { Name = "MissingMethod", Parameters = [] };

        _mockExecuteDataAccess.Setup(m => m.FindMethodInHierarchyPublicOrProtected(simClass, signature, 0)).Returns((SimMethod?)null);

        _executionService.ValidateMethodExistsInClass(simClass, signature, true);
    }

    [TestMethod]
    public void SaveExecutionLog_CreatesLogAndCallsDataAccess()
    {
        var reference = "TestClass";
        var objCreate = "TestObject";
        var execution = "Test execution trace";

        _mockExecuteDataAccess!
            .Setup(m => m.SaveExecutionLog(It.Is<ExecutionLog>(log =>
                log.Reference == reference &&
                log.ObjectCreate == objCreate &&
                log.Execution == execution)))
            .Verifiable();

        _executionService!.SaveExecutionLog(reference, objCreate, execution);

        _mockExecuteDataAccess.Verify(m => m.SaveExecutionLog(It.Is<ExecutionLog>(log =>
            log.Reference == reference &&
            log.ObjectCreate == objCreate &&
            log.Execution == execution)),
            Times.Once());
    }

    [TestMethod]
    public void IsReferenceBaseOfInstance_SameClass_ReturnsTrue()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };

        var result = _executionService!.IsReferenceBaseOfInstance(simClass, simClass);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsReferenceBaseOfInstance_BaseClassNotFound_ReturnsFalse()
    {
        var parentClass = new SimClass { Id = Guid.NewGuid(), Name = "ParentClass" };
        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = parentClass.Id
        };

        _mockExecuteDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Loose);
        _executionService = new ExecutionService(_mockExecuteDataAccess.Object, _mockApikeyDataAccess.Object);

        _mockExecuteDataAccess
            .Setup(m => m.GetFilteredClasses(It.Is<Func<IQueryable<SimClass>, IQueryable<SimClass>>>(
                func => true)))
            .Returns([]);

        var result = _executionService.IsReferenceBaseOfInstance(new SimClass { Id = Guid.NewGuid() }, childClass);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsReferenceBaseOfInstance_Unrelated_ReturnsFalse()
    {
        var classA = new SimClass { Id = Guid.NewGuid(), Name = "ClassA" };
        var classB = new SimClass { Id = Guid.NewGuid(), Name = "ClassB" };

        _ = _mockExecuteDataAccess.Setup(exec => exec.GetFilteredClasses(It.IsAny<Func<IQueryable<SimClass>, IQueryable<SimClass>>>()))
            .Returns([]);

        var result = _executionService!.IsReferenceBaseOfInstance(classA, classB);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsReferenceBaseOfInstance_NullClasses_ReturnsFalse()
    {
        Assert.IsFalse(_executionService!.IsReferenceBaseOfInstance(null, new SimClass()));
        Assert.IsFalse(_executionService!.IsReferenceBaseOfInstance(new SimClass(), null));
        Assert.IsFalse(_executionService!.IsReferenceBaseOfInstance(null, null));
    }

    [TestMethod]
    public void ValidateMethodExistsInClass_AbstractMethodWithIsNotAbstractFalse_DoesNotThrow()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        var signature = new Signature { Name = "AbstractMethod", Parameters = [] };
        var abstractMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "AbstractMethod",
            RelatedClass = simClass,
            Accesibility = SimAccesibility.Abstract
        };

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchyPublicOrProtected(simClass, signature, 0))
            .Returns(abstractMethod);

        _executionService!.ValidateMethodExistsInClass(simClass, signature, false);
    }

    [TestMethod]
    public void IsAuthorizedUser_EmptyApiKey_ReturnsFalse()
    {
        var apiKey = Guid.Empty;

        _mockApikeyDataAccess!
            .Setup(m => m.ApiKeyExists(apiKey))
            .Returns(false);

        var result = _executionService!.IsAuthorizedUser(apiKey);

        Assert.IsFalse(result);
        _mockApikeyDataAccess.Verify(m => m.ApiKeyExists(apiKey), Times.Once);
    }

    [TestMethod]
    public void IsAuthorizedUser_NonExistentApiKey_ReturnsFalse()
    {
        var apiKey = Guid.NewGuid();

        _mockApikeyDataAccess!
            .Setup(m => m.ApiKeyExists(apiKey))
            .Returns(false);

        var result = _executionService.IsAuthorizedUser(apiKey);

        Assert.IsFalse(result);
        _mockApikeyDataAccess.Verify(m => m.ApiKeyExists(apiKey), Times.Once);
    }

    [TestMethod]
    public void IsAuthorizedUser_ValidCase_ReturnsTrue()
    {
        var validApikey = new Guid("77777777-aaaa-1111-1111-111111111111");
        _mockApikeyDataAccess!
            .Setup(m => m.ApiKeyExists(validApikey))
            .Returns(true);

        var result = _executionService.IsAuthorizedUser(validApikey);

        Assert.IsTrue(result);
        _mockApikeyDataAccess.Verify(m => m.ApiKeyExists(validApikey), Times.Once);
    }

    [TestMethod]
    public void ExecuteMethod_DynamicDispatch_UsesObjRealClass()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            IsVirtual = true,
            Invocations = [],
            RelatedClass = simClass,
            RelatedClassId = simClass.Id
        };

        var innerMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "InnerMethod",
            Invocations = [],
            RelatedClass = simClass,
            RelatedClassId = simClass.Id
        };
        var innerSignature = new Signature { Name = "InnerMethod", Parameters = [] };

        var thisRef = new Mock<ReferenceThis>();
        thisRef.Setup(r => r.GetReferenceClass()).Returns(simClass);
        thisRef.Setup(r => r.GetSignature(It.IsAny<Signature>())).Returns("this.Method()");
        thisRef.Setup(r => r.GetSignatureWithClassName(It.IsAny<Signature>())).Returns("TestClass.Method()");

        var invocation = new Invocation
        {
            Reference = thisRef.Object,
            Signature = innerSignature
        };
        method.Invocations.Add(invocation);

        var signature = new Signature { Name = "TestMethod", Parameters = [] };

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature, It.IsAny<int>()))
            .Returns(method);

        _mockExecuteDataAccess
            .Setup(m => m.FindMethodInHierarchy(simClass, innerSignature, It.IsAny<int>()))
            .Returns(innerMethod);

        var result = _executionService!.ExecuteMethod(
            simClass,
            simClass,
            thisRef.Object,
            signature,
            null,
            0
        );
        Assert.IsTrue(result.Contains("TestClass.Method()"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationLogic))]
    public void ExecuteMethod_PrivateMethodFromDifferentClass_ThrowsException()
    {
        var referenceClass = new SimClass { Id = Guid.NewGuid(), Name = "ReferenceClass" };
        var objClass = new SimClass { Id = Guid.NewGuid(), Name = "ObjectClass" };

        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "PrivateMethod",
            Privacity = SimPrivacity.Private,
            Invocations = []
        };

        var signature = new Signature { Name = "PrivateMethod", Parameters = [] };

        var mockRef = new Mock<Reference>();
        mockRef.Setup(r => r.GetReferenceClass()).Returns(referenceClass);

        var mockObjReal = new Mock<Reference>();
        mockObjReal.Setup(r => r.GetReferenceClass()).Returns(objClass);

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(referenceClass, signature, It.IsAny<int>()))
            .Returns(method);

        _executionService!.ExecuteMethod(
            referenceClass,
            objClass,
            mockRef.Object,
            signature,
            null,
            0
        );
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationLogic))]
    public void ExecuteMethod_MethodNotFound_ThrowsException()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        var signature = new Signature { Name = "NonExistentMethod", Parameters = [] };

        var mockRef = new Mock<Reference>();
        mockRef.Setup(r => r.GetReferenceClass()).Returns(simClass);

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature, It.IsAny<int>()))
            .Returns((SimMethod?)null);

        _executionService!.ExecuteMethod(
            simClass,
            simClass,
            mockRef.Object,
            signature,
            null,
            0
        );
    }

    [TestMethod]
    public void IsReferenceBaseOfInstance_NoBaseClassId_ReturnsFalse()
    {
        var refer = new SimClass { Id = Guid.NewGuid(), Name = "ReferClass" };
        var obj = new SimClass { Id = Guid.NewGuid(), Name = "ObjClass", BaseClassId = null };

        _mockExecuteDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Loose);
        _executionService = new ExecutionService(_mockExecuteDataAccess.Object, _mockApikeyDataAccess.Object);

        var result = _executionService.IsReferenceBaseOfInstance(refer, obj);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsReferenceBaseOfInstance_BaseClassIdMatchesReferId_ReturnsTrue()
    {
        var refer = new SimClass { Id = Guid.NewGuid(), Name = "ReferClass" };
        var obj = new SimClass { Id = Guid.NewGuid(), Name = "ObjClass", BaseClassId = refer.Id };

        _mockExecuteDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Loose);
        _executionService = new ExecutionService(_mockExecuteDataAccess.Object, _mockApikeyDataAccess.Object);

        var result = _executionService.IsReferenceBaseOfInstance(refer, obj);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsReferenceBaseOfInstance_RecursiveBaseClass_ReturnsTrue()
    {
        var refer = new SimClass { Id = Guid.NewGuid(), Name = "ReferClass" };
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass", BaseClassId = refer.Id };
        var obj = new SimClass { Id = Guid.NewGuid(), Name = "ObjClass", BaseClassId = baseClass.Id };

        var mockDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Loose);

        mockDataAccess
            .Setup(m => m.GetFilteredClasses(It.Is<Func<IQueryable<SimClass>, IQueryable<SimClass>>>(f =>
                f(new List<SimClass> { baseClass }.AsQueryable()).Any(c => c.Id == baseClass.Id))))
            .Returns([baseClass]);

        mockDataAccess
            .Setup(m => m.GetFilteredClasses(It.Is<Func<IQueryable<SimClass>, IQueryable<SimClass>>>(f =>
                f(new List<SimClass> { refer }.AsQueryable()).Any(c => c.Id == refer.Id))))
            .Returns([refer]);

        var service = new ExecutionService(mockDataAccess.Object, _mockApikeyDataAccess.Object);

        var result = service.IsReferenceBaseOfInstance(refer, obj);

        Assert.IsTrue(result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeLogic))]
    public void ValidateMethodExistsInClass_AbstractMethodWithIsNotAbstractTrue_Throws()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        var signature = new Signature { Name = "AbstractMethod", Parameters = [] };
        var abstractMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "AbstractMethod",
            RelatedClass = simClass,
            Accesibility = SimAccesibility.Abstract
        };

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchyPublicOrProtected(simClass, signature, 0))
            .Returns(abstractMethod);

        _executionService!.ValidateMethodExistsInClass(simClass, signature, true);
    }
}
