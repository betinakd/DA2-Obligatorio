namespace Domain;

public class SimNamespace
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public List<SimClass> Classes { get; set; } = [];
    public List<SimInterface> Interfaces { get; set; } = [];
}
