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
    public Guid TypeId => Guid.TryParse(IdType, out var guid) ? guid : Guid.Empty;

    [Required(ErrorMessage = "IdType is required.")]
    public string IdType { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimModelsPrivacity Privacity { get; set; }

    [Required(ErrorMessage = "IdRelatedClass is required.")]
    public string IdRelatedClass { get; set; } = string.Empty;

    [JsonIgnore]
    public Guid RelatedClassId => Guid.TryParse(IdRelatedClass, out var guid) ? guid : Guid.Empty;
}
