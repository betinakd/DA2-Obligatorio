using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class ParameterTest
{
    private SimClass? simObject;

    [TestInitialize]
    public void Initialize()
    {
        simObject = new SimClass()
        {
            Name = "Object",
            Id = Guid.NewGuid(),
        };
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void CreateParameterWithEmptyOrWhiteSpace_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new Parameter()
        {
            Name = " ",
            Type = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void CreateAParameterWithInvalidCharacters_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new Parameter()
        {
            Name = "Invalid@Name",
            Type = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void CreateParameterWithOnlyNumbers_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new Parameter()
        {
            Name = "123456",
            Type = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void CreateParameterWithReservedWords_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new Parameter()
        {
            Name = "class",
            Type = simObject,
        };
    }
}
