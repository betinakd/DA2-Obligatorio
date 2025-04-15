using System.Text.Json.Serialization;

namespace Models.Request;

public class MethodExecutionRequest()
{
    public string? MethodName { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public List<ParameterRequest>? Parameters { get; set; }
    public string? ReferenceType { get; set; }
    public string? InstanceType { get; set; }
}
