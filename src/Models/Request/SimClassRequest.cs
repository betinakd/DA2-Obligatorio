namespace Models.Request;
public class SimClassRequest()
{
    public string? Name { get; set; }
    public bool? IsAbstract { get; set; }
    public bool? IsSealed { get; set; }
    public Guid? BaseClassId { get; set; }
}