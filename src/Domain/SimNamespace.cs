namespace Domain;

public class SimNamespace
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public Guid? BaseNamespaceId { get; set; }
    public List<SimClass> Elements { get; set; } = [];
}
