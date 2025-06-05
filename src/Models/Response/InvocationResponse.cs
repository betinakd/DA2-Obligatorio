namespace Models.Response;

public class InvocationResponse()
{
    public Guid Id { get; set; }
    public Guid IdReference { get; set; }
    public string? TypeReference { get; set; }
    public string? MethodName { get; set; }
    public List<ParameterResponse>? Parameters { get; set; }
}
