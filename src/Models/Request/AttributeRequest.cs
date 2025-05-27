using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class AttributeRequest()
{
    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; }

    [JsonIgnore]
    public Guid ClassTypeId => Guid.TryParse(IdClassType, out var guid) ? guid : Guid.Empty;

    [Required(ErrorMessage = "IdClassType is required.")]
    public string IdClassType { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimModelsPrivacity Privacity { get; set; }

    [JsonRequired]
    public bool IsStatic { get; set; }
}
