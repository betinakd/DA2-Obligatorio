using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class SimClassRequestCreate()
{
    public Guid Id { get; set; }

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

    [Required(ErrorMessage = "Namespace ig is required.")]
    public Guid BaseNamespaceId { get; set; } = Guid.Empty;
}
