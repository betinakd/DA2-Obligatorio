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
}
