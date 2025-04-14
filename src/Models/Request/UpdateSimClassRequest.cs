namespace Models.Request;
public class UpdateSimClassRequest()
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool? IsAbstract { get; set; }
    public bool? IsSealed { get; set; }
    public Guid? BaseClassId { get; set; }
}
