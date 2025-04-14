using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Response;

public class AttributeResponse()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public SimClassResponse? Type { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimAccesibility Accesibility { get; set; }

    public SimClassResponse? RelatedClass { get; set; }
}