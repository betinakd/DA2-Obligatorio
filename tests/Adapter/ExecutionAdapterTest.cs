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
        var instanceTypeId = Guid.NewGuid();
        var referenceTypeId = Guid.NewGuid();
        var param1TypeId = Guid.NewGuid();
        var param2TypeId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdInstanceType = instanceTypeId.ToString(),
            IdReferenceType = referenceTypeId.ToString(),
            Parameters =
            [
            new ParameterRequest { Name = "param1", IdReference = param1TypeId.ToString() },
                new ParameterRequest { Name = "param2", IdReference = param2TypeId.ToString() }
            ]
        };

        var instanceTypeClass = new SimClass { Id = instanceTypeId, Name = "InstanceType" };
        var referenceTypeClass = new SimClass { Id = referenceTypeId, Name = "ReferenceType" };
        var simClass1 = new SimClass { Id = param1TypeId, Name = "Type1" };
        var simClass2 = new SimClass { Id = param2TypeId, Name = "Type2" };

        _simClassService!
            .Setup(s => s.GetSimClassById(instanceTypeId))
            .Returns(instanceTypeClass);

        _simClassService
            .Setup(s => s.GetSimClassById(referenceTypeId))
            .Returns(referenceTypeClass);

        _simClassService
            .Setup(s => s.GetSimClassById(param1TypeId))
            .Returns(simClass1);

        _simClassService
            .Setup(s => s.GetSimClassById(param2TypeId))
            .Returns(simClass2);

        _mockExecutionService!
            .Setup(s => s.ExecuteMethod(
                It.Is<Reference>(r => r is ReferenceThis &&
                    ((ReferenceThis)r).Reference.Id == referenceTypeId),
                It.Is<Reference>(r => r is ReferenceThis &&
                    ((ReferenceThis)r).Reference.Id == instanceTypeId),
                It.Is<Signature>(s => s.Name == "TestMethod" && s.Parameters.Count == 2),
                It.IsAny<int>(),
                It.IsAny<HashSet<Guid>>()))
            .Returns("expectedResult");

        _mockExecutionService!
            .Setup(s => s.IsReferenceBaseOfInstance(
                It.Is<SimClass>(c => c.Id == referenceTypeClass.Id),
                It.Is<SimClass>(c => c.Id == instanceTypeClass.Id)))
            .Returns(true);

        _mockExecutionService!.Setup(s => s.SaveExecutionLog("ReferenceType", "InstanceType", "expectedResult"));
        var result = _executionAdapter!.ExecuteMethod(request);

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
            new ParameterRequest { Name = "param1", IdReference = Guid.NewGuid().ToString() }
        ]
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
        var instanceTypeId = Guid.NewGuid();
        var referenceTypeId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdInstanceType = instanceTypeId.ToString(),
            IdReferenceType = referenceTypeId.ToString(),
            Parameters = []
        };

        var instanceTypeClass = new SimClass { Id = instanceTypeId, Name = "InstanceType" };
        var referenceTypeClass = new SimClass { Id = referenceTypeId, Name = "ReferenceType" };

        _simClassService!
            .Setup(s => s.GetSimClassById(instanceTypeId))
            .Returns(instanceTypeClass);

        _simClassService
            .Setup(s => s.GetSimClassById(referenceTypeId))
            .Returns(referenceTypeClass);

        _mockExecutionService!
            .Setup(s => s.IsReferenceBaseOfInstance(
                It.Is<SimClass>(c => c.Id == referenceTypeClass.Id),
                It.Is<SimClass>(c => c.Id == instanceTypeClass.Id)))
            .Returns(false);

        _executionAdapter!.ExecuteMethod(request);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void ExecuteMethod_WhenNonExistentValueLogicThrown_ShouldThrowNonExistentValueAdapter()
    {
        var instanceTypeId = Guid.NewGuid();
        var referenceTypeId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdInstanceType = instanceTypeId.ToString(),
            IdReferenceType = referenceTypeId.ToString(),
            Parameters = []
        };

        var instanceTypeClass = new SimClass { Id = instanceTypeId, Name = "InstanceType" };
        var referenceTypeClass = new SimClass { Id = referenceTypeId, Name = "ReferenceType" };

        _simClassService!
            .Setup(s => s.GetSimClassById(instanceTypeId))
            .Returns(instanceTypeClass);

        _simClassService
            .Setup(s => s.GetSimClassById(referenceTypeId))
            .Returns(referenceTypeClass);

        _mockExecutionService!
            .Setup(s => s.IsReferenceBaseOfInstance(
                It.Is<SimClass>(c => c.Id == referenceTypeClass.Id),
                It.Is<SimClass>(c => c.Id == instanceTypeClass.Id)))
            .Returns(true);

        _mockExecutionService
            .Setup(s => s.ExecuteMethod(
                It.IsAny<Reference>(),
                It.IsAny<Reference>(),
                It.IsAny<Signature>(),
                It.IsAny<int>(),
                It.IsAny<HashSet<Guid>>()))
            .Throws(new NonExistentValueLogic("Method not found"));

        _executionAdapter!.ExecuteMethod(request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidExecutionAdapter))]
    public void ExecuteMethod_WhenClassIsAbstract_ThrowsInvalidExecutionAdapter()
    {
        var instanceTypeId = Guid.NewGuid();
        var referenceTypeId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdInstanceType = instanceTypeId.ToString(),
            IdReferenceType = referenceTypeId.ToString(),
            Parameters = []
        };

        var instanceTypeClass = new SimClass
        {
            Id = instanceTypeId,
            Name = "InstanceType",
            State = SimAccesibility.Abstract
        };

        var referenceTypeClass = new SimClass
        {
            Id = referenceTypeId,
            Name = "ReferenceType"
        };

        _simClassService!
            .Setup(s => s.GetSimClassById(instanceTypeId))
            .Returns(instanceTypeClass);

        _simClassService
            .Setup(s => s.GetSimClassById(referenceTypeId))
            .Returns(referenceTypeClass);

        _executionAdapter!.ExecuteMethod(request);
    }

    [TestMethod]
    public void ExecuteMethodWithTransform_ShouldReturnTransformedResponse()
    {
        var mockExecutionService = new Mock<IExecutionService>();
        var mockSimClassService = new Mock<ISimClassService>();
        var mockTransformerService = new Mock<ITransformerService>();

        var paramTypeId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdInstanceType = instanceId.ToString(),
            IdReferenceType = referenceId.ToString(),
            Parameters =
            [
                new ParameterRequest { Name = "param1", IdReference = paramTypeId.ToString() }
            ]
        };

        var simClass = new SimClass { Id = paramTypeId, Name = "ParamType", State = SimAccesibility.Normal };
        var simReference = new SimClass { Id = referenceId, Name = "Ref", State = SimAccesibility.Normal };
        var simInstance = new SimClass { Id = instanceId, Name = "Obj", State = SimAccesibility.Normal };

        mockSimClassService.Setup(x => x.GetSimClassById(paramTypeId)).Returns(simClass);
        mockSimClassService.Setup(x => x.GetSimClassById(referenceId)).Returns(simReference);
        mockSimClassService.Setup(x => x.GetSimClassById(instanceId)).Returns(simInstance);

        mockExecutionService.Setup(x => x.IsReferenceBaseOfInstance(simReference, simInstance)).Returns(true);

        mockExecutionService.Setup(x => x.ExecuteMethod(It.IsAny<ReferenceThis>(), It.IsAny<ReferenceThis>(), It.IsAny<Signature>(), It.IsAny<int>(), It.IsAny<HashSet<Guid>>()))
            .Returns("resultado");

        mockExecutionService.Setup(x => x.SaveExecutionLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        var expectedResponse = new TransformedResponse
        {
            OriginalResult = "resultado",
            TransformedResult = "transformado",
            ContentType = "text/plain",
            TransformerId = "test"
        };
        mockTransformerService.Setup(x => x.TransformExecution("resultado", "test")).Returns(expectedResponse);

        var adapter = new ExecutionAdapter(
            mockExecutionService.Object,
            mockSimClassService.Object,
            mockTransformerService.Object);

        var validKey = new Guid("77777777-aaaa-1111-1111-111111111111");
        mockExecutionService.Setup(x => x.IsAuthorizedUser(validKey)).Returns(true);
        var result = adapter.ExecuteMethodWithTransform(validKey, request, "test");

        Assert.IsNotNull(result);
        Assert.AreEqual("transformado", result.TransformedResult);
        Assert.AreEqual("test", result.TransformerId);
    }

    [TestMethod]
    public void ExecuteMethodWithTransform_EmptyApiKey_ShouldThrowException()
    {
        var mockExecutionService = new Mock<IExecutionService>();
        var mockSimClassService = new Mock<ISimClassService>();
        var mockTransformerService = new Mock<ITransformerService>();

        var paramTypeId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdInstanceType = instanceId.ToString(),
            IdReferenceType = referenceId.ToString(),
            Parameters =
            [
                new ParameterRequest { Name = "param1", IdClassType = paramTypeId.ToString() }
            ]
        };

        var simClass = new SimClass { Id = paramTypeId, Name = "ParamType", State = SimAccesibility.Normal };
        var simReference = new SimClass { Id = referenceId, Name = "Ref", State = SimAccesibility.Normal };
        var simInstance = new SimClass { Id = instanceId, Name = "Obj", State = SimAccesibility.Normal };

        mockSimClassService.Setup(x => x.GetSimClassById(paramTypeId)).Returns(simClass);
        mockSimClassService.Setup(x => x.GetSimClassById(referenceId)).Returns(simReference);
        mockSimClassService.Setup(x => x.GetSimClassById(instanceId)).Returns(simInstance);

        mockExecutionService.Setup(x => x.IsReferenceBaseOfInstance(simReference, simInstance)).Returns(true);
        mockExecutionService.Setup(x => x.ExecuteMethod(It.IsAny<ReferenceThis>(), It.IsAny<ReferenceThis>(), It.IsAny<Signature>(), It.IsAny<int>(), It.IsAny<HashSet<Guid>>()))
            .Returns("resultado");
        mockExecutionService.Setup(x => x.SaveExecutionLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        var expectedResponse = new TransformedResponse
        {
            OriginalResult = "resultado",
            TransformedResult = "transformado",
            ContentType = "text/plain",
            TransformerId = "test"
        };
        mockTransformerService.Setup(x => x.TransformExecution("resultado", "test")).Returns(expectedResponse);

        var adapter = new ExecutionAdapter(
            mockExecutionService.Object,
            mockSimClassService.Object,
            mockTransformerService.Object);

        var invalidKey = Guid.Empty;
        mockExecutionService.Setup(x => x.IsAuthorizedUser(invalidKey)).Returns(false);

        var ex = Assert.ThrowsException<InvalidApikeyAdapter>(() =>
        {
            adapter.ExecuteMethodWithTransform(invalidKey, request, "test");
        });
        Assert.AreEqual("API Key inválida o ausente", ex.Message);
    }

    [TestMethod]
    public void ExecuteMethodWithTransform_InvalidApiKey_ShouldThrowException()
    {
        var mockExecutionService = new Mock<IExecutionService>();
        var mockSimClassService = new Mock<ISimClassService>();
        var mockTransformerService = new Mock<ITransformerService>();

        var paramTypeId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            IdInstanceType = instanceId.ToString(),
            IdReferenceType = referenceId.ToString(),
            Parameters =
            [
                new ParameterRequest { Name = "param1", IdClassType = paramTypeId.ToString() }
            ]
        };

        var simClass = new SimClass { Id = paramTypeId, Name = "ParamType", State = SimAccesibility.Normal };
        var simReference = new SimClass { Id = referenceId, Name = "Ref", State = SimAccesibility.Normal };
        var simInstance = new SimClass { Id = instanceId, Name = "Obj", State = SimAccesibility.Normal };

        mockSimClassService.Setup(x => x.GetSimClassById(paramTypeId)).Returns(simClass);
        mockSimClassService.Setup(x => x.GetSimClassById(referenceId)).Returns(simReference);
        mockSimClassService.Setup(x => x.GetSimClassById(instanceId)).Returns(simInstance);

        mockExecutionService.Setup(x => x.IsReferenceBaseOfInstance(simReference, simInstance)).Returns(true);
        mockExecutionService.Setup(x => x.ExecuteMethod(It.IsAny<ReferenceThis>(), It.IsAny<ReferenceThis>(), It.IsAny<Signature>(), It.IsAny<int>(), It.IsAny<HashSet<Guid>>()))
            .Returns("resultado");
        mockExecutionService.Setup(x => x.SaveExecutionLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        var expectedResponse = new TransformedResponse
        {
            OriginalResult = "resultado",
            TransformedResult = "transformado",
            ContentType = "text/plain",
            TransformerId = "test"
        };
        mockTransformerService.Setup(x => x.TransformExecution("resultado", "test")).Returns(expectedResponse);

        var adapter = new ExecutionAdapter(
            mockExecutionService.Object,
            mockSimClassService.Object,
            mockTransformerService.Object);

        var invalidKey = Guid.NewGuid();
        mockExecutionService.Setup(x => x.IsAuthorizedUser(invalidKey)).Returns(false);

        var ex = Assert.ThrowsException<InvalidApikeyAdapter>(() =>
        {
            adapter.ExecuteMethodWithTransform(invalidKey, request, "test");
        });
        Assert.AreEqual("API Key inválida o ausente", ex.Message);
    }
}
