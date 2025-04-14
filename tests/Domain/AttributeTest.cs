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
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void CreateAttributeWithEmptyOrWhiteSpace_ShouldThrowSimClassInvalidAttribute()
    {
        _ = new SimAttribute()
        {
            Name = " ",
            Accesibility = SimAccesibility.Public,
            Type = simObject,
            RelatedClass = simInt,
        };
    }
}
