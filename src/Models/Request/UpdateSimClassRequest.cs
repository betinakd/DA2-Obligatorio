using Models.Enums;

namespace Models.Request;
public class UpdateSimClassRequest()
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public SimModelsAccesibility State { get; set; }
    public Guid? BaseClassId { get; set; }
}
