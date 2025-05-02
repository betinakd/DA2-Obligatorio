using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Models.Request;
[ExcludeFromCodeCoverage]
public class MethodExecutionRequest()
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required(ErrorMessage = "Name is required.")]
    public string MethodName { get; set; } = string.Empty;

    public List<ParameterRequest>? Parameters { get; set; } = [];

    [Required(ErrorMessage = "IdReferenceType is required and should be a Guid.")]
    public string IdReferenceType { get; set; } = string.Empty;

    [JsonIgnore]
    public Guid ReferenceTypeId => Guid.TryParse(IdReferenceType, out var guid) ? guid : Guid.Empty;

    [Required(ErrorMessage = "IdInstanceType is required and should be a Guid.")]
    public string IdInstanceType { get; set; } = string.Empty;

    [JsonIgnore]
    public Guid InstanceTypeId => Guid.TryParse(IdInstanceType, out var guid) ? guid : Guid.Empty;
}
