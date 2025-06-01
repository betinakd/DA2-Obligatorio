using System.Diagnostics.CodeAnalysis;

namespace Models.Response;

[ExcludeFromCodeCoverage]
public class ParameterSignatureResponse()
{
    public string? Name { get; set; }
    public Guid ReferenceId { get; set; } = Guid.Empty;
    public Guid InstanceId { get; set; } = Guid.Empty;
}
