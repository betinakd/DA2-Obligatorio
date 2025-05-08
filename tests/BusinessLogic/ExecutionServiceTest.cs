using BussinesLogic;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Enums;
using IDataAccess;
using Moq;

namespace Tests.BussinesLogic;

[TestClass]
public class ExecutionServiceTest
{
    private Mock<IExecutionDataAccess>? _mockExecuteDataAccess;
    private ExecutionService? _executionService;

    [TestInitialize]
    public void Initialize()
    {
        _mockExecuteDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Strict);
        _executionService = new ExecutionService(_mockExecuteDataAccess.Object);
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
        mockRef.Setup(r => r.GetSimClass()).Returns(simClass);
        mockRef.Setup(r => r.GetSignature(signature)).Returns("TestClass.TestMethod()");
        mockRef.Setup(r => r.GetSignatureWithClassName(signature)).Returns("TestClass.TestMethod()");
        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature, 0))
            .Returns(method);

        var result = _executionService!.ExecuteMethod(mockRef.Object, mockRef.Object, signature);

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
        thisRef.Setup(r => r.GetSimClass()).Returns(simClass);
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

        _executionService = new ExecutionService(_mockExecuteDataAccess.Object);

        var result = _executionService.ExecuteMethod(thisRef.Object, thisRef.Object, outerSignature);

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
        mockRef.Setup(r => r.GetSimClass()).Returns(simClass);
        mockRef.Setup(r => r.GetSignature(signature)).Returns("Recursive.RecursiveMethod()");

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature, 0))
            .Returns(method);

        var result = _executionService!.ExecuteMethod(mockRef.Object, mockRef.Object, signature);

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
    public void ClassInheritAttribute_AttributeInherited_DoesNotThrowException()
    {
        var classId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();

        _mockExecuteDataAccess!
            .Setup(m => m.ClassInheritAttribute(classId, attributeId, It.IsAny<int>()))
            .Returns(true);

        _executionService!.ClassInheritAttribute(classId, attributeId);

        _mockExecuteDataAccess.Verify(m => m.ClassInheritAttribute(classId, attributeId, It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void ClassInheritAttribute_AttributeNotInherited_ThrowsNonExistentValueLogic()
    {
        var classId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();

        _mockExecuteDataAccess!
            .Setup(m => m.ClassInheritAttribute(classId, attributeId, It.IsAny<int>()))
            .Returns(false);

        _executionService!.ClassInheritAttribute(classId, attributeId);
    }

    [TestMethod]
    public void ClassInheritAttribute_VerifiesExceptionMessage()
    {
        var classId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();

        _mockExecuteDataAccess!
            .Setup(m => m.ClassInheritAttribute(classId, attributeId, It.IsAny<int>()))
            .Returns(false);

        var exception = Assert.ThrowsException<NonExistentValueLogic>(
            () => _executionService!.ClassInheritAttribute(classId, attributeId));

        Assert.AreEqual("Attribute not reacheable from method.", exception.Message);
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
    public void MethodIsOverridingSealed_CallsDataAccess()
    {
        var classId = Guid.NewGuid();
        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            Accesibility = SimAccesibility.Normal
        };

        _mockExecuteDataAccess!
            .Setup(m => m.MethodIsOverridingSealed(classId, method))
            .Returns(false)
            .Verifiable();

        _executionService!.MethodIsOverridingSealed(classId, method);

        _mockExecuteDataAccess.Verify(m => m.MethodIsOverridingSealed(classId, method), Times.Once());
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
        _executionService = new ExecutionService(_mockExecuteDataAccess.Object);

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
}
