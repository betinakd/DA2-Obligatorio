namespace Domain;
public abstract class Reference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Invocation? RelatedInvocation { get; set; }
    public Guid RelatedInvocationId { get; set; }

    public abstract SimClass GetSimClass();
    public abstract string GetSignature(Signature signature);
}
