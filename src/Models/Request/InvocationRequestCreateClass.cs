using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class InvocationRequestCreateClass()
{
    public Guid IdReference { get; set; }
    public string? MethodName { get; set; }
    public List<ParameterRequestCreateClass>? Parameters { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required(ErrorMessage = "TypeReference is required and their values should be: Base, This, LocalVariable, Attribute, Parameter.")]
    public TypeReference? TypeReference { get; set; }
}
