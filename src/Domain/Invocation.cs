namespace Domain;
public class Invocation()
{
    public Guid Id { get; set; }
    public Guid ReferenceId { get; set; }
    public string MethodName { get; set; } = string.Empty;
    public List<Parameter> Parameters { get; set; } = [];
    public Guid? RelatedMethodId { get; set; }
    public SimMethod? RelatedMethod { get; set; }
}
