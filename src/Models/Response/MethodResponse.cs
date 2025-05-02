using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Response;
[ExcludeFromCodeCoverage]
public class MethodResponse()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? IdClassOwner { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimModelsPrivacity Privacity { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SimModelsAccesibility Accesibility { get; set; }
    public Guid? ReturnTypeId { get; set; }
    public List<ParameterResponse> Parameters { get; set; } = [];
    public List<VariableResponse> Variables { get; set; } = [];
    public List<InvocationResponse> Invocations { get; set; } = [];
}
