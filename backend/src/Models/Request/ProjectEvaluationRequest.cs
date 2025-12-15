using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class ProjectEvaluationRequest
{
    [Required]
    public string ProjectName { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Customer Acquisition Cost must be a positive value")]
    public decimal CustomerAcquisitionCost { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Average Revenue Per Customer must be a positive value")]
    public decimal AverageRevenuePerCustomer { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Average Purchase Frequency must be a positive value")]
    public decimal AveragePurchaseFrequency { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Customer Lifespan must be a positive value")]
    public decimal CustomerLifespan { get; set; }

    [Required]
    [Range(0, 1, ErrorMessage = "Gross Margin must be between 0 and 1")]
    public decimal GrossMargin { get; set; }

    [Required]
    [Range(0, 1, ErrorMessage = "Churn Rate must be between 0 and 1")]
    public decimal ChurnRate { get; set; }

    public bool IncludeTaxesInCLV { get; set; } = false;

    [Range(0, 1, ErrorMessage = "Tax Rate must be between 0 and 1")]
    public decimal TaxRate { get; set; } = 0;
}
