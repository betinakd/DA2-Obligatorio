namespace Models.Response;

public class InterfaceResponse
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }

    public List<MethodResponse> Methods { get; set; } = [];
}
