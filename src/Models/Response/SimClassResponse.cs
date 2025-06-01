using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Response;

public class SimClassResponse()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? IdBaseClass { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimModelsAccesibility State { get; set; }
    public List<MethodResponse> Methods { get; set; } = [];
    public List<AttributeResponse> Attributes { get; set; } = [];
    public List<InterfaceResponse> Implements { get; set; } = [];
    public NamespaceResponse? BaseNamespace { get; set; }
}
