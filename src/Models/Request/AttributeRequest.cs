using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;

public class AttributeRequest()
{
    public Guid Id { get; set; }

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
