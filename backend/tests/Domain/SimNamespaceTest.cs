using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class SimNamespaceTest
{
    [TestMethod]
    public void Name_WhenSetToValidValue_ShouldSetValue()
    {
        var simNamespace = new SimNamespace();

        simNamespace.Name = "ValidNamespace";

        Assert.AreEqual("ValidNamespace", simNamespace.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_WhenSetToEmptyString_ShouldThrowException()
    {
        var simNamespace = new SimNamespace();

        simNamespace.Name = string.Empty;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_WhenSetToInvalidCharacters_ShouldThrowException()
    {
        var simNamespace = new SimNamespace();

        simNamespace.Name = "Invalid@Namespace";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_WhenSetToOnlyNumbers_ShouldThrowException()
    {
        var simNamespace = new SimNamespace();

        simNamespace.Name = "12345";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_WhenSetToReservedWord_ShouldThrowException()
    {
        var simNamespace = new SimNamespace();

        simNamespace.Name = "class";
    }

    [TestMethod]
    public void Name_WhenSetToValidNameWithNumbers_ShouldSetValue()
    {
        var simNamespace = new SimNamespace();

        simNamespace.Name = "Namespace123";

        Assert.AreEqual("Namespace123", simNamespace.Name);
    }
}