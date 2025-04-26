using Models.Enums;

namespace Models.Request;

public class InvocationRequest()
{
    public Guid IdReference { get; set; }
    public string? MethodName { get; set; }
    public List<ParameterRequest>? Parameters { get; set; }
    public TypeReference TypeReference { get; set; }
}
