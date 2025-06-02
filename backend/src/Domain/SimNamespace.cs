namespace Domain;

public class SimNamespace
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public SimNamespace? BaseNamespace { get; set; }
    public Guid? BaseNamespaceId { get; set; } = null;
    public List<SimClass> Elements { get; set; } = [];
}
