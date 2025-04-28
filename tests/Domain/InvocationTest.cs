using Domain;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class InvocationTest
{
    private Invocation? invocation;
    private Reference? validReference;
    private Signature? validSignature;
    private Guid validMethodId;

    [TestInitialize]
    public void Initialize()
    {
        invocation = new Invocation();
        validReference = new ReferenceThis();
        validSignature = new Signature { Name = "TestMethod" };
        validMethodId = Guid.NewGuid();
    }

    // Reference property tests
    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void SetReference_ToNull_ThrowsSimClassInvalidAttribute()
    {
        invocation.Reference = null;
    }

    [TestMethod]
    public void SetReference_ToValidReference_SetsReference()
    {
        invocation.Reference = validReference;
        Assert.AreEqual(validReference, invocation.Reference);
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void SetSignature_ToNull_ThrowsSimClassInvalidAttribute()
    {
        invocation.Signature = null;
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void SetSignature_WithEmptyName_ThrowsSimClassInvalidAttribute()
    {
        invocation.Signature = new Signature { Name = string.Empty };
    }

    [TestMethod]
    public void SetSignature_ToValidSignature_SetsSignature()
    {
        invocation.Signature = validSignature;
        Assert.AreEqual(validSignature, invocation.Signature);
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void SetRelatedMethodId_ToNull_ThrowsSimClassInvalidAttribute()
    {
        invocation.RelatedMethodId = null;
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void SetRelatedMethodId_ToEmptyGuid_ThrowsSimClassInvalidAttribute()
    {
        invocation.RelatedMethodId = Guid.Empty;
    }
}
