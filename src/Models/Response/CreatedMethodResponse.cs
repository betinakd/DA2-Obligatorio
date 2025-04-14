namespace Models.Response;

public class CreatedMethodResponse()
{
    public Guid? Id { get; set; }
    public string? Message { get; set; }
    public MethodResponse? MethodResponse { get; set; }
}
