namespace Domain;

public class ExecutionLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Execution { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string ObjectCreate { get; set; } = string.Empty;
}
