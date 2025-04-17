using Adapter;
using Adapter.Exceptions;
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
    private MethodAdapter? _methodAdapter;

    [TestInitialize]
    public void Initialize()
    {
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _methodAdapter = new MethodAdapter(_mockMethodService.Object, _mockSimClassService.Object);
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
        var simAttribute = new SimAttribute
        {
            Id = parameter.Id,
            Name = parameterName,
            RelatedClass = simClass
        };

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockSimClassService.Setup(s => s.GetSimClassById(classTypeId)).Returns(simClass);
        mockMethodService.Setup(s => s.GetMethodById(methodId)).Returns(method);
        mockMethodService.Setup(s => s.AddMethodParameter(methodId, It.IsAny<Parameter>())).Returns(simAttribute);

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

        var result = adapter.CreateParameter(methodId, request);

        result.Should().NotBeNull();
        result.Message.Should().Be("Parameter created successfully");
        result.Parameter.Name.Should().Be(parameterName);
        result.Parameter.MethodId.Should().Be(methodId);
        result.Parameter.ClassTypeId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttribute))]
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
        mockSimClassService.Setup(s => s.GetSimClassById(classTypeId)).Throws(new SimClassInvalidAttribute("SimClass error"));

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

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
        var simAttribute = new SimAttribute
        {
            Id = localVariable.Id,
            Name = variableName,
            RelatedClass = simClass
        };

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockSimClassService.Setup(s => s.GetSimClassById(classTypeId)).Returns(simClass);
        mockMethodService.Setup(s => s.GetMethodById(methodId)).Returns(method);
        mockMethodService.Setup(s => s.AddLocalVariable(methodId, It.IsAny<LocalVariable>())).Returns(simAttribute);

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

        var result = adapter.CreateVariable(methodId, request);

        result.Should().NotBeNull();
        result.Message.Should().Be("Variable created successfully");
        result.Variable.Name.Should().Be(variableName);
        result.Variable.MethodId.Should().Be(methodId);
        result.Variable.ClassTypeId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttribute))]
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

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);
        adapter.CreateVariable(methodId, request);
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

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockMethodService.Setup(s => s.GetParameterById(parameterId)).Returns(parameter);

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

        var result = adapter.GetParameter(parameterId);

        result.Should().NotBeNull();
        result.Id.Should().Be(parameterId);
        result.Name.Should().Be("param1");
        result.MethodId.Should().Be(methodId);
        result.ClassTypeId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetParameter_ShouldThrowInvalidOperationException_WhenSimClassInvalidAttributeIsThrown()
    {
        var parameterId = Guid.NewGuid();

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockMethodService.Setup(s => s.GetParameterById(parameterId)).Throws(new SimClassInvalidAttribute("error"));

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

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

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockMethodService.Setup(s => s.GetVariableById(variableId)).Returns(variable);

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

        var result = adapter.GetVariable(variableId);

        result.Should().NotBeNull();
        result.Id.Should().Be(variableId);
        result.Name.Should().Be("var1");
        result.MethodId.Should().Be(methodId);
        result.ClassTypeId.Should().Be(classTypeId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetVariable_ShouldThrowInvalidOperationException_WhenSimClassInvalidAttributeIsThrown()
    {
        var variableId = Guid.NewGuid();

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockMethodService.Setup(s => s.GetVariableById(variableId)).Throws(new SimClassInvalidAttribute("error"));

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

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

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockMethodService.Setup(s => s.GetMethodById(methodId)).Returns(method);

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

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
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetMethod_ShouldThrowInvalidOperationException_WhenSimClassInvalidAttributeIsThrown()
    {
        var methodId = Guid.NewGuid();

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockMethodService.Setup(s => s.GetMethodById(methodId)).Throws(new SimClassInvalidAttribute("error"));

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

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

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockSimClassService.Setup(s => s.GetSimClassById(idClass)).Returns(classOwner);
        mockSimClassService.Setup(s => s.GetSimClassById(returnTypeId)).Returns(returnType);
        mockMethodService.Setup(s => s.AddMethod(idClass, It.IsAny<SimMethod>())).Returns(simMethod);

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

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
    [ExpectedException(typeof(InvalidAttribute))]
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

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockSimClassService.Setup(s => s.GetSimClassById(idClass)).Throws(new SimClassInvalidAttribute("SimClass error"));

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

        adapter.CreateMethod(idClass, methodRequest);
    }

    [TestMethod]
    public void DeleteMethod_ShouldCallDeleteMethod_WhenValid()
    {
        var methodId = Guid.NewGuid();

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

        adapter.DeleteMethod(methodId);

        mockMethodService.Verify(s => s.DeleteMethod(methodId), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DeleteMethod_ShouldThrowInvalidOperationException_WhenSimClassInvalidAttributeIsThrown()
    {
        var methodId = Guid.NewGuid();

        var mockSimClassService = new Mock<ISimClassService>();
        var mockMethodService = new Mock<IMethodService>();
        mockMethodService.Setup(s => s.DeleteMethod(methodId)).Throws(new SimClassInvalidAttribute("error"));

        var adapter = new MethodAdapter(mockMethodService.Object, mockSimClassService.Object);

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

        var parameterRequest = new ParameterRequest
        {
            Name = parameterName,
            ClassTypeId = classTypeId
        };

        var invocationRequest = new InvocationRequest
        {
            IdReference = referenceId,
            Parameters = [parameterRequest]
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Returns(method);
        _mockSimClassService!.Setup(s => s.GetSimClassById(classTypeId)).Returns(simClass);
        _mockMethodService.Setup(s => s.AddInvocation(methodId, It.IsAny<Invocation>()))
            .Returns((Guid id, Invocation inv) => inv);

        var result = _methodAdapter!.CreateInvocation(methodId, invocationRequest);

        result.Should().NotBeNull();
        result.Message.Should().Be("Invocation created successfully");
        result.InvocationResponse.Should().NotBeNull();
        result.InvocationResponse.MethodName.Should().Be("TestMethod");
        result.InvocationResponse.IdReference.Should().Be(referenceId);
        result.InvocationResponse.Parameters.Should().HaveCount(1);
        result.InvocationResponse.Parameters[0].Name.Should().Be(parameterName);
        result.InvocationResponse.Parameters[0].ClassTypeId.Should().Be(classTypeId);
        result.InvocationResponse.Parameters[0].MethodId.Should().Be(methodId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
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
            Parameters = [parameterRequest]
        };

        _mockMethodService!.Setup(s => s.GetMethodById(methodId)).Throws(new Exception("error"));

        _methodAdapter!.CreateInvocation(methodId, invocationRequest);
    }
}
