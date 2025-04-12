using Domain;
using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class SimClassControllerTest
{
    private Mock<ISimClassAdapter>? _mockSimClassAdapter;
    private SimClassController? _simClassController;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimClassAdapter = new Mock<ISimClassAdapter>(MockBehavior.Strict);
        _simClassController = new SimClassController(_mockSimClassAdapter.Object);
    }

    [TestMethod]
    public void GetAllClasses_ShouldReturnAllClasses()
    {
        // Arrange: Crear una lista simulada de clases
        var classes = new List<SimClassResponse>
    {
        new SimClassResponse(new SimClass { Id = Guid.NewGuid(), Name = "ClassA" }),
        new SimClassResponse(new SimClass { Id = Guid.NewGuid(), Name = "ClassB" })
    };

        _mockSimClassAdapter?.Setup(x => x.GetAllSimClasses()).Returns(classes);

        var result = _simClassController?.GetAllSimClass();

        var listResult = result as OkObjectResult;

        var resultList = listResult?.Value as List<SimClassResponse>;

        _mockSimClassAdapter?.VerifyAll(); // Verificar que el mock fue llamado
        Assert.IsNotNull(result); // El resultado no debe ser null
        Assert.IsInstanceOfType(result, typeof(OkObjectResult)); // El resultado debe ser OkObjectResult
        Assert.IsNotNull(resultList); // La lista no debe ser null
        Assert.AreEqual(2, resultList!.Count); // La lista debe contener 2 elementos
        Assert.AreEqual(classes.First().Name, resultList.First().Name); // Verificar el primer elemento
        Assert.AreEqual(classes.Last().Name, resultList.Last().Name); // Verificar el último elemento
    }
}
