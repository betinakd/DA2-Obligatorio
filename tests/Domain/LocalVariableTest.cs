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
            Type = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateALocalVariableWithInvalidCharacters_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new LocalVariable()
        {
            Name = "Invalid@Name",
            Type = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateLocalVariableWithOnlyNumbers_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new LocalVariable()
        {
            Name = "123456",
            Type = simObject,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateLocalVariableWithReservedWords_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new LocalVariable()
        {
            Name = "class",
            Type = simObject,
        };
    }
}
