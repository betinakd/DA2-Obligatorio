using Domain;
namespace Models.Responses;

public class SimClassResponse(SimClass simClass)
{
    public Guid? Id { get; set; } = simClass.Id;
    public string? Name { get; set; } = simClass.Name;
}
