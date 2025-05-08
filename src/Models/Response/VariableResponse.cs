namespace Models.Response;

public class VariableResponse()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? MethodId { get; set; }
    public Guid? ClassTypeId { get; set; }
}
