using Adapter;
using Domain;
using Domain.Enums;
using IAdapter.Exceptions;
using IBusinessLogic;
using IBusinessLogic.Exceptions;
using Models.Request;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class ExecutionAdapterTest
{
    private Mock<IExecutionService>? _mockExecutionService;
    private ExecutionAdapter? _executionAdapter;
    private Mock<ISimClassService>? _simClassService;

    [TestInitialize]
    public void Initialize()
    {
        _mockExecutionService = new Mock<IExecutionService>(MockBehavior.Strict);
        _simClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _executionAdapter = new ExecutionAdapter(_mockExecutionService.Object, _simClassService.Object);
    }

    [TestMethod]
    public void ExecuteMethodValidInputs_ShouldMapParametersAndCallExecutionService()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var instanceTypeId = Guid.NewGuid();
        var referenceTypeId = Guid.NewGuid();
        var param1TypeId = Guid.NewGuid();
        var param2TypeId = Guid.NewGuid();
        var param1InstanceId = Guid.NewGuid();
        var param2InstanceId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdReturnType = typeId.ToString(),
            IdInstanceType = instanceTypeId.ToString(),
            IdReferenceType = referenceTypeId.ToString(),
            Parameters =
            [
                new ParameterSignatureRequest { Name = "param1", IdReference = param1TypeId.ToString(), IdInstance = param1InstanceId.ToString() },
                new ParameterSignatureRequest { Name = "param2", IdReference = param2TypeId.ToString(), IdInstance = param2InstanceId.ToString() }
            ],
        };

        var instanceTypeClass = new SimClass { Id = instanceTypeId, Name = "InstanceType" };
        var referenceTypeClass = new SimClass { Id = referenceTypeId, Name = "ReferenceType" };
        var simClass1 = new SimClass { Id = param1TypeId, Name = "Type1" };
        var simClass2 = new SimClass { Id = param2TypeId, Name = "Type2" };
        var simInstance1 = new SimClass { Id = param1InstanceId, Name = "Instance1" };
        var simInstance2 = new SimClass { Id = param2InstanceId, Name = "Instance2" };

        _simClassService!
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(returnType);

        _simClassService!
            .Setup(s => s.GetSimClassById(instanceTypeId))
            .Returns(instanceTypeClass);

        _simClassService!
            .Setup(s => s.GetSimClassById(referenceTypeId))
            .Returns(referenceTypeClass);

        _simClassService!
            .Setup(s => s.GetSimClassById(param1TypeId))
            .Returns(simClass1);

        _simClassService!
            .Setup(s => s.GetSimClassById(param2TypeId))
            .Returns(simClass2);

        _simClassService!
            .Setup(s => s.GetSimClassById(param1InstanceId))
            .Returns(simInstance1);

        _simClassService!
            .Setup(s => s.GetSimClassById(param2InstanceId))
            .Returns(simInstance2);

        _mockExecutionService!
            .Setup(s => s.ExecuteMethod(
                It.Is<SimClass>(c => c.Id == referenceTypeId),
                It.Is<SimClass>(c => c.Id == instanceTypeId),
                It.Is<Reference>(r => r is ReferenceThis && ((ReferenceThis)r).Reference.Id == referenceTypeId),
                It.Is<Signature>(sig =>
                    sig.Name == "TestMethod" &&
                    sig.Parameters.Count == 2 &&
                    sig.ReturnTypeId == typeId &&
                    sig.ReturnType == returnType),
                It.IsAny<HashSet<Guid>>(),
                It.IsAny<int>()))
            .Returns("expectedResult");

        _mockExecutionService!
            .Setup(s => s.SaveExecutionLog("ReferenceType", "InstanceType", "expectedResult"));

        var result = _executionAdapter!.ExecuteMethod(request).Execution;

        Assert.AreEqual("expectedResult", result);
        _simClassService.VerifyAll();
        _mockExecutionService.VerifyAll();
    }

    [TestMethod]
    public void ExecuteMethod_WhenExceptionThrown_ShouldThrowInvalidExecutionException()
    {
        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdInstanceType = Guid.NewGuid().ToString(),
            IdReferenceType = Guid.NewGuid().ToString(),
            Parameters =
        [
            new ParameterSignatureRequest { Name = "param1", IdReference = Guid.NewGuid().ToString() }
        ],
        };

        _simClassService!
            .Setup(s => s.GetSimClassById(It.IsAny<Guid>()))
            .Throws(new InvalidOperationLogic("SimClass error"));

        Assert.ThrowsException<InvalidExecutionAdapter>(() =>
        {
            _executionAdapter!.ExecuteMethod(request);
        });
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidExecutionAdapter))]
    public void ExecuteMethod_WhenReferenceIsNotBaseOfInstance_ThrowsInvalidExecutionAdapter()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var instanceTypeId = Guid.NewGuid();
        var referenceTypeId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdReturnType = typeId.ToString(),
            IdInstanceType = instanceTypeId.ToString(),
            IdReferenceType = referenceTypeId.ToString(),
            Parameters = [],
        };

        var instanceTypeClass = new SimClass { Id = instanceTypeId, Name = "InstanceType" };
        var referenceTypeClass = new SimClass { Id = referenceTypeId, Name = "ReferenceType" };

        _simClassService!.Setup(s => s.GetSimClassById(typeId)).Returns(returnType);
        _simClassService!.Setup(s => s.GetSimClassById(instanceTypeId)).Returns(instanceTypeClass);
        _simClassService!.Setup(s => s.GetSimClassById(referenceTypeId)).Returns(referenceTypeClass);

        _mockExecutionService!
            .Setup(s => s.ExecuteMethod(
                It.Is<SimClass>(c => c.Id == referenceTypeId),
                It.Is<SimClass>(c => c.Id == instanceTypeId),
                It.IsAny<Reference>(),
                It.Is<Signature>(sig =>
                    sig.Name == "TestMethod" &&
                    sig.ReturnTypeId == typeId &&
                    sig.ReturnType == returnType),
                It.IsAny<HashSet<Guid>>(),
                It.IsAny<int>()))
            .Throws(new InvalidExecutionAdapter("No se puede ejecutar sobre una clase abstracta."));

        _executionAdapter!.ExecuteMethod(request);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void ExecuteMethod_WhenNonExistentValueLogicThrown_ShouldThrowNonExistentValueAdapter()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var instanceTypeId = Guid.NewGuid();
        var referenceTypeId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdReturnType = typeId.ToString(),
            IdInstanceType = instanceTypeId.ToString(),
            IdReferenceType = referenceTypeId.ToString(),
            Parameters = [],
        };

        var instanceTypeClass = new SimClass { Id = instanceTypeId, Name = "InstanceType" };
        var referenceTypeClass = new SimClass { Id = referenceTypeId, Name = "ReferenceType" };

        _simClassService!.Setup(s => s.GetSimClassById(typeId)).Returns(returnType);
        _simClassService!.Setup(s => s.GetSimClassById(instanceTypeId)).Returns(instanceTypeClass);
        _simClassService!.Setup(s => s.GetSimClassById(referenceTypeId)).Returns(referenceTypeClass);

        _mockExecutionService!
            .Setup(s => s.IsReferenceBaseOfInstance(referenceTypeClass, instanceTypeClass))
            .Returns(true);

        _mockExecutionService!
            .Setup(s => s.ExecuteMethod(
                It.Is<SimClass>(c => c.Id == referenceTypeId),
                It.Is<SimClass>(c => c.Id == instanceTypeId),
                It.IsAny<Reference>(),
                It.Is<Signature>(sig =>
                    sig.Name == "TestMethod" &&
                    sig.ReturnTypeId == typeId &&
                    sig.ReturnType == returnType),
                It.IsAny<HashSet<Guid>>(),
                It.IsAny<int>()))
            .Throws(new NonExistentValueLogic("Method not found"));

        _executionAdapter!.ExecuteMethod(request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidExecutionAdapter))]
    public void ExecuteMethod_WhenClassIsAbstract_ThrowsInvalidExecutionAdapter()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var instanceTypeId = Guid.NewGuid();
        var referenceTypeId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdReturnType = typeId.ToString(),
            IdInstanceType = instanceTypeId.ToString(),
            IdReferenceType = referenceTypeId.ToString(),
            Parameters = [],
        };

        var instanceTypeClass = new SimClass
        {
            Id = instanceTypeId,
            Name = "InstanceType",
            State = SimAccesibility.Abstract,
        };
        var referenceTypeClass = new SimClass { Id = referenceTypeId, Name = "ReferenceType" };

        _simClassService!.Setup(s => s.GetSimClassById(typeId)).Returns(returnType);
        _simClassService!.Setup(s => s.GetSimClassById(instanceTypeId)).Returns(instanceTypeClass);
        _simClassService!.Setup(s => s.GetSimClassById(referenceTypeId)).Returns(referenceTypeClass);

        _mockExecutionService!
            .Setup(s => s.ExecuteMethod(
                It.Is<SimClass>(c => c.Id == referenceTypeId),
                It.Is<SimClass>(c => c.Id == instanceTypeId),
                It.IsAny<Reference>(),
                It.Is<Signature>(sig =>
                    sig.Name == "TestMethod" &&
                    sig.ReturnTypeId == typeId &&
                    sig.ReturnType == returnType),
                It.IsAny<HashSet<Guid>>(),
                It.IsAny<int>()))
            .Throws(new InvalidExecutionAdapter("No se puede ejecutar sobre una clase abstracta."));

        _executionAdapter!.ExecuteMethod(request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void ExecuteMethod_WhenInvalidAttributeLogicThrown_ShouldThrowInvalidAttributeAdapter()
    {
        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var instanceTypeId = Guid.NewGuid();
        var referenceTypeId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdReturnType = typeId.ToString(),
            IdInstanceType = instanceTypeId.ToString(),
            IdReferenceType = referenceTypeId.ToString(),
            Parameters = [],
        };

        var instanceTypeClass = new SimClass { Id = instanceTypeId, Name = "InstanceType" };
        var referenceTypeClass = new SimClass { Id = referenceTypeId, Name = "ReferenceType" };

        _simClassService!.Setup(s => s.GetSimClassById(typeId)).Returns(returnType);
        _simClassService!.Setup(s => s.GetSimClassById(instanceTypeId)).Returns(instanceTypeClass);
        _simClassService!.Setup(s => s.GetSimClassById(referenceTypeId)).Returns(referenceTypeClass);

        _mockExecutionService!
            .Setup(s => s.ExecuteMethod(
                It.Is<SimClass>(c => c.Id == referenceTypeId),
                It.Is<SimClass>(c => c.Id == instanceTypeId),
                It.IsAny<Reference>(),
                It.Is<Signature>(sig =>
                    sig.Name == "TestMethod" &&
                    sig.ReturnTypeId == typeId &&
                    sig.ReturnType == returnType),
                It.IsAny<HashSet<Guid>>(),
                It.IsAny<int>()))
            .Throws(new InvalidAttributeLogic("Invalid attribute"));

        _executionAdapter!.ExecuteMethod(request);
    }
}
