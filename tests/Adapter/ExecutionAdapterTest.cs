using Adapter;
using Adapter.Exceptions;
using Domain;
using IBussinesLogic;
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
        // Arrange
        var instanceTypeId = Guid.NewGuid();
        var referenceTypeId = Guid.NewGuid();
        var param1TypeId = Guid.NewGuid();
        var param2TypeId = Guid.NewGuid();

        var request = new MethodExecutionRequest
        {
            MethodName = "TestMethod",
            InstanceTypeId = instanceTypeId,
            ReferenceTypeId = referenceTypeId,
            InstanceName = "Instance1",
            Parameters =
            [
            new ParameterRequest { Name = "param1", ClassTypeId = param1TypeId },
                new ParameterRequest { Name = "param2", ClassTypeId = param2TypeId }
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
            InstanceTypeId = Guid.NewGuid(),
            ReferenceTypeId = Guid.NewGuid(),
            InstanceName = "Instance1",
            Parameters =
        [
            new ParameterRequest { Name = "param1", ClassTypeId = Guid.NewGuid() }
        ]
        };

        _simClassService!
            .Setup(s => s.GetSimClassById(It.IsAny<Guid>()))
            .Throws(new Exception("SimClass error"));

        Assert.ThrowsException<InvalidExecutionException>(() =>
        {
            _executionAdapter!.ExecuteMethod(request);
        });
    }
}
