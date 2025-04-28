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
}
