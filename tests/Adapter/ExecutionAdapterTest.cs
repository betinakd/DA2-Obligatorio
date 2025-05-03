using Adapter;
using Adapter.Exceptions;
using BussinesLogic.Exceptions;
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
            IdInstanceType = instanceTypeId.ToString(),
            IdReferenceType = referenceTypeId.ToString(),
            Parameters =
            [
            new ParameterRequest { Name = "param1", IdClassType = param1TypeId.ToString() },
                new ParameterRequest { Name = "param2", IdClassType = param2TypeId.ToString() }
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
            new ParameterRequest { Name = "param1", IdClassType = Guid.NewGuid().ToString() }
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
}
