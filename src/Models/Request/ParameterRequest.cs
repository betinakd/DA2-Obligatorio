namespace Models.Request;
public class ParameterRequest()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? MethodId { get; set; }
}
