using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class InvocationRequest()
{
    [JsonIgnore]
    public Guid ReferenceId => Guid.TryParse(IdReference, out var guid) ? guid : Guid.Empty;

    [Required(ErrorMessage = "IdReference is required and a Guid Type.")]
    public string IdReference { get; set; } = string.Empty;
    public string? MethodName { get; set; }
    public List<ParameterSignatureRequest>? Parameters { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required(ErrorMessage = "TypeReference is required and their values should be: Base, This, LocalVariable, Attribute, Parameter.")]
    public TypeReference? TypeReference { get; set; }
}
