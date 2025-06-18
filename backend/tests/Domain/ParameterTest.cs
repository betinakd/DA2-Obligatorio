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
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateParameterWithEmptyOrWhiteSpace_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new Parameter()
        {
            Name = " ",
            Reference = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateAParameterWithInvalidCharacters_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new Parameter()
        {
            Name = "Invalid@Name",
            Reference = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateParameterWithOnlyNumbers_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new Parameter()
        {
            Name = "123456",
            Reference = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateParameterWithReservedWords_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new Parameter()
        {
            Name = "class",
            Reference = simObject,
        };
    }
}
