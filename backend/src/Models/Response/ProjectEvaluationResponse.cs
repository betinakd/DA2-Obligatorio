namespace Models.Response;

public class ProjectEvaluationResponse
{
    public Guid Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal CustomerAcquisitionCost { get; set; }
    public decimal CustomerLifetimeValue { get; set; }
    public decimal ReturnOnInvestment { get; set; }
    public decimal CLVtoCACRatio { get; set; }
    public decimal ChurnRate { get; set; }
    public bool IncludedTaxesInCLV { get; set; }
    public DateTime CreatedAt { get; set; }
}
