using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class ParameterSignatureTest
{
    [TestMethod]
    public void Constructor_ShouldInitializeWithNewGuid()
    {
        var type = new SimClass() { Id = Guid.NewGuid(), Name = "typeP" };
        var parameter = new ParameterSignature() { Id = Guid.NewGuid(), Name = "parameter", SignatureId = Guid.NewGuid(), Reference = type, ReferenceId = type.Id };

        Assert.AreNotEqual(Guid.Empty, parameter.Id);
    }

    [TestMethod]
    public void Name_ShouldSetValidName()
    {
        var parameter = new ParameterSignature();
        var validName = "validName";

        parameter.Name = validName;

        Assert.AreEqual(validName, parameter.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_ShouldThrowException_WhenNameIsEmpty()
    {
        var parameter = new ParameterSignature();

        parameter.Name = string.Empty;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_ShouldThrowException_WhenNameContainsInvalidCharacters()
    {
        var parameter = new ParameterSignature();

        parameter.Name = "invalid@name";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_ShouldThrowException_WhenNameContainsOnlyNumbers()
    {
        var parameter = new ParameterSignature();

        parameter.Name = "12345";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void Name_ShouldThrowException_WhenNameIsReservedWord()
    {
        var parameter = new ParameterSignature();

        parameter.Name = "class";
    }
}
