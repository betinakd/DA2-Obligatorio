using Adapter;
using Adapter.Exceptions;
using Domain;
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
    private MethodAdapter? _methodAdapter;

    [TestInitialize]
    public void Initialize()
    {
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _methodAdapter = new MethodAdapter(_mockMethodService.Object);
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
    public void AddMethodVariable_ShouldReturnCreatedParameterResponse_WhenValid()
    {
        var methodId = Guid.NewGuid();
        var varName = "ValidVariable";
        var varType = "string";

        var request = new VariableRequest
        {
            MethodId = methodId,
            Name = varName,
            Type = varType
        };

        var expectedVariableResponse = new VariableResponse
        {
            Id = Guid.NewGuid(),
            Name = varName,
            Type = varType,
            MethodId = methodId
        };

        var relatedClass = new SimClass { Name = "string" };
        var typeClass = new SimClass { Name = "string" };
        var expectedattribute = new SimAttribute { Name = varName, RelatedClass = relatedClass, Type = typeClass };

        var expectedResponse = new CreatedVariableResponse
        {
            Message = "Local variable added successfully",
            Variable = expectedVariableResponse
        };

        _mockMethodService
            ?.Setup(service => service.AddLocalVariable(methodId, request.Name, request.Type))
            .Returns(expectedattribute);

        var result = _methodAdapter?.CreateVariable(methodId, request);

        _mockMethodService?.Verify(service => service.AddLocalVariable(methodId, request.Name, request.Type), Times.Once); // Verifica el método correcto

        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedVariableResponse>();
        Assert.AreEqual(result.Message, "Local variable added successfully");
        var variable = result.Variable;
        Assert.AreEqual(variable.Name, varName);
        Assert.AreEqual(variable.MethodId, methodId);
        Assert.AreEqual(variable.Type, varType);
    }

    [TestMethod]
    public void AddMethodLocalVariable_ShouldThrowInvalidAttribute_WhenNameOrTypeIsNull()
    {
        var methodId = Guid.NewGuid();
        var parameterName = " ";
        var parameterType = " ";

        var request = new VariableRequest
        {
            Name = parameterName,
            MethodId = methodId,
            Type = parameterType
        };

        var exception = Assert.ThrowsException<InvalidAttribute>(() =>
        {
            _methodAdapter?.CreateVariable(methodId, request);
        });

        Assert.AreEqual("Local variable information cant be empty", exception.Message);
    }
}
