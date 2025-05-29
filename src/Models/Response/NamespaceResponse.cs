namespace Models.Response;

public class NamespaceResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Guid? BaseNamespaceId { get; set; }
    public List<SimClassResponse> Classes { get; set; } = [];
    public List<InterfaceResponse> Interfaces { get; set; } = [];
}
