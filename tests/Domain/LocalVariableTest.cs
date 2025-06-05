using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class LocalVariableTest
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
    public void CreateLocalVariableWithEmptyOrWhiteSpace_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new LocalVariable()
        {
            Name = " ",
            Reference = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateALocalVariableWithInvalidCharacters_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new LocalVariable()
        {
            Name = "Invalid@Name",
            Reference = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateLocalVariableWithOnlyNumbers_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new LocalVariable()
        {
            Name = "123456",
            Reference = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateLocalVariableWithReservedWords_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new LocalVariable()
        {
            Name = "class",
            Reference = simObject,
        };
    }
}
