using Domain.Enums;
using Models.Enums;

namespace Tests.Adapter;

[TestClass]
public class HelperTest
{
    [TestMethod]
    public void MapToModelPrivacity_NullInput_ReturnsDefault()
    {
        SimPrivacity? privacity = null;

        var result = EnumMapper.MapToModelPrivacity(privacity);

        Assert.AreEqual(default(SimModelsPrivacity), result);
    }

    [TestMethod]
    public void MapToModelAccesibility_NullInput_ReturnsDefault()
    {
        SimAccesibility? accesibility = null;

        var result = EnumMapper.MapToModelAccesibility(accesibility);

        Assert.AreEqual(default(SimModelsAccesibility), result);
    }

    [TestMethod]
    public void MapToModelPrivacity_WithValue_ReturnsCorrectMapping()
    {
        SimPrivacity? privacity = SimPrivacity.Public;

        var result = EnumMapper.MapToModelPrivacity(privacity);

        Assert.AreEqual(SimModelsPrivacity.Public, result);
    }

    [TestMethod]
    public void MapToModelAccesibility_WithValue_ReturnsCorrectMapping()
    {
        SimAccesibility? accesibility = SimAccesibility.Normal;

        var result = EnumMapper.MapToModelAccesibility(accesibility);

        Assert.AreEqual(SimModelsAccesibility.Normal, result);
    }
}
