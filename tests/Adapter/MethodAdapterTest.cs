using Adapter;
using Adapter.Exceptions;
using Domain;
using FluentAssertions;
using IBussinesLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
