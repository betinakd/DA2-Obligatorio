using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models.Request;

public class NamespaceRequest
{
    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; }

    [JsonIgnore]
    public Guid BaseNamespaceId => Guid.TryParse(IdBaseNamespace, out var guid) ? guid : Guid.Empty;

    public string IdBaseNamespace { get; set; } = string.Empty;
}
