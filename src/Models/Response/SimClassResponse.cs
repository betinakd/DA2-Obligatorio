using Models.Enums;

namespace Models.Response;

public class SimClassResponse()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public SimModelsAccesibility State { get; set; }

    public string? Message { get; set; }
}
