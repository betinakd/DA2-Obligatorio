namespace Domain;

public class SimMethod
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public SimAttribute? ReturnType { get; set; } = null!;
}
