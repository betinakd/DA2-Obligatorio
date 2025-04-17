using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;

public class AttributeRequest()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid TypeId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimModelsPrivacity Privacity { get; set; }

    public Guid RelatedClassId { get; set; }
}
