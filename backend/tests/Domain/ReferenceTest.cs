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

    [TestMethod]
    public void ReferenceThis_UsesDynamicDispatch_ReturnsTrue()
    {
        var reference = new ReferenceThis();
        Assert.IsTrue(reference.UsesDynamicDispatch());
    }

    [TestMethod]
    public void ReferenceVariable_UsesDynamicDispatch_ReturnsTrue()
    {
        var reference = new ReferenceVariable();
        Assert.IsTrue(reference.UsesDynamicDispatch());
    }

    [TestMethod]
    public void ReferenceAttribute_UsesDynamicDispatch_ReturnsTrue()
    {
        var reference = new ReferenceAttribute();
        Assert.IsTrue(reference.UsesDynamicDispatch());
    }

    [TestMethod]
    public void ReferenceParameter_UsesDynamicDispatch_ReturnsTrue()
    {
        var reference = new ReferenceParameter();
        Assert.IsTrue(reference.UsesDynamicDispatch());
    }

    [TestMethod]
    public void ReferenceStatic_UsesDynamicDispatch_ReturnsFalse()
    {
        var reference = new ReferenceStatic();
        Assert.IsFalse(reference.UsesDynamicDispatch());
    }

    [TestMethod]
    public void ReferenceStaticAttribute_UsesDynamicDispatch_ReturnsFalse()
    {
        var reference = new ReferenceStaticAttribute();
        Assert.IsTrue(reference.UsesDynamicDispatch());
    }

    [TestMethod]
    public void ReferenceBase_UsesDynamicDispatch_ReturnsFalse()
    {
        var reference = new ReferenceBase();
        Assert.IsFalse(reference.UsesDynamicDispatch());
    }
}
