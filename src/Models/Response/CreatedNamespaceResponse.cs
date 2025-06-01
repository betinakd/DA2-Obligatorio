namespace Models.Response;

public class CreatedNamespaceResponse
{
    public string Message { get; set; } = "Namespace created successfully";
    public NamespaceResponse NamespaceResponse { get; set; } = new NamespaceResponse();
}
