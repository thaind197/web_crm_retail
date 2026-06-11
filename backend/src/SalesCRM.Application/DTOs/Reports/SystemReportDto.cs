namespace SalesCRM.Application.DTOs.Reports;

public class SystemReportDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalBranches { get; set; }
    public int TotalProducts { get; set; }
    public List<BranchRevenueReportDto> RevenueByBranch { get; set; } = [];
    public List<TopProductReportDto> GlobalTopProducts { get; set; } = [];
    public List<DailyRevenueReportDto> SystemDailyRevenue { get; set; } = [];
}

public class BranchRevenueReportDto
{
    public Guid BranchId { get; set; }
    public string? BranchName { get; set; }
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}
