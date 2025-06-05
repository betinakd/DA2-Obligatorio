using Domain;
using Domain.Enums;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class AttributeTest
{
    private SimClass? simObject;
    private SimClass simInt = new SimClass() { };

    [TestInitialize]
    public void Initialize()
    {
        simObject = new SimClass()
        {
            Name = "Object",
            Id = Guid.NewGuid(),
        };

        simInt = new SimClass()
        {
            Name = "Int",
            Id = Guid.NewGuid(),
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateAttributeWithEmptyOrWhiteSpace_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new SimAttribute()
        {
            Name = " ",
            Privacity = SimPrivacity.Public,
            Reference = simObject,
            RelatedClass = simInt,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateAttributeWithInvalidCharacters_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new SimAttribute()
        {
            Name = "Invalid@Name",
            Privacity = SimPrivacity.Public,
            Reference = simObject,
            RelatedClass = simInt,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateAttributeWithOnlyNumbers_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new SimAttribute()
        {
            Name = "123456",
            Privacity = SimPrivacity.Public,
            Reference = simObject,
            RelatedClass = simInt,
        };
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateAttributeWithReservedWords_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new SimAttribute()
        {
            Name = "class",
            Privacity = SimPrivacity.Public,
            Reference = simObject,
            RelatedClass = simInt,
        };
    }
}
