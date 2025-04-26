namespace Domain;
public class Signature
{
    public Invocation? RelatedInvocation { get; set; }
    public Guid RelatedInvocationId { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<ParameterSignature> Parameters { get; set; } = [];
}
