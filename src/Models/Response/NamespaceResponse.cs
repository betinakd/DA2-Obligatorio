namespace Models.Response;

public class NamespaceResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? BaseNamespaceName { get; set; }
    public Guid? BaseNamespaceId { get; set; }
    public List<SimClassResponse> Elements { get; set; } = [];
}
