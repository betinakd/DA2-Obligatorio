using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class MethodRequest()
{
    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required(ErrorMessage = "Privacity is required and their values should be: Private, Public, Protected.")]
    public SimModelsPrivacity? Privacity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required(ErrorMessage = "Accesibility is required and their values should be: Normal, Abstract, Sealed.")]
    public SimModelsAccesibility? Accesibility { get; set; }
    [JsonIgnore]
    public Guid ReturnTypeId => Guid.TryParse(IdReturnType, out var guid) ? guid : Guid.Empty;

    [Required(ErrorMessage = "IdReturnType is required.")]
    public string IdReturnType { get; set; } = string.Empty;
    public List<ParameterRequest> Parameters { get; set; } = [];

    [JsonRequired]
    public bool IsStatic { get; set; }
}
