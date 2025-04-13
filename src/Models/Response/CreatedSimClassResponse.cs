namespace Models.Response;

public class CreatedSimClassResponse()
{
    public Guid? Id { get; set; }
    public string? Message { get; set; }
    public SimClassResponse? SimClass { get; set; }
}