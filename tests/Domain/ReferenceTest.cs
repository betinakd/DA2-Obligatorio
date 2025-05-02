using Domain;

namespace Tests.Domain;

[TestClass]
public class ReferenceTest()
{
    [TestMethod]
    public void TestGetReferenceTypeDescription_ShouldReturnCorrectType()
    {
        var referenceVariable = new ReferenceVariable();
        var referenceThis = new ReferenceThis();
        var referenceParameter = new ReferenceParameter();
        var referenceBase = new ReferenceBase();
        var referenceAttribute = new ReferenceAttribute();

        Assert.AreEqual("LocalVariable", referenceVariable.GetReferenceTypeDescription());
        Assert.AreEqual("This", referenceThis.GetReferenceTypeDescription());
        Assert.AreEqual("Parameter", referenceParameter.GetReferenceTypeDescription());
        Assert.AreEqual("Base", referenceBase.GetReferenceTypeDescription());
        Assert.AreEqual("Attribute", referenceAttribute.GetReferenceTypeDescription());
    }
}
