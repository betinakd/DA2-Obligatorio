using Adapter;
using Adapter.Exceptions;
using Domain;
using Domain.Exceptions;
using FluentAssertions;
using IBussinesLogic;
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
}
