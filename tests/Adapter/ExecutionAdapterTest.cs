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

        var simClass1 = new SimClass { Id = param1TypeId, Name = "Type1" };
        var simClass2 = new SimClass { Id = param2TypeId, Name = "Type2" };

        _simClassService!
            .Setup(s => s.GetSimClassById(param1TypeId)).Returns(simClass1);
        _simClassService
            .Setup(s => s.GetSimClassById(param2TypeId)).Returns(simClass2);

        _mockExecutionService!
            .Setup(s => s.ExecuteMethod(
                "TestMethod",
                It.Is<List<Parameter>>(l =>
                    l.Count == 2 &&
                    l[0].Name == "param1" && l[0].Type == simClass1 &&
                    l[1].Name == "param2" && l[1].Type == simClass2),
                instanceTypeId, referenceTypeId, "Instance1"))
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
