namespace WebApi.Filters;

public class ErrorResponse
{
    public int InnerCode { get; set; } = 0;
    public string Message { get; set; } = string.Empty;
}
