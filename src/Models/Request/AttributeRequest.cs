using System.Text.Json.Serialization;
using Models.Enums;
using Models.Response;

namespace Models.Request;

public class AttributeRequest()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public SimClassResponse? Type { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimAccesibility Accesibility { get; set; }

    public SimClassResponse? RelatedClass { get; set; }
}