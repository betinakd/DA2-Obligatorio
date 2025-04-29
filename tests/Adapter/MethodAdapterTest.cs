using Adapter;
using Adapter.Exceptions;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;
using IBussinesLogic;
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
            MethodId = methodId,
            Name = parameterName,
            ClassTypeId = classTypeId
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
        result.Parameter.ClassTypeId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateParameter_ShouldThrowInvalidAttribute_WhenSimClassInvalidAttributeIsThrown()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();
        var request = new ParameterRequest
        {
            MethodId = methodId,
            Name = "Invalid",
            ClassTypeId = classTypeId
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
        var variableName = "ValidVariable";

        var request = new VariableRequest
        {
            MethodId = methodId,
            Name = variableName,
            ClassTypeId = classTypeId
        };

        var simClass = new SimClass { Id = classTypeId, Name = "string" };
        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var localVariable = new LocalVariable
        {
            Id = Guid.NewGuid(),
            Name = variableName,
            Type = simClass,
            RelatedMethod = method
        };
        var localVar = new LocalVariable
        {
            Id = localVariable.Id,
            Name = variableName,
            Type = simClass,
            RelatedMethod = method
        };

        _mockSimClassService.Setup(s => s.GetSimClassById(classTypeId)).Returns(simClass);
        _mockMethodService.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockMethodService.Setup(s => s.AddLocalVariable(methodId, It.IsAny<LocalVariable>())).Returns(localVar);

        var adapter = new MethodAdapter(_mockMethodService.Object, _mockSimClassService.Object, _mockAttributeService.Object, _mockExecutionService.Object);

        var result = adapter.CreateVariable(methodId, request);

        result.Should().NotBeNull();
        result.Message.Should().Be("Variable created successfully");
        result.Variable.Name.Should().Be(variableName);
        result.Variable.MethodId.Should().Be(methodId);
        result.Variable.ClassTypeId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void CreateVariable_ShouldThrowInvalidAttribute_WhenSimClassInvalidAttributeIsThrown()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();
        var request = new VariableRequest
        {
            MethodId = methodId,
            Name = " ",
            ClassTypeId = classTypeId
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
            RelatedMethod = method
        };

        _mockMethodService.Setup(s => s.GetParameterById(parameterId)).Returns(parameter);

        var result = adapter.GetParameter(parameterId);

        result.Should().NotBeNull();
        result.Id.Should().Be(parameterId);
        result.Name.Should().Be("param1");
        result.MethodId.Should().Be(methodId);
        result.ClassTypeId.Should().Be(classTypeId);
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
            Type = simClass,
            RelatedMethod = method
        };

        _mockMethodService.Setup(s => s.GetVariableById(variableId)).Returns(variable);

        var result = adapter.GetVariable(variableId);

        result.Should().NotBeNull();
        result.Id.Should().Be(variableId);
        result.Name.Should().Be("var1");
        result.MethodId.Should().Be(methodId);
        result.ClassTypeId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void GetVariable_ShouldThrowInvalidOperationException_WhenSimClassInvalidAttributeIsThrown()
    {
        var variableId = Guid.NewGuid();

        _mockMethodService.Setup(s => s.GetVariableById(variableId)).Throws(new InvalidAttributeDomain("error"));

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
            Name = "TestMethod",
            RelatedClass = new SimClass { Id = classOwnerId, Name = "TestClass" },
            Privacity = SimPrivacity.Public,
            Accesibility = SimAccesibility.Normal,
            ReturnType = new SimClass { Id = returnTypeId, Name = "int" }
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
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void GetMethod_ShouldThrowInvalidOperationException_WhenSimClassInvalidAttributeIsThrown()
    {
        var methodId = Guid.NewGuid();

        _mockMethodService.Setup(s => s.GetMethodById(methodId)).Throws(new InvalidAttributeDomain("error"));

        adapter.GetMethod(methodId);
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
            ReturnTypeId = returnTypeId
        };

        var classOwner = new SimClass { Id = idClass, Name = "TestClass" };
        var returnType = new SimClass { Id = returnTypeId, Name = "int" };

        var simMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = methodName,
            RelatedClass = classOwner,
            Privacity = SimPrivacity.Public,
            Accesibility = SimAccesibility.Normal,
            ReturnType = returnType
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
            ReturnTypeId = returnTypeId
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
    [ExpectedException(typeof(InvalidAttributeAdapter))]
    public void DeleteMethod_ShouldThrowInvalidOperationException_WhenSimClassInvalidAttributeIsThrown()
    {
        var methodId = Guid.NewGuid();

        _mockMethodService.Setup(s => s.DeleteMethod(methodId)).Throws(new InvalidAttributeDomain("error"));

        adapter.DeleteMethod(methodId);
    }

    [TestMethod]
    public void CreateInvocation_ShouldReturnCreatedInvocationResponse_WhenValid()
    {
        var methodId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();
        var parameterName = "param1";

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var simClass = new SimClass { Id = classTypeId, Name = "int" };
        var referenceClass = new SimClass { Id = referenceId, Name = "TestReferenceClass" };

        var parameterRequest = new ParameterRequest
        {
            Name = parameterName,
            ClassTypeId = classTypeId
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = referenceId,
            TypeReference = TypeReference.This,
            MethodName = "TestMethod",
            Parameters = [parameterRequest]
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockSimClassService!.Setup(s => s.GetSimClassById(classTypeId)).Returns(simClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(referenceId)).Returns(referenceClass);

        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockMethodService.Setup(s => s.GetParameterById(It.IsAny<Guid>())).Returns(new Parameter());
        _mockMethodService.Setup(s => s.GetVariableById(It.IsAny<Guid>())).Returns(new LocalVariable());
        _mockAttributeService!.Setup(s => s.GetSimAttribute(It.IsAny<Guid>())).Returns(new SimAttribute());

        _mockExecutionService!
            .Setup(s => s.ValidateMethodExistsInClass(It.IsAny<SimClass>(), It.IsAny<Signature>()));

        var result = adapter!.CreateInvocation(methodId, invocationRequest);

        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.Should().NotBeNull();
        result.InvocationResponse.MethodName.Should().Be("TestMethod");
        result.InvocationResponse.Parameters.Should().HaveCount(1);
        result.InvocationResponse.Parameters[0].Name.Should().Be(parameterName);
        result.InvocationResponse.Parameters[0].ClassTypeId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void CreateWrongInvocation_ShouldThrowInvalidOperationException()
    {
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();
        var parameterName = "param1";

        var parameterRequest = new ParameterRequest
        {
            Name = parameterName,
            ClassTypeId = classTypeId
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = Guid.NewGuid(),
            TypeReference = TypeReference.This,
            MethodName = "TestMethod",
            Parameters = [parameterRequest]
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Throws(new NonExistentValueLogic("error"));

        adapter!.CreateInvocation(methodId, invocationRequest);
    }

    [TestMethod]
    public void GetInvocation_ShouldReturnInvocationResponse_WhenValid()
    {
        var invocationId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var simClass = new SimClass { Id = classTypeId, Name = "int" };
        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var parameter = new ParameterSignature
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Type = simClass,
            TypeId = classTypeId
        };

        var reference = new ReferenceThis() { Reference = simClass };

        var invocation = new Invocation
        {
            Id = invocationId,
            RelatedMethod = method,
            RelatedMethodId = methodId,
            Reference = reference
        };
        var signature = new Signature() { Name = "TestMethod", Parameters = [parameter], Id = Guid.NewGuid(), RelatedInvocationId = invocationId, RelatedInvocation = invocation };
        invocation.Signature = signature;

        _mockMethodService!.Setup(s => s.GetInvocationById(invocationId)).Returns(invocation);

        var result = adapter!.GetInvocation(invocationId);

        _mockMethodService.Verify(s => s.GetInvocationById(invocationId), Times.Once);
        result.Should().NotBeNull();
        result.Id.Should().Be(invocationId);
        result.MethodName.Should().Be("TestMethod");

        result.IdReference.Should().Be(classTypeId);

        result.Parameters.Should().HaveCount(1);
        result.Parameters[0].Id.Should().Be(parameter.Id);
        result.Parameters[0].Name.Should().Be("param1");
        result.Parameters[0].ClassTypeId.Should().Be(classTypeId);
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
    public void CreateInvocation_WithBaseReference_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var simClass = new SimClass { Id = classTypeId, Name = "int" };
        var baseClass = new SimClass { BaseClass = simClass, Id = referenceId, Name = "BaseClass" };
        var method = new SimMethod { Id = methodId, Name = "BaseMethod", Parameters = [], RelatedClass = simClass };
        simClass.Methods.Add(method);

        var invocationRequest = new InvocationRequest
        {
            IdReference = referenceId,
            TypeReference = TypeReference.Base,
            MethodName = "BaseMethod",
            Parameters = []
        };

        var signature = new Signature
        {
            Name = "BaseMethod",
            Parameters = [],
            Id = Guid.NewGuid(),
            RelatedInvocationId = Guid.NewGuid()
        };

        _mockSimClassService!.Setup(s => s.GetSimClassById(referenceId)).Returns(baseClass);
        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockExecutionService
            .Setup(s => s.ValidateMethodExistsInClass(It.Is<SimClass>(c => c == baseClass), It.Is<Signature>(s => s == signature)))
            .Verifiable();

        var result = adapter!.CreateInvocation(methodId, invocationRequest);

        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.Should().NotBeNull();
        result.InvocationResponse.MethodName.Should().Be("BaseMethod");
        result.InvocationResponse.Parameters.Should().BeEmpty();

        _mockMethodService.VerifyAll();
        _mockSimClassService.VerifyAll();
        _mockAttributeService.VerifyAll();
    }

    [TestMethod]
    public void CreateInvocation_WithAttributeReference_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var simClass = new SimClass { Id = classTypeId, Name = "int" };
        var attribute = new SimAttribute
        {
            Id = attributeId,
            Name = "TestAttribute",
            Type = simClass
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = attributeId,
            TypeReference = TypeReference.Attribute,
            MethodName = "AttrMethod",
            Parameters = []
        };
        var signature = new Signature() { Name = "BaseMethod", Parameters = [], Id = Guid.NewGuid(), RelatedInvocationId = Guid.NewGuid() };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockAttributeService!.Setup(s => s.GetSimAttribute(attributeId)).Returns(attribute);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockExecutionService.Setup(s => s.ValidateMethodExistsInClass(simClass, signature));

        var result = adapter!.CreateInvocation(methodId, invocationRequest);

        _mockMethodService.VerifyAll();
        _mockSimClassService.VerifyAll();
        _mockAttributeService.VerifyAll();

        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.MethodName.Should().Be("AttrMethod");
    }

    [TestMethod]
    public void CreateInvocation_WithParameterReference_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var parameterId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var simClass = new SimClass { Id = classTypeId, Name = "int" };
        var parameter = new Parameter
        {
            Id = parameterId,
            Name = "TestParam",
            Type = simClass
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = parameterId,
            TypeReference = TypeReference.Parameter,
            MethodName = "ParamMethod",
            Parameters = []
        };
        var signature = new Signature() { Name = "BaseMethod", Parameters = [], Id = Guid.NewGuid(), RelatedInvocationId = Guid.NewGuid() };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockMethodService.Setup(s => s.GetParameterById(parameterId)).Returns(parameter);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockExecutionService.Setup(s => s.ValidateMethodExistsInClass(simClass, signature));

        var result = adapter!.CreateInvocation(methodId, invocationRequest);

        _mockMethodService.VerifyAll();
        _mockSimClassService.VerifyAll();
        _mockAttributeService.VerifyAll();

        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.MethodName.Should().Be("ParamMethod");
    }

    [TestMethod]
    public void CreateInvocation_WithLocalVariableReference_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var variableId = Guid.NewGuid();
        var classTypeId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var simClass = new SimClass { Id = classTypeId, Name = "string" };
        var variable = new LocalVariable
        {
            Id = variableId,
            Name = "TestVar",
            Type = simClass
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = variableId,
            TypeReference = TypeReference.LocalVariable,
            MethodName = "VarMethod",
            Parameters = []
        };

        var signature = new Signature
        {
            Name = "VarMethod",
            Parameters = [],
            Id = Guid.NewGuid(),
            RelatedInvocationId = Guid.NewGuid()
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockMethodService.Setup(s => s.GetVariableById(variableId)).Returns(variable);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockExecutionService
            .Setup(s => s.ValidateMethodExistsInClass(It.Is<SimClass>(c => c.Id == simClass.Id), It.IsAny<Signature>()))
            .Verifiable();

        var result = adapter!.CreateInvocation(methodId, invocationRequest);

        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.Should().NotBeNull();
        result.InvocationResponse.MethodName.Should().Be("VarMethod");

        _mockMethodService.VerifyAll();
        _mockExecutionService.VerifyAll();
    }

    [TestMethod]
    public void CreateInvocation_WithMultipleParameters_ShouldReturnCreatedInvocationResponse()
    {
        var methodId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();
        var intTypeId = Guid.NewGuid();
        var stringTypeId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var intType = new SimClass { Id = intTypeId, Name = "int" };
        var stringType = new SimClass { Id = stringTypeId, Name = "string" };
        var referenceClass = new SimClass { Id = referenceId, Name = "TestClass" };

        var parameters = new List<ParameterRequest>
    {
        new ParameterRequest { Name = "param1", ClassTypeId = intTypeId },
        new ParameterRequest { Name = "param2", ClassTypeId = stringTypeId }
    };

        var invocationRequest = new InvocationRequest
        {
            IdReference = referenceId,
            TypeReference = TypeReference.This,
            MethodName = "MultiParamMethod",
            Parameters = parameters
        };
        var signature = new Signature() { Name = "BaseMethod", Parameters = [], Id = Guid.NewGuid(), RelatedInvocationId = Guid.NewGuid() };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockSimClassService!.Setup(s => s.GetSimClassById(referenceId)).Returns(referenceClass);
        _mockSimClassService.Setup(s => s.GetSimClassById(intTypeId)).Returns(intType);
        _mockSimClassService.Setup(s => s.GetSimClassById(stringTypeId)).Returns(stringType);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);
        _mockExecutionService.Setup(s => s.ValidateMethodExistsInClass(referenceClass, signature));

        var result = adapter!.CreateInvocation(methodId, invocationRequest);

        _mockMethodService.VerifyAll();
        _mockSimClassService.VerifyAll();

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
            IdReference = referenceId,
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
            IdReference = referenceId,
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
            IdReference = attributeId,
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
            IdReference = referenceId,
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
            IdReference = referenceId,
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
            ClassTypeId = classTypeId
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
            ClassTypeId = classTypeId
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
}
