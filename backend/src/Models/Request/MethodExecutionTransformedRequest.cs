using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class MethodExecutionTransformedRequest
{
    public MethodExecutionRequest Execution { get; set; } = new MethodExecutionRequest();

    [Required(ErrorMessage = "Transformer's name is required.")]
    public string TransformerName { get; set; } = string.Empty;
}
