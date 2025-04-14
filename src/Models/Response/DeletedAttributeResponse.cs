namespace Models.Response;

public class DeletedAttributeResponse()
{
    public Guid? Id { get; set; }
    public string? Message { get; set; }
    public AttributeResponse? Attribute { get; set; }
}