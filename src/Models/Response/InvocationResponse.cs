namespace Models.Response;

public class InvocationResponse()
{
    public Guid Id { get; set; }
    public Guid IdReference { get; set; }
    public string? MethodName { get; set; }
    public List<ParameterResponse>? Parametros { get; set; }
}
