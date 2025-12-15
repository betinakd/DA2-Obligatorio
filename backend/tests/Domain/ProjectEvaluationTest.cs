using Domain;

namespace Tests.Domain;

[TestClass]
public class ProjectEvaluationTest
{
    [TestMethod]
    public void CalculateCustomerLifetimeValue_ValidInputs_ReturnsCorrectValue()
    {
        var projectEvaluation = new ProjectEvaluation
        {
            AverageRevenuePerCustomer = 100m,
            AveragePurchaseFrequency = 2m,
            CustomerLifespan = 5m,
            GrossMargin = 0.5m,
            IncludeTaxesInCLV = false
        };

        var clv = projectEvaluation.CalculateCustomerLifetimeValue();

        Assert.AreEqual(500m, clv);
    }

    [TestMethod]
    public void CalculateCustomerLifetimeValue_WithTaxes_ReturnsCorrectValue()
    {
        var projectEvaluation = new ProjectEvaluation
        {
            AverageRevenuePerCustomer = 100m,
            AveragePurchaseFrequency = 2m,
            CustomerLifespan = 5m,
            GrossMargin = 0.5m,
            IncludeTaxesInCLV = true,
            TaxRate = 0.2m
        };

        var clv = projectEvaluation.CalculateCustomerLifetimeValue();

        Assert.AreEqual(400m, clv);
    }

    [TestMethod]
    public void CalculateReturnOnInvestment_ValidInputs_ReturnsCorrectValue()
    {
        var projectEvaluation = new ProjectEvaluation
        {
            CustomerAcquisitionCost = 100m,
            AverageRevenuePerCustomer = 100m,
            AveragePurchaseFrequency = 2m,
            CustomerLifespan = 5m,
            GrossMargin = 0.5m,
            IncludeTaxesInCLV = false
        };

        var roi = projectEvaluation.CalculateReturnOnInvestment();

        Assert.AreEqual(4m, roi);
    }

    [TestMethod]
    public void CalculateReturnOnInvestment_ZeroCAC_ReturnsZero()
    {
        var projectEvaluation = new ProjectEvaluation
        {
            CustomerAcquisitionCost = 0m,
            AverageRevenuePerCustomer = 100m,
            AveragePurchaseFrequency = 2m,
            CustomerLifespan = 5m,
            GrossMargin = 0.5m
        };

        var roi = projectEvaluation.CalculateReturnOnInvestment();

        Assert.AreEqual(0m, roi);
    }

    [TestMethod]
    public void CalculateCLVtoCAC_ValidInputs_ReturnsCorrectRatio()
    {
        var projectEvaluation = new ProjectEvaluation
        {
            CustomerAcquisitionCost = 100m,
            AverageRevenuePerCustomer = 100m,
            AveragePurchaseFrequency = 2m,
            CustomerLifespan = 5m,
            GrossMargin = 0.5m
        };

        var ratio = projectEvaluation.CalculateCLVtoCAC();

        Assert.AreEqual(5m, ratio);
    }

    [TestMethod]
    public void Validate_NegativeCAC_ThrowsException()
    {
        var projectEvaluation = new ProjectEvaluation
        {
            CustomerAcquisitionCost = -100m
        };

        Assert.ThrowsException<ArgumentException>(() => projectEvaluation.Validate());
    }

    [TestMethod]
    public void Validate_InvalidGrossMargin_ThrowsException()
    {
        var projectEvaluation = new ProjectEvaluation
        {
            CustomerAcquisitionCost = 100m,
            GrossMargin = 1.5m
        };

        Assert.ThrowsException<ArgumentException>(() => projectEvaluation.Validate());
    }

    [TestMethod]
    public void Validate_InvalidChurnRate_ThrowsException()
    {
        var projectEvaluation = new ProjectEvaluation
        {
            CustomerAcquisitionCost = 100m,
            GrossMargin = 0.5m,
            ChurnRate = -0.1m
        };

        Assert.ThrowsException<ArgumentException>(() => projectEvaluation.Validate());
    }

    [TestMethod]
    public void Validate_ValidInputs_NoException()
    {
        var projectEvaluation = new ProjectEvaluation
        {
            CustomerAcquisitionCost = 100m,
            AverageRevenuePerCustomer = 100m,
            AveragePurchaseFrequency = 2m,
            CustomerLifespan = 5m,
            GrossMargin = 0.5m,
            ChurnRate = 0.1m,
            TaxRate = 0.2m
        };

        projectEvaluation.Validate();
    }
}
