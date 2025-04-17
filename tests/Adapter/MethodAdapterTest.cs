using Adapter;
using Adapter.Exceptions;
using Domain;
using Domain.Exceptions;
using FluentAssertions;
using IBussinesLogic;
using Models.Request;
using Models.Response;
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
    public void AddMethodParameter_ShouldReturnCreatedParameterResponse_WhenValid()
    {
        // Arrange
        var methodId = Guid.NewGuid();
        var parameterName = "ValidParameter";
        var parameterType = "string";

        var request = new ParameterRequest
        {
            MethodId = methodId,
            Name = parameterName,
            Type = parameterType
        };

        var expectedParameterResponse = new ParameterResponse
        {
            Id = Guid.NewGuid(),
            Name = parameterName,
            Type = parameterType,
            MethodId = methodId
        };

        var relatedClass = new SimClass { Name = "string" };
        var typeClass = new SimClass { Name = "string" };
        var expectedattribute = new SimAttribute() { Name = parameterName, RelatedClass = relatedClass, Type = typeClass };

        var expectedResponse = new CreatedParameterResponse
        {
            Message = "Parameter added successfully",
            Parameter = expectedParameterResponse
        };

        _mockMethodService
            ?.Setup(service => service.AddMethodParameter(methodId, request.Name, request.Type))
            .Returns(expectedattribute);

        var result = _methodAdapter?.CreateParameter(methodId, request);
        _mockMethodService?.Verify(service => service.AddMethodParameter(methodId, request.Name, request.Type), Times.Once);

        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedParameterResponse>();
        Assert.AreEqual(result.Message, "Parameter added successfully");
        var parameters = result.Parameter;
        Assert.AreEqual(parameters.Name, parameterName);
        Assert.AreEqual(parameters.MethodId, methodId);
        Assert.AreEqual(parameters.Type, parameterType);
    }

    public void AddMethodParameter_ShouldThrowInvalidAttribute_WhenNameOrTypeIsNull()
    {
        var methodId = Guid.NewGuid();
        var parameterName = " ";
        var parameterType = " ";

        var request = new ParameterRequest
        {
            Name = parameterName,
            MethodId = methodId,
            Type = parameterType
        };

        var exception = Assert.ThrowsException<InvalidAttribute>(() =>
        {
            _methodAdapter?.CreateParameter(methodId, request);
        });

        Assert.AreEqual("Parameter information cant be empty", exception.Message);
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
}
