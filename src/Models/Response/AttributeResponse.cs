using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Response;

public class AttributeResponse()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? ReferenceId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimModelsPrivacity Privacity { get; set; }

    public Guid? RelatedClassId { get; set; }

    public bool IsStatic { get; set; }
}
