using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class AttributeRequestUpdate()
{
    [Required(ErrorMessage = "Id is a Guid and it is required.")]
    public string Id { get; set; } = string.Empty;

    [JsonIgnore]
    public Guid IdAttribute => Guid.TryParse(Id, out var guid) ? guid : Guid.Empty;

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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimModelsPrivacity Privacity { get; set; }

    [Required(ErrorMessage = "IdRelatedClass is required.")]
    public string IdRelatedClass { get; set; } = string.Empty;

    [JsonIgnore]
    public Guid RelatedClassId => Guid.TryParse(IdRelatedClass, out var guid) ? guid : Guid.Empty;

    [Required(ErrorMessage = "IsStatic is required his value is true or false.")]
    public bool IsStatic { get; set; }
}
