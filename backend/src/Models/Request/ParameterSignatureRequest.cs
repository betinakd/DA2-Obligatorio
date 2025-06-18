using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class ParameterSignatureRequest()
{
    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; }

    [JsonIgnore]
    public Guid ReferenceId => Guid.TryParse(IdReference, out var guid) ? guid : Guid.Empty;

    [Required(ErrorMessage = "IdReference is required.")]
    public string IdReference { get; set; } = string.Empty;
    [JsonIgnore]
    public Guid InstanceId => Guid.TryParse(IdInstance, out var guid) ? guid : Guid.Empty;

    [Required(ErrorMessage = "IdInstance is required.")]
    public string IdInstance { get; set; } = string.Empty;
}
