namespace Domain;

public class SimInterface
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public List<Signature> MethodsSignatures { get; set; } = [];
}
