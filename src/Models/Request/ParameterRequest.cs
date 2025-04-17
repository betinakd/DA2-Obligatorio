namespace Models.Request;
public class ParameterRequest()
{
    public string? Name { get; set; }
    public Guid? MethodId { get; set; }
    public Guid ClassTypeId { get; set; }
}
