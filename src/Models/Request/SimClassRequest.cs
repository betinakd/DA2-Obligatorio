using Models.Enums;

namespace Models.Request;
public class SimClassRequest()
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsAbstract { get; set; }
    public bool IsSealed { get; set; }
    public Guid BaseClassId { get; set; }
    public SimModelsAccesibility State { get; set; } = SimModelsAccesibility.Normal;
}
