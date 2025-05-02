using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class SimClassRequestUpdate()
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required(ErrorMessage = "State is required and their values should be: Normal, Abstract, Sealed.")]
    public SimModelsAccesibility? State { get; set; }

    [JsonIgnore]
    public Guid BaseClassId => Guid.TryParse(IdBaseClass, out var guid) ? guid : Guid.Empty;

    [Required(ErrorMessage = "IdBaseClass is required and Guid format.")]
    public string IdBaseClass { get; set; } = string.Empty;
}
