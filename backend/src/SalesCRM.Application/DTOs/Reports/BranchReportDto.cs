using SalesCRM.Domain.Enums;

namespace SalesCRM.Application.DTOs.Reports;

public class BranchReportDto
{
    public Guid BranchId { get; set; }
    public string? BranchName { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<PaymentMethodReportDto> RevenueByPaymentMethod { get; set; } = [];
    public List<TopProductReportDto> TopProducts { get; set; } = [];
    public List<DailyRevenueReportDto> DailyRevenue { get; set; } = [];
}

public class PaymentMethodReportDto
{
    public PaymentMethod Method { get; set; }
    public string? MethodName { get; set; }
    public decimal Revenue { get; set; }
    public int TransactionCount { get; set; }
}

public class TopProductReportDto
{
    public Guid ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public int QuantitySold { get; set; }
    public decimal RevenueGenerated { get; set; }
}

public class DailyRevenueReportDto
{
    public DateTime Date { get; set; }
    public string? DateStr { get; set; }
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}
