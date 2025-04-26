namespace Domain;
public class Invocation()
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Reference Reference { get; set; } = null;
    public Signature Signature { get; set; } = new Signature();
    public Guid? RelatedMethodId { get; set; }
    public SimMethod? RelatedMethod { get; set; }
}
