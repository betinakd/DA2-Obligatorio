using System.Diagnostics.CodeAnalysis;
using Models.Enums;

namespace Models.Request;

[ExcludeFromCodeCoverage]
public class UpdateSimClassRequest()
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public SimModelsAccesibility State { get; set; }
    public Guid? BaseClassId { get; set; }
}
