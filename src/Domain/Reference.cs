namespace Domain;
public abstract class Reference
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public abstract SimClass GetSimClass();

    public abstract string GetSignature(Signature signature);
}
