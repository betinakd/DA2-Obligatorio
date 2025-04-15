namespace Models.Request;

public class VariableRequest()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? MethodId { get; set; }
}