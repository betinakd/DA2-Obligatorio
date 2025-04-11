using Domain;
using IBussinesLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Responses;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class SimClassControllerTest
{
    private Mock<ISimClassService>? _mockSimClassService;
    private SimClassController? _simClassController;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _simClassController = new SimClassController(_mockSimClassService.Object);
    }

    [TestMethod]
    public void GetAllClasses_ShouldReturnAllClasses()
    {
        IEnumerable<SimClass> classes =
        [
            new SimClass { Id = Guid.NewGuid(), Name = "ClassA" },
            new SimClass { Id = Guid.NewGuid(), Name = "ClassB" }
        ];

        var expectedResponse = classes.Select(x => new SimClassResponse(x)).ToList();
        var expectedResult = new OkObjectResult(expectedResponse);

        _mockSimClassService?.Setup(x => x.GetAllSimClass()).Returns(classes);
        var result = _simClassController?.GetAllSimClass();
        var listResult = result as OkObjectResult;
        var resultList = listResult?.Value as List<SimClassResponse>;

        _mockSimClassService?.VerifyAll();

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        Assert.IsNotNull(resultList);
        Assert.AreEqual(expectedResponse.First().Id, resultList!.First().Id);
        Assert.AreEqual(expectedResponse.First().Name, resultList!.First().Name);
        Assert.AreEqual(expectedResponse.Last().Id, resultList!.Last().Id);
        Assert.AreEqual(expectedResponse.Last().Name, resultList!.Last().Name);
        Assert.AreEqual(2, resultList!.Count);
    }
}
