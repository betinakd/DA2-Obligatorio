using Adapter;
using Adapter.Exceptions;
using BusinessLogic.Exceptions;
using Domain;
using Domain.Enums;
using IBusinessLogic;
using Models.Request;
using Moq;
using Transformers.Abstractions;

namespace Tests.Adapter;

[TestClass]
public class ExecutionAdapterTest
{
    private Mock<IExecutionService>? _mockExecutionService;
    private ExecutionAdapter? _executionAdapter;
    private Mock<ISimClassService>? _simClassService;
    private Mock<ITransformerService>? _mockTransformerService;

    [TestInitialize]
    public void Initialize()
    {
        _mockExecutionService = new Mock<IExecutionService>(MockBehavior.Strict);
        _mockTransformerService = new Mock<ITransformerService>(MockBehavior.Strict);
        _simClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _executionAdapter = new ExecutionAdapter(_mockExecutionService.Object, _simClassService.Object, _mockTransformerService.Object);
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
    public void ExecuteMethodWithTransform_ShouldReturnTransformedResponse()
    {
        var paramTypeId = Guid.NewGuid();
        var paramInstanceId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();

        var typeId = Guid.NewGuid();
        var returnType = new SimClass { Id = typeId, Name = "ReturnType" };

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdReturnType = typeId.ToString(),
            IdInstanceType = instanceId.ToString(),
            IdReferenceType = referenceId.ToString(),
            Parameters =
            [
                new ParameterSignatureRequest { Name = "param1", IdReference = paramTypeId.ToString(), IdInstance = paramInstanceId.ToString() }
            ],
        };

        var simClass = new SimClass { Id = paramTypeId, Name = "ParamType", State = SimAccesibility.Normal };
        var simInstanceParam = new SimClass { Id = paramInstanceId, Name = "ParamInstance", State = SimAccesibility.Normal };
        var simReference = new SimClass { Id = referenceId, Name = "Ref", State = SimAccesibility.Normal };
        var simInstance = new SimClass { Id = instanceId, Name = "Obj", State = SimAccesibility.Normal };

        var mockExecutionService = new Mock<IExecutionService>();
        var mockSimClassService = new Mock<ISimClassService>();
        var mockTransformerService = new Mock<ITransformerService>();

        mockSimClassService.Setup(x => x.GetSimClassById(paramTypeId)).Returns(simClass);
        mockSimClassService.Setup(x => x.GetSimClassById(paramInstanceId)).Returns(simInstanceParam);
        mockSimClassService.Setup(x => x.GetSimClassById(referenceId)).Returns(simReference);
        mockSimClassService.Setup(x => x.GetSimClassById(instanceId)).Returns(simInstance);
        mockSimClassService.Setup(x => x.GetSimClassById(typeId)).Returns(returnType);

        mockExecutionService.Setup(x => x.IsReferenceBaseOfInstance(simReference, simInstance)).Returns(true);

        mockExecutionService.Setup(x => x.ExecuteMethod(
                It.Is<SimClass>(c => c.Id == referenceId),
                It.Is<SimClass>(c => c.Id == instanceId),
                It.IsAny<Reference>(),
                It.Is<Signature>(sig =>
                    sig.Name == "TestMethod" &&
                    sig.ReturnTypeId == typeId &&
                    sig.ReturnType == returnType &&
                    sig.Parameters.Count == 1),
                It.IsAny<HashSet<Guid>>(),
                It.IsAny<int>()))
            .Returns("resultado");

        mockExecutionService.Setup(x => x.SaveExecutionLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        var expectedResponse = new TransformedResponse
        {
            OriginalResult = "resultado",
            TransformedResult = "transformado",
            ContentType = "text/plain",
            TransformerId = "test",
        };
        mockTransformerService.Setup(x => x.TransformExecution("resultado", "test")).Returns(expectedResponse);

        var adapter = new ExecutionAdapter(
            mockExecutionService.Object,
            mockSimClassService.Object,
            mockTransformerService.Object);

        var validKey = new Guid("77777777-aaaa-1111-1111-111111111111");
        mockExecutionService.Setup(x => x.IsAuthorizedUser(validKey)).Returns(true);

        var result = adapter.ExecuteMethodWithTransform(request, "test");

        Assert.IsNotNull(result);
        Assert.AreEqual("transformado", result.TransformedResult);
        Assert.AreEqual("test", result.TransformerId);
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
