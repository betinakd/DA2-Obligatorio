namespace Models.Request;

public class MethodExecutionRequest()
{
    public string MethodName { get; set; } = string.Empty;
    public List<ParameterRequest>? Parameters { get; set; }
    public Guid ReferenceTypeId { get; set; }
    public Guid InstanceTypeId { get; set; }
    public string InstanceName { get; set; } = string.Empty;
}
