using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class InterfaceRequestUpdate()
{
    [JsonIgnore]
    public Guid InterfaceId => Guid.TryParse(IdInterface, out var guid) ? guid : Guid.Empty;

    public string IdInterface { get; set; } = string.Empty;
}
