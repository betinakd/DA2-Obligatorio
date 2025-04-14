namespace Models.Response;

public class UpdateSimClassResponse()
{
    public Guid? Id { get; set; }
    public string? Message { get; set; }
    public SimClassResponse? SimClass { get; set; }
}
