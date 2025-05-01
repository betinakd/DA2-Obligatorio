using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;
public class AttributeRequestCreateClass()
{
    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; }

    [JsonIgnore]
    public Guid ClassTypeId => Guid.TryParse(IdClassType, out var guid) ? guid : Guid.Empty;

    [Required(ErrorMessage = "IdClassType is required.")]
    public string IdClassType { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimModelsPrivacity Privacity { get; set; }
}
