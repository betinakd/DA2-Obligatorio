namespace Domain;

public class ProjectEvaluation
{
    public Guid Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal CustomerAcquisitionCost { get; set; }
    public decimal AverageRevenuePerCustomer { get; set; }
    public decimal AveragePurchaseFrequency { get; set; }
    public decimal CustomerLifespan { get; set; }
    public decimal GrossMargin { get; set; }
    public decimal ChurnRate { get; set; }
    public bool IncludeTaxesInCLV { get; set; }
    public decimal TaxRate { get; set; }
    public DateTime CreatedAt { get; set; }

    public decimal CalculateCustomerLifetimeValue()
    {
        // CLV = (Average Revenue Per Customer × Average Purchase Frequency × Customer Lifespan) × Gross Margin
        var clv = AverageRevenuePerCustomer * AveragePurchaseFrequency * CustomerLifespan * GrossMargin;

        if (IncludeTaxesInCLV)
        {
            // Apply tax reduction
            clv = clv * (1 - TaxRate);
        }

        return clv;
    }

    public decimal CalculateReturnOnInvestment()
    {
        // ROI = (CLV - CAC) / CAC
        var clv = CalculateCustomerLifetimeValue();

        if (CustomerAcquisitionCost == 0)
        {
            return 0; // Avoid division by zero
        }

        return (clv - CustomerAcquisitionCost) / CustomerAcquisitionCost;
    }

    public decimal CalculateCLVtoCAC()
    {
        // CLV:CAC ratio
        if (CustomerAcquisitionCost == 0)
        {
            return 0; // Avoid division by zero
        }

        return CalculateCustomerLifetimeValue() / CustomerAcquisitionCost;
    }

    public void Validate()
    {
        if (CustomerAcquisitionCost < 0)
        {
            throw new ArgumentException("Customer Acquisition Cost cannot be negative");
        }

        if (AverageRevenuePerCustomer < 0)
        {
            throw new ArgumentException("Average Revenue Per Customer cannot be negative");
        }

        if (AveragePurchaseFrequency < 0)
        {
            throw new ArgumentException("Average Purchase Frequency cannot be negative");
        }

        if (CustomerLifespan < 0)
        {
            throw new ArgumentException("Customer Lifespan cannot be negative");
        }

        if (GrossMargin < 0 || GrossMargin > 1)
        {
            throw new ArgumentException("Gross Margin must be between 0 and 1");
        }

        if (ChurnRate < 0 || ChurnRate > 1)
        {
            throw new ArgumentException("Churn Rate must be between 0 and 1");
        }

        if (TaxRate < 0 || TaxRate > 1)
        {
            throw new ArgumentException("Tax Rate must be between 0 and 1");
        }
    }
}
