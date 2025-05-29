using Adapter;
using Adapter.Exceptions;
using BusinessLogic.Exceptions;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;
using IBusinessLogic;
using Models.Enums;
using Models.Request;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class MethodAdapterTest
{
    private Mock<IMethodService>? _mockMethodService;
    private Mock<ISimClassService>? _mockSimClassService;
    private Mock<ISimAttributeService>? _mockAttributeService;
    private Mock<IExecutionService>? _mockExecutionService;
    private MethodAdapter? adapter;

    [TestInitialize]
    public void Initialize()
    {
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockAttributeService = new Mock<ISimAttributeService>(MockBehavior.Strict);
        _mockExecutionService = new Mock<IExecutionService>(MockBehavior.Loose);
        adapter = new MethodAdapter(_mockMethodService.Object, _mockSimClassService.Object, _mockAttributeService.Object, _mockExecutionService.Object);
    }

    [TestMethod]
    public void CreateParameter_ShouldReturnCreatedParameterResponse_WhenValid()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();
        var parameterName = "ValidParameter";

        var request = new ParameterRequest
        {
            Name = parameterName,
            IdReference = classTypeId.ToString()
        };

        var simClass = new SimClass { Id = classTypeId, Name = "int" };
        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = parameterName,
            Type = simClass,
            RelatedMethod = method
        };

        _mockSimClassService.Setup(s => s.GetSimClassById(classTypeId)).Returns(simClass);
        _mockMethodService.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockMethodService.Setup(s => s.AddMethodParameter(methodId, It.IsAny<Parameter>())).Returns(parameter);

        var result = adapter!.CreateParameter(methodId, request);

        result.Should().NotBeNull();
        result.Message.Should().Be("Parameter created successfully");
        result.Parameter.Name.Should().Be(parameterName);
        result.Parameter.MethodId.Should().Be(methodId);
        result.Parameter.ReferenceId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateParameter_ShouldThrowInvalidAttribute_WhenSimClassInvalidAttributeIsThrown()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();
        var request = new ParameterRequest
        {
            Name = "Invalid",
            IdReference = classTypeId.ToString()
        };

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockSimClassService.Setup(s => s.GetSimClassById(classTypeId)).Throws(new InvalidAttributeDomain("SimClass error"));

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object, _mockAttributeService.Object, _mockExecutionService.Object);

        adapter.CreateParameter(methodId, request);
    }

    [TestMethod]
    public void CreateVariable_ShouldReturnCreatedVariableResponse_WhenValid()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();
        var variableName = "ValidVariable";

        var request = new VariablesRequest
        {
            Name = variableName,
            IdReference = classTypeId.ToString(),
            IdInstance = instanceId.ToString()
        };

        var simClass = new SimClass { Id = classTypeId, Name = "string" };
        var instanceClass = new SimClass { Id = instanceId, Name = "InstanceClass" };
        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var localVar = new LocalVariable
        {
            Id = Guid.NewGuid(),
            Name = variableName,
            Reference = simClass,
            ReferenceId = classTypeId,
            RelatedMethod = method,
            RelatedMethodId = methodId,
            Instance = instanceClass,
            InstanceId = instanceId
        };

        _mockSimClassService.Setup(s => s.GetSimClassById(classTypeId)).Returns(simClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(instanceId)).Returns(instanceClass);
        _mockMethodService.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockMethodService.Setup(s => s.AddLocalVariable(methodId, It.IsAny<LocalVariable>())).Returns(localVar);

        var adapter = new MethodAdapter(_mockMethodService.Object, _mockSimClassService.Object, _mockAttributeService.Object, _mockExecutionService.Object);

        var result = adapter.CreateVariable(methodId, request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Variable created successfully", result.Message);
        Assert.IsNotNull(result.Variable);
        Assert.AreEqual(variableName, result.Variable.Name);
        Assert.AreEqual(methodId, result.Variable.MethodId);
        Assert.AreEqual(classTypeId, result.Variable.ReferenceId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateVariable_ShouldThrowInvalidAttribute_WhenSimClassInvalidAttributeIsThrown()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();
        var request = new VariablesRequest
        {
            Name = " ",
            IdReference = classTypeId.ToString()
        };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(classTypeId))
            .Throws(new InvalidAttributeDomain("SimClass error"));

        adapter!.CreateVariable(methodId, request);
    }

    [TestMethod]
    public void GetParameter_ShouldReturnParameterResponse_WhenValid()
    {
        var parameterId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var simClass = new SimClass { Id = classTypeId, Name = "int" };
        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var parameter = new Parameter
        {
            Id = parameterId,
            Name = "param1",
            Type = simClass,
            TypeId = classTypeId,
            RelatedMethod = method,
            RelatedMethodId = methodId
        };

        _mockMethodService.Setup(s => s.GetParameterById(parameterId)).Returns(parameter);

        var result = adapter.GetParameter(parameterId);

        result.Should().NotBeNull();
        result.Id.Should().Be(parameterId);
        result.Name.Should().Be("param1");
        result.MethodId.Should().Be(methodId);
        result.ReferenceId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void GetParameter_ShouldThrowInvalidOperationException_WhenSimClassInvalidAttributeIsThrown()
    {
        var parameterId = Guid.NewGuid();

        _mockMethodService.Setup(s => s.GetParameterById(parameterId)).Throws(new InvalidAttributeDomain("error"));

        adapter.GetParameter(parameterId);
    }

    [TestMethod]
    public void GetVariable_ShouldReturnVariableResponse_WhenValid()
    {
        var variableId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var simClass = new SimClass { Id = classTypeId, Name = "int" };
        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var variable = new LocalVariable
        {
            Id = variableId,
            Name = "var1",
            Reference = simClass,
            ReferenceId = classTypeId,
            RelatedMethod = method,
            RelatedMethodId = methodId
        };

        _mockMethodService.Setup(s => s.GetVariableById(variableId)).Returns(variable);

        var result = adapter.GetVariable(variableId);

        result.Should().NotBeNull();
        result.Id.Should().Be(variableId);
        result.Name.Should().Be("var1");
        result.MethodId.Should().Be(methodId);
        result.ReferenceId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void GetVariable_ShouldThrowInvalidOperationException_WhenSimClassInvalidAttributeIsThrown()
    {
        var variableId = Guid.NewGuid();

        _mockMethodService.Setup(s => s.GetVariableById(variableId)).Throws(new NonExistentValueLogic("error"));

        adapter.GetVariable(variableId);
    }

    [TestMethod]
    public void GetMethod_ShouldReturnMethodResponse_WhenValid()
    {
        var methodId = Guid.NewGuid();
        var classOwnerId = Guid.NewGuid();
        var returnTypeId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = methodId,
            RelatedClassId = classOwnerId,
            Name = "TestMethod",
            RelatedClass = new SimClass { Id = classOwnerId, Name = "TestClass" },
            Privacity = SimPrivacity.Public,
            Accesibility = SimAccesibility.Normal,
            ReturnType = new SimClass { Id = returnTypeId, Name = "int" },
            ReturnTypeId = returnTypeId,
        };

        _mockMethodService.Setup(s => s.GetMethodById(methodId)).Returns(method);

        var result = adapter.GetMethod(methodId);

        result.Should().NotBeNull();
        result.Id.Should().Be(methodId);
        result.Name.Should().Be("TestMethod");
        result.IdClassOwner.Should().Be(classOwnerId);
        result.Privacity.Should().Be(SimModelsPrivacity.Public);
        result.Accesibility.Should().Be(SimModelsAccesibility.Normal);
        result.ReturnTypeId.Should().Be(returnTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void GetMethod_ShouldThrowNonExistentValueAdapter_WhenSimClassInvalidAttributeIsThrown()
    {
        var methodId = Guid.NewGuid();

        _mockMethodService.Setup(s => s.GetMethodById(methodId)).Throws(new NonExistentValueLogic("error"));

        adapter.GetMethod(methodId);
        _mockMethodService.VerifyAll();
    }

    [TestMethod]
    public void CreateMethod_ShouldReturnCreatedMethodResponse_WhenValid()
    {
        var idClass = Guid.NewGuid();
        var returnTypeId = Guid.NewGuid();
        var methodName = "TestMethod";

        var methodRequest = new MethodRequest
        {
            Name = methodName,
            Privacity = SimModelsPrivacity.Public,
            Accesibility = SimModelsAccesibility.Normal,
            IdReturnType = returnTypeId.ToString()
        };

        var classOwner = new SimClass { Id = idClass, Name = "TestClass" };
        var returnType = new SimClass { Id = returnTypeId, Name = "int" };

        var simMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = methodName,
            RelatedClass = classOwner,
            RelatedClassId = idClass,
            Privacity = SimPrivacity.Public,
            Accesibility = SimAccesibility.Normal,
            ReturnType = returnType,
            ReturnTypeId = returnTypeId
        };

        _mockSimClassService.Setup(s => s.GetSimClassById(idClass)).Returns(classOwner);
        _mockSimClassService.Setup(s => s.GetSimClassById(returnTypeId)).Returns(returnType);
        _mockMethodService.Setup(s => s.AddMethod(idClass, It.IsAny<SimMethod>())).Returns(simMethod);

        var result = adapter.CreateMethod(idClass, methodRequest);

        result.Should().NotBeNull();
        result.Message.Should().Be("Method created successfully");
        result.MethodResponse.Should().NotBeNull();
        result.MethodResponse.Id.Should().Be(simMethod.Id);
        result.MethodResponse.Name.Should().Be(methodName);
        result.MethodResponse.IdClassOwner.Should().Be(idClass);
        result.MethodResponse.Privacity.Should().Be(SimModelsPrivacity.Public);
        result.MethodResponse.Accesibility.Should().Be(SimModelsAccesibility.Normal);
        result.MethodResponse.ReturnTypeId.Should().Be(returnTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateMethod_ShouldThrowInvalidAttribute_WhenSimClassInvalidAttributeIsThrown()
    {
        var idClass = Guid.NewGuid();
        var returnTypeId = Guid.NewGuid();

        var methodRequest = new MethodRequest
        {
            Name = "TestMethod",
            Privacity = SimModelsPrivacity.Public,
            Accesibility = SimModelsAccesibility.Normal,
            IdReturnType = returnTypeId.ToString()
        };

        _mockSimClassService.Setup(s => s.GetSimClassById(idClass)).Throws(new InvalidAttributeDomain("SimClass error"));

        adapter.CreateMethod(idClass, methodRequest);
    }

    [TestMethod]
    public void DeleteMethod_ShouldCallDeleteMethod_WhenValid()
    {
        var methodId = Guid.NewGuid();

        _mockMethodService!.Setup(s => s.DeleteMethod(methodId));

        adapter!.DeleteMethod(methodId);

        _mockMethodService.Verify(s => s.DeleteMethod(methodId), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void DeleteMethod_ShouldThrowNonExistentValueAdapter_WhenNonExistentValueLogic()
    {
        var methodId = Guid.NewGuid();

        _mockMethodService.Setup(s => s.DeleteMethod(methodId)).Throws(new NonExistentValueLogic("error"));

        adapter.DeleteMethod(methodId);

        _mockMethodService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void CreateWrongInvocation_ShouldThrowInvalidOperationException()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();
        var parameterName = "param1";

        var parameterRequest = new ParameterSignatureRequest
        {
            Name = parameterName,
            IdReference = classTypeId.ToString()
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = Guid.NewGuid().ToString(),
            TypeReference = TypeReference.This,
            MethodName = "TestMethod",
            Parameters = [parameterRequest]
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Throws(new NonExistentValueLogic("error"));

        adapter!.CreateInvocation(methodId, invocationRequest);
    }

    [TestMethod]
    public void GetInvocation_ShouldReturnInvocationResponse_WhenValid_New()
    {
        var invocationId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var simClass = new SimClass { Id = classTypeId, Name = "CustomType" };
        var method = new SimMethod { Id = methodId, Name = "CustomMethod" };
        var parameterId = Guid.NewGuid();
        var parameter = new ParameterSignature
        {
            Id = parameterId,
            Name = "customParam",
            Reference = simClass,
            ReferenceId = classTypeId
        };

        var reference = new ReferenceThis() { Reference = simClass };

        var invocation = new Invocation
        {
            Id = invocationId,
            RelatedMethod = method,
            RelatedMethodId = methodId,
            Reference = reference
        };

        invocation.Signature = new Signature
        {
            Id = Guid.NewGuid(),
            Name = "CustomMethod",
            Parameters = [parameter],
            RelatedInvocation = invocation
        };

        _mockMethodService!.Setup(s => s.GetInvocationById(invocationId)).Returns(invocation);

        var result = adapter!.GetInvocation(invocationId);

        _mockMethodService.Verify(s => s.GetInvocationById(invocationId), Times.Once);
        result.Should().NotBeNull();
        result.Id.Should().Be(invocationId);
        result.MethodName.Should().Be("CustomMethod");
        result.IdReference.Should().Be(classTypeId);
        result.Parameters.Should().HaveCount(1);
        result.Parameters[0].Name.Should().Be("customParam");
        result.Parameters[0].ReferenceId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void GetWrongInvocation_ShouldThrowInvalidOperationException()
    {
        var invocationId = Guid.NewGuid();

        _mockMethodService!.Setup(s => s.GetInvocationById(invocationId)).Throws(new NonExistentValueLogic("error"));

        adapter!.GetInvocation(invocationId);
    }

    [TestMethod]
    public void CreateInvocation_WithAttributeReference_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" }, // Añadir RelatedClass
            RelatedClassId = Guid.NewGuid()
        };

        var simClass = new SimClass { Id = classTypeId, Name = "int" };
        var attribute = new SimAttribute
        {
            Id = attributeId,
            Name = "TestAttribute",
            Reference = simClass
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = attributeId.ToString(),
            TypeReference = TypeReference.Attribute,
            MethodName = "AttrMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockAttributeService!.Setup(s => s.GetSimAttribute(attributeId)).Returns(attribute);

        _mockMethodService.Setup(s => s.MethodInheritsAttribute(method, attribute));

        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);

        _mockExecutionService!.Setup(s => s.ValidateMethodExistsInClass(
            It.IsAny<SimClass>(),
            It.Is<Signature>(sig => sig.Name == "AttrMethod"),
            false));

        var result = adapter!.CreateInvocation(methodId, invocationRequest);

        _mockMethodService.VerifyAll();
        _mockSimClassService.VerifyAll();
        _mockAttributeService.VerifyAll();

        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.MethodName.Should().Be("AttrMethod");
    }

    [TestMethod]
    public void CreateInvocation_WithMultipleParameters_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();
        var intTypeId = Guid.NewGuid();
        var stringTypeId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod", RelatedClassId = referenceId };
        var intType = new SimClass { Id = intTypeId, Name = "int" };
        var stringType = new SimClass { Id = stringTypeId, Name = "string" };
        var referenceClass = new SimClass { Id = referenceId, Name = "TestClass" };

        var parameters = new List<ParameterSignatureRequest>
    {
        new ParameterSignatureRequest { Name = "param1", IdReference = intTypeId.ToString() },
        new ParameterSignatureRequest { Name = "param2",  IdReference = stringTypeId.ToString() }
    };

        var invocationRequest = new InvocationRequest
        {
            IdReference = referenceId.ToString(),
            TypeReference = TypeReference.This,
            MethodName = "MultiParamMethod",
            Parameters = parameters
        };

        _mockMethodService.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockSimClassService.Setup(s => s.GetSimClassById(referenceId)).Returns(referenceClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(intTypeId)).Returns(intType);
        _mockSimClassService.Setup(s => s.GetSimClassById(stringTypeId)).Returns(stringType);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);

        _mockExecutionService.Setup(s => s.ValidateMethodExistsInClass(
            It.Is<SimClass>(c => c.Id == referenceClass.Id),
            It.Is<Signature>(sig =>
                sig.Name == "MultiParamMethod" &&
                sig.Parameters.Count == 2 &&
                sig.Parameters[0].Name == "param1" &&
                sig.Parameters[1].Name == "param2"), true));

        var result = adapter.CreateInvocation(methodId, invocationRequest);

        _mockMethodService.VerifyAll();
        _mockSimClassService.VerifyAll();
        _mockExecutionService.Verify(s => s.ValidateMethodExistsInClass(
            It.Is<SimClass>(c => c.Id == referenceClass.Id),
            It.Is<Signature>(sig => sig.Name == "MultiParamMethod" && sig.Parameters.Count == 2), false));
        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.MethodName.Should().Be("MultiParamMethod");
        result.InvocationResponse.Parameters.Should().HaveCount(2);
        result.InvocationResponse.Parameters[0].Name.Should().Be("param1");
        result.InvocationResponse.Parameters[1].Name.Should().Be("param2");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateInvocation_WithInvalidTypeReference_ShouldThrowException()
    {
        var methodId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };

        var invocationRequest = new InvocationRequest
        {
            IdReference = referenceId.ToString(),
            TypeReference = (TypeReference)999,
            MethodName = "InvalidMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);

        adapter!.CreateInvocation(methodId, invocationRequest);
        _mockMethodService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void CreateInvocation_WithInvalidSimClassService_ShouldThrowException()
    {
        var methodId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };

        var invocationRequest = new InvocationRequest
        {
            IdReference = referenceId.ToString(),
            TypeReference = TypeReference.This,
            MethodName = "FailMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockSimClassService!.Setup(s => s.GetSimClassById(referenceId))
            .Throws(new NonExistentValueLogic("SimClass service error"));

        adapter!.CreateInvocation(methodId, invocationRequest);

        _mockMethodService.VerifyAll();
        _mockSimClassService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void CreateInvocation_WithInvalidAttributeService_ShouldThrowException()
    {
        var methodId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };

        var invocationRequest = new InvocationRequest
        {
            IdReference = attributeId.ToString(),
            TypeReference = TypeReference.Attribute,
            MethodName = "FailMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockAttributeService!.Setup(s => s.GetSimAttribute(attributeId))
            .Throws(new NonExistentValueLogic("Attribute service error"));

        adapter!.CreateInvocation(methodId, invocationRequest);
        _mockMethodService.VerifyAll();
        _mockAttributeService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateInvocation_ShouldThrowInvalidAttributeAdapter_WhenInvalidAttributeLogicIsThrown()
    {
        var methodId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();

        var invocationRequest = new InvocationRequest
        {
            IdReference = referenceId.ToString(),
            TypeReference = TypeReference.Attribute,
            MethodName = "InvalidMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(new SimMethod { Id = methodId, Name = "TestMethod" });
        _mockAttributeService!.Setup(s => s.GetSimAttribute(referenceId))
            .Throws(new InvalidAttributeLogic("Invalid attribute error"));

        adapter!.CreateInvocation(methodId, invocationRequest);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateInvocation_ShouldThrowInvalidAttributeAdapter_WhenInvalidAttributeDomainIsThrown()
    {
        var methodId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();

        var invocationRequest = new InvocationRequest
        {
            IdReference = referenceId.ToString(),
            TypeReference = TypeReference.Base,
            MethodName = "InvalidMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(new SimMethod { Id = methodId, Name = "TestMethod" });
        _mockSimClassService!.Setup(s => s.GetSimClassById(referenceId))
            .Throws(new InvalidAttributeDomain("Invalid attribute domain error"));

        adapter!.CreateInvocation(methodId, invocationRequest);
        _mockMethodService.VerifyAll();
        _mockSimClassService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateParameter_ShouldThrowInvalidAttributeAdapter_WhenInvalidAttributeLogicIsThrown()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var parameterRequest = new ParameterRequest
        {
            Name = "InvalidParameter",
            IdReference = classTypeId.ToString()
        };

        _mockSimClassService!.Setup(s => s.GetSimClassById(classTypeId))
            .Throws(new InvalidAttributeLogic("Invalid attribute logic error"));

        adapter!.CreateParameter(methodId, parameterRequest);
        _mockSimClassService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void CreateParameter_ShouldThrowNonExistentValueAdapter_WhenNonExistentValueLogicIsThrown()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var parameterRequest = new ParameterRequest
        {
            Name = "NonExistentParameter",
            IdReference = classTypeId.ToString()
        };

        _mockSimClassService!.Setup(s => s.GetSimClassById(classTypeId))
            .Throws(new NonExistentValueLogic("Class type does not exist"));

        adapter!.CreateParameter(methodId, parameterRequest);
        _mockSimClassService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void GetParameter_ShouldThrowNonExistentValueAdapter_WhenNonExistentValueLogicIsThrown()
    {
        var parameterId = Guid.NewGuid();

        _mockMethodService!.Setup(s => s.GetParameterById(parameterId))
            .Throws(new NonExistentValueLogic("Parameter does not exist"));

        adapter!.GetParameter(parameterId);

        _mockMethodService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateVariable_ShouldThrowInvalidAttributeAdapter_WhenInvalidAttributeLogicIsThrown()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var variableRequest = new VariablesRequest
        {
            Name = "InvalidVariable",
            IdReference = classTypeId.ToString()
        };

        _mockSimClassService!.Setup(s => s.GetSimClassById(classTypeId))
            .Throws(new InvalidAttributeLogic("Invalid attribute logic error"));

        adapter!.CreateVariable(methodId, variableRequest);
        _mockSimClassService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void CreateVariable_ShouldThrowNonExistentValueAdapter_WhenNonExistentValueLogicIsThrown()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var variableRequest = new VariablesRequest
        {
            Name = "NonExistentVariable",
            IdReference = classTypeId.ToString()
        };

        _mockSimClassService!.Setup(s => s.GetSimClassById(classTypeId))
            .Throws(new NonExistentValueLogic("Class type does not exist"));

        adapter!.CreateVariable(methodId, variableRequest);

        _mockSimClassService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateMethod_ShouldThrowInvalidAttributeAdapter_WhenInvalidAttributeLogicIsThrown()
    {
        var idClass = Guid.NewGuid();
        var returnTypeId = Guid.NewGuid();

        var methodRequest = new MethodRequest
        {
            Name = "InvalidMethod",
            Privacity = SimModelsPrivacity.Public,
            Accesibility = SimModelsAccesibility.Normal,
            IdReturnType = returnTypeId.ToString()
        };

        _mockSimClassService!.Setup(s => s.GetSimClassById(idClass))
            .Returns(new SimClass { Id = idClass, Name = "TestClass" });

        _mockSimClassService.Setup(s => s.GetSimClassById(returnTypeId))
            .Throws(new InvalidAttributeLogic("Invalid attribute logic error"));

        adapter!.CreateMethod(idClass, methodRequest);
        _mockSimClassService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void CreateMethod_ShouldThrowNonExistentValueAdapter_WhenNonExistentValueLogicIsThrown()
    {
        var idClass = Guid.NewGuid();
        var returnTypeId = Guid.NewGuid();

        var methodRequest = new MethodRequest
        {
            Name = "NonExistentMethod",
            Privacity = SimModelsPrivacity.Public,
            Accesibility = SimModelsAccesibility.Normal,
            IdReturnType = returnTypeId.ToString()
        };

        _mockSimClassService!.Setup(s => s.GetSimClassById(idClass))
            .Throws(new NonExistentValueLogic("Class does not exist"));

        adapter!.CreateMethod(idClass, methodRequest);
        _mockSimClassService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InUseValueAdapter))]
    public void DeleteMethod_ShouldThrowInUseValueAdapter_WhenInUseValueLogicIsThrown()
    {
        var methodId = Guid.NewGuid();

        _mockMethodService!
            .Setup(s => s.DeleteMethod(methodId))
            .Throws(new InUseValueLogic("Method is in use"));

        adapter!.DeleteMethod(methodId);
        _mockMethodService.VerifyAll();
    }

    [TestMethod]
    public void CreateInvocation_WithThisReference_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId
        };
        var simClass = new SimClass { Id = classId, Name = "TestClass" };

        var invocationRequest = new InvocationRequest
        {
            IdReference = classId.ToString(),
            TypeReference = TypeReference.This,
            MethodName = "ThisMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockSimClassService!.Setup(s => s.GetSimClassById(classId)).Returns(simClass);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockExecutionService!
            .Setup(s => s.ValidateMethodExistsInClass(
                It.Is<SimClass>(c => c.Id == classId),
                It.Is<Signature>(sig => sig.Name == "ThisMethod"), false));

        var result = adapter!.CreateInvocation(methodId, invocationRequest);
        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.MethodName.Should().Be("ThisMethod");
        _mockMethodService.VerifyAll();
        _mockSimClassService.VerifyAll();
        _mockExecutionService.Verify(s => s.ValidateMethodExistsInClass(
            It.Is<SimClass>(c => c.Id == classId),
            It.Is<Signature>(sig => sig.Name == "ThisMethod"), false), Times.Once);
    }

    [TestMethod]
    public void CreateInvocation_WithBaseReference_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId
        };

        var baseClass = new SimClass
        {
            Id = baseClassId,
            Name = "BaseClass"
        };

        var simClass = new SimClass
        {
            Id = classId,
            Name = "TestClass",
            BaseClassId = baseClassId,
            BaseClass = baseClass
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = classId.ToString(),
            TypeReference = TypeReference.Base,
            MethodName = "BaseMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockSimClassService!.Setup(s => s.GetSimClassById(classId)).Returns(simClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(baseClassId)).Returns(baseClass);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);

        _mockExecutionService!
            .Setup(s => s.ValidateMethodExistsInClass(
                It.Is<SimClass>(c => c.Id == baseClassId),
                It.Is<Signature>(sig => sig.Name == "BaseMethod"), true));
        var result = adapter!.CreateInvocation(methodId, invocationRequest);

        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.MethodName.Should().Be("BaseMethod");
    }

    [TestMethod]
    public void CreateInvocation_WithParameterReference_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var parameterId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var parameterType = new SimClass { Id = typeId, Name = "int" };
        var parameter = new Parameter
        {
            Id = parameterId,
            Name = "testParam",
            Type = parameterType,
            RelatedMethodId = methodId
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = parameterId.ToString(),
            TypeReference = TypeReference.Parameter,
            MethodName = "ParamMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockMethodService.Setup(s => s.GetParameterById(parameterId)).Returns(parameter);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockExecutionService!
            .Setup(s => s.ValidateMethodExistsInClass(
                It.Is<SimClass>(c => c.Id == typeId),
                It.Is<Signature>(sig => sig.Name == "ParamMethod"), false));
        var result = adapter!.CreateInvocation(methodId, invocationRequest);
        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.MethodName.Should().Be("ParamMethod");
    }

    [TestMethod]
    public void CreateInvocation_WithLocalVariableReference_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var variableId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var variableType = new SimClass { Id = typeId, Name = "string" };
        var variable = new LocalVariable
        {
            Id = variableId,
            Name = "testVar",
            Reference = variableType,
            RelatedMethodId = methodId
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = variableId.ToString(),
            TypeReference = TypeReference.LocalVariable,
            MethodName = "VarMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockMethodService.Setup(s => s.GetVariableById(variableId)).Returns(variable);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockExecutionService!
            .Setup(s => s.ValidateMethodExistsInClass(
                It.Is<SimClass>(c => c.Id == typeId),
                It.Is<Signature>(sig => sig.Name == "VarMethod"), true));
        var result = adapter!.CreateInvocation(methodId, invocationRequest);
        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.MethodName.Should().Be("VarMethod");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateInvocation_WithParameterReference_ShouldThrowWhenMethodIdMismatch()
    {
        var methodId = Guid.NewGuid();
        var differentMethodId = Guid.NewGuid();
        var parameterId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var parameterType = new SimClass { Id = typeId, Name = "int" };
        var parameter = new Parameter
        {
            Id = parameterId,
            Name = "testParam",
            Type = parameterType,
            RelatedMethodId = differentMethodId
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = parameterId.ToString(),
            TypeReference = TypeReference.Parameter,
            MethodName = "ParamMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockMethodService.Setup(s => s.GetParameterById(parameterId)).Returns(parameter);

        adapter!.CreateInvocation(methodId, invocationRequest);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateInvocation_WithVariableReference_ShouldThrowWhenMethodIdMismatch()
    {
        var methodId = Guid.NewGuid();
        var differentMethodId = Guid.NewGuid();
        var variableId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var variableType = new SimClass { Id = typeId, Name = "string" };
        var variable = new LocalVariable
        {
            Id = variableId,
            Name = "testVar",
            Reference = variableType,
            RelatedMethodId = differentMethodId
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = variableId.ToString(),
            TypeReference = TypeReference.LocalVariable,
            MethodName = "VarMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockMethodService.Setup(s => s.GetVariableById(variableId)).Returns(variable);

        adapter!.CreateInvocation(methodId, invocationRequest);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateInvocation_ShouldThrowInvalidAttributeAdapter_WhenValidateMethodExistsInClassFails()
    {
        var methodId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId
        };
        var simClass = new SimClass { Id = classId, Name = "TestClass" };

        var invocationRequest = new InvocationRequest
        {
            IdReference = classId.ToString(),
            TypeReference = TypeReference.This,
            MethodName = "NonExistentMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockSimClassService!.Setup(s => s.GetSimClassById(classId)).Returns(simClass);

        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);

        _mockExecutionService!
            .Setup(s => s.ValidateMethodExistsInClass(
                It.Is<SimClass>(c => c.Id == classId),
                It.Is<Signature>(sig => sig.Name == "NonExistentMethod"), false))
            .Throws(new InvalidAttributeLogic("Method does not exist in class"));

        adapter!.CreateInvocation(methodId, invocationRequest);
    }

    [TestMethod]
    public void CreateInvocation_WithClassInheritAttribute_ShouldCallClassInheritAttribute()
    {
        var methodId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var attributeTypeId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId,
            Accesibility = SimAccesibility.Normal,
            Privacity = SimPrivacity.Public,
            ReturnTypeId = attributeTypeId,
            ReturnType = new SimClass { Id = attributeTypeId, Name = "ReturnType" }
        };
        var attributeType = new SimClass { Id = attributeTypeId, Name = "AttributeType" };
        var attribute = new SimAttribute
        {
            Id = attributeId,
            Name = "TestAttr",
            Reference = attributeType
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = attributeId.ToString(),
            TypeReference = TypeReference.Attribute,
            MethodName = "AttributeMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockAttributeService!.Setup(s => s.GetSimAttribute(attributeId)).Returns(attribute);
        _mockSimClassService!.Setup(s => s.ClassInheritAttribute(classId, attributeId));

        _mockMethodService.Setup(s => s.MethodInheritsAttribute(method, attribute));

        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockExecutionService
            .Setup(s => s.ValidateMethodExistsInClass(
                It.Is<SimClass>(c => c.Id == attributeTypeId),
                It.Is<Signature>(sig => sig.Name == "AttributeMethod"), false));

        var result = adapter!.CreateInvocation(methodId, invocationRequest);
        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        _mockMethodService.Verify(s => s.MethodInheritsAttribute(method, attribute), Times.Once);
    }

    [TestMethod]
    public void CreateInvocation_WithStaticClassReference_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var staticClassId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId
        };

        var staticClass = new SimClass
        {
            Id = staticClassId,
            Name = "StaticClass"
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = staticClassId.ToString(),
            TypeReference = TypeReference.Static,
            MethodName = "StaticMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockSimClassService!.Setup(s => s.GetSimClassById(staticClassId)).Returns(staticClass);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockMethodService
            .Setup(s => s.SignatureStaticExistsInClass(
                It.Is<SimClass>(c => c.Id == staticClassId),
                methodId,
                It.Is<Signature>(sig => sig.Name == "StaticMethod")));

        var result = adapter!.CreateInvocation(methodId, invocationRequest);

        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.MethodName.Should().Be("StaticMethod");
        result.InvocationResponse.TypeReference.Should().Be("Static");

        _mockMethodService.Verify(s => s.SignatureStaticExistsInClass(
            It.Is<SimClass>(c => c.Id == staticClassId),
            methodId,
            It.Is<Signature>(sig => sig.Name == "StaticMethod")),
            Times.Once);

        _mockMethodService.Verify(s => s.AddInvocation(methodId, It.IsAny<Invocation>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void CreateInvocation_WithStaticClassReference_ShouldThrowWhenMethodDoesNotExist()
    {
        var methodId = Guid.NewGuid();
        var staticClassId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var staticClass = new SimClass { Id = staticClassId, Name = "StaticClass" };

        var invocationRequest = new InvocationRequest
        {
            IdReference = staticClassId.ToString(),
            TypeReference = TypeReference.Static,
            MethodName = "NonExistentStaticMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockSimClassService!.Setup(s => s.GetSimClassById(staticClassId)).Returns(staticClass);

        _mockMethodService
            .Setup(s => s.SignatureStaticExistsInClass(
                It.Is<SimClass>(c => c.Id == staticClassId),
                methodId,
                It.Is<Signature>(sig => sig.Name == "NonExistentStaticMethod")))
            .Throws(new NonExistentValueLogic("Static method does not exist in class"));

        adapter!.CreateInvocation(methodId, invocationRequest);
    }

    [TestMethod]
    public void CreateInvocation_WithStaticAttributeReference_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var staticAttributeId = Guid.NewGuid();
        var attributeTypeId = Guid.NewGuid();

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId
        };

        var attributeType = new SimClass { Id = attributeTypeId, Name = "AttributeType" };

        var staticAttribute = new SimAttribute
        {
            Id = staticAttributeId,
            Name = "StaticTestAttr",
            Reference = attributeType,
            IsStatic = true
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = staticAttributeId.ToString(),
            TypeReference = TypeReference.StaticAttribute,
            MethodName = "StaticAttributeMethod",
            Parameters = []
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockAttributeService!.Setup(s => s.GetSimAttribute(staticAttributeId)).Returns(staticAttribute);
        _mockMethodService.Setup(s => s.ValidateStaticAttributeAccessibility(staticAttribute, methodId));
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockExecutionService!
            .Setup(s => s.ValidateMethodExistsInClass(
                It.Is<SimClass>(c => c.Id == attributeTypeId),
                It.Is<Signature>(sig => sig.Name == "StaticAttributeMethod"),
                false));

        var result = adapter!.CreateInvocation(methodId, invocationRequest);

        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.MethodName.Should().Be("StaticAttributeMethod");
        result.InvocationResponse.TypeReference.Should().Be("StaticAttribute");

        _mockMethodService.Verify(s => s.ValidateStaticAttributeAccessibility(
            It.Is<SimAttribute>(a => a.Id == staticAttributeId),
            methodId), Times.Once);

        _mockExecutionService.Verify(s => s.ValidateMethodExistsInClass(
            It.Is<SimClass>(c => c.Id == attributeTypeId),
            It.Is<Signature>(sig => sig.Name == "StaticAttributeMethod"),
            false), Times.Once);

        _mockMethodService.Verify(s => s.AddInvocation(methodId, It.IsAny<Invocation>()), Times.Once);
    }
}
