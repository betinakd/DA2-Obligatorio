using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Response;

public class MethodResponse()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? IdClassOwner { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimPrivacity Privacity { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimAccesibility Accesibility { get; set; }
}
