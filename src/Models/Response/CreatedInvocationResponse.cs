namespace Models.Response;

public class CreatedInvocationResponse
{
    public Guid? Id { get; set; }
    public string? Message { get; set; }
    public InvocationResponse? InvocationResponse { get; set; }
}
