using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class SimClassRequestUpdate()
{
    [Required(ErrorMessage = "Id is a Guid and it is required.")]
    public string Id { get; set; } = string.Empty;

    [JsonIgnore]
    public Guid IdClass => Guid.TryParse(Id, out var guid) ? guid : Guid.Empty;

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required(ErrorMessage = "State is required and their values should be: Normal, Abstract, Sealed.")]
    public SimModelsAccesibility? State { get; set; }

    [JsonIgnore]
    public Guid BaseClassId => Guid.TryParse(IdBaseClass, out var guid) ? guid : Guid.Empty;

    public string IdBaseClass { get; set; } = "11111111-1111-1111-1111-111111111111";
    public List<MethodRequest> Methods { get; set; } = [];
    public List<AttributeRequest> Attributes { get; set; } = [];
}
