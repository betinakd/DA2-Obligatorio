using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;

public class MethodRequest()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? IdClassOwner { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimModelsPrivacity Privacity { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimModelsAccesibility Accesibility { get; set; }
}
