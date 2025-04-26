namespace Domain;
public class Signature
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<Parameter> Parameters { get; set; } = [];
}
