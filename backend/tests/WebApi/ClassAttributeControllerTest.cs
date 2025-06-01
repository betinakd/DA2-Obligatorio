using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Request;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class ClassAttributeControllerTest
{
    private Mock<IAttributeAdapter>? _mockAttributeAdapter;
    private ClassAttributeController? _attributeController;

    [TestInitialize]
    public void Setup()
    {
        _mockAttributeAdapter = new Mock<IAttributeAdapter>();
        _attributeController = new ClassAttributeController(_mockAttributeAdapter.Object);
    }

    [TestMethod]
    public void CreateAttribute_ReturnsCreatedAtRouteResult()
    {
        var id = Guid.NewGuid();
        var request = new AttributeRequest()
        {
            Name = "DummyAttribute",
            IdReference = Guid.NewGuid().ToString(),
            Privacity = SimModelsPrivacity.Public,
        };

        var expectedResponse = new CreatedAttributeResponse()
        {
            Message = "Attribute was created successfully",
            Attribute = new AttributeResponse()
            {
                Id = id,
                Name = "DummyAttribute",
                ReferenceId = Guid.NewGuid(),
                Privacity = SimModelsPrivacity.Public,
                RelatedClassId = Guid.NewGuid()
            }
        };

        _mockAttributeAdapter!
            .Setup(a => a.CreateAttribute(id, request))
            .Returns(expectedResponse);

        var result = _attributeController!.CreateAttribute(id, request);

        _mockAttributeAdapter.Verify(a => a.CreateAttribute(id, request), Times.Once);
        var createdResult = result as CreatedAtRouteResult;
        Assert.IsNotNull(createdResult, "El resultado no es un CreatedAtRouteResult");
        Assert.AreEqual("GetAttributeById", createdResult.RouteName, "El nombre de la ruta no coincide");
        Assert.IsTrue(createdResult.RouteValues.ContainsKey("id"), "La ruta no contiene 'id'");
        Assert.AreEqual(expectedResponse.Attribute.Id, createdResult.RouteValues["id"], "El id de la ruta no coincide con el esperado");
        Assert.AreEqual(expectedResponse, createdResult.Value, "El valor retornado no coincide con el esperado");
    }
}
