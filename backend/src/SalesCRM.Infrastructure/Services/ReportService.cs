using Microsoft.EntityFrameworkCore;
using SalesCRM.Application.DTOs.Reports;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Domain.Enums;
using SalesCRM.Infrastructure.Data;
using SalesCRM.Shared.Models;

namespace SalesCRM.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ReportService(ApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResult<BranchReportDto>> GetBranchReportAsync(
        Guid? branchId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        // Enforce branch boundary
        Guid? targetBranchId = _currentUserService.IsAdmin ? branchId : _currentUserService.BranchId;
        if (!targetBranchId.HasValue)
        {
            var defaultBranch = await _context.Branches.FirstOrDefaultAsync(cancellationToken);
            if (defaultBranch == null)
            {
                return ApiResult<BranchReportDto>.Failure("No default branch exists.");
            }
            targetBranchId = defaultBranch.Id;
        }

        var branch = await _context.Branches.FindAsync(new object[] { targetBranchId.Value }, cancellationToken);
        if (branch == null)
        {
            return ApiResult<BranchReportDto>.Failure($"Branch with ID '{targetBranchId.Value}' not found.");
        }

        // Adjust dates to cover full days
        var start = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(endDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        // Fetch successful orders in range
        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(d => d.Product)
            .Include(o => o.Payments)
            .Where(o => o.BranchId == targetBranchId.Value && o.CreatedAt >= start && o.CreatedAt <= end && 
                       (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Confirmed))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var report = new BranchReportDto
        {
            BranchId = targetBranchId.Value,
            BranchName = branch.Name,
            TotalRevenue = orders.Sum(o => o.FinalAmount),
            TotalOrders = orders.Count,
            AverageOrderValue = orders.Count == 0 ? 0 : orders.Sum(o => o.FinalAmount) / orders.Count
        };

        // 1. Revenue by Payment Method
        var payments = orders.SelectMany(o => o.Payments)
            .Where(p => p.Status == PaymentStatus.Completed)
            .GroupBy(p => p.PaymentMethod)
            .Select(g => new PaymentMethodReportDto
            {
                Method = g.Key,
                MethodName = g.Key.ToString(),
                Revenue = g.Sum(p => p.Amount),
                TransactionCount = g.Count()
            }).ToList();
        
        report.RevenueByPaymentMethod = payments;

        // 2. Top Selling Products
        var topProducts = orders.SelectMany(o => o.OrderDetails)
            .GroupBy(d => d.ProductId)
            .Select(g => {
                var firstDetail = g.First();
                return new TopProductReportDto
                {
                    ProductId = g.Key,
                    ProductCode = firstDetail.Product?.Code,
                    ProductName = firstDetail.Product?.Name,
                    QuantitySold = g.Sum(d => d.Quantity),
                    RevenueGenerated = g.Sum(d => d.SubTotal)
                };
            })
            .OrderByDescending(p => p.QuantitySold)
            .Take(10)
            .ToList();

        report.TopProducts = topProducts;

        // 3. Daily Revenue
        var dailyRev = orders.GroupBy(o => o.CreatedAt.Date)
            .Select(g => new DailyRevenueReportDto
            {
                Date = g.Key,
                DateStr = g.Key.ToString("yyyy-MM-dd"),
                Revenue = g.Sum(o => o.FinalAmount),
                OrderCount = g.Count()
            })
            .OrderBy(d => d.Date)
            .ToList();

        report.DailyRevenue = dailyRev;

        return ApiResult<BranchReportDto>.Success(report);
    }

    public async Task<ApiResult<SystemReportDto>> GetSystemReportAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        // Enforce Admin role
        if (!_currentUserService.IsAdmin)
        {
            return ApiResult<SystemReportDto>.Failure("Access denied. Admin role required for system report.");
        }

        var start = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(endDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        // Fetch successful orders in range
        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(d => d.Product)
            .Where(o => o.CreatedAt >= start && o.CreatedAt <= end && 
                       (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Confirmed))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var branches = await _context.Branches.AsNoTracking().ToListAsync(cancellationToken);
        var productsCount = await _context.Products.CountAsync(p => p.IsActive, cancellationToken);

        var report = new SystemReportDto
        {
            TotalRevenue = orders.Sum(o => o.FinalAmount),
            TotalOrders = orders.Count,
            TotalBranches = branches.Count,
            TotalProducts = productsCount
        };

        // 1. Revenue by Branch
        var revByBranch = orders.GroupBy(o => o.BranchId)
            .Select(g => {
                var branchName = branches.FirstOrDefault(b => b.Id == g.Key)?.Name ?? "Unknown";
                return new BranchRevenueReportDto
                {
                    BranchId = g.Key,
                    BranchName = branchName,
                    Revenue = g.Sum(o => o.FinalAmount),
                    OrderCount = g.Count()
                };
            })
            .OrderByDescending(b => b.Revenue)
            .ToList();

        report.RevenueByBranch = revByBranch;

        // 2. Global Top Selling Products
        var globalTop = orders.SelectMany(o => o.OrderDetails)
            .GroupBy(d => d.ProductId)
            .Select(g => {
                var firstDetail = g.First();
                return new TopProductReportDto
                {
                    ProductId = g.Key,
                    ProductCode = firstDetail.Product?.Code,
                    ProductName = firstDetail.Product?.Name,
                    QuantitySold = g.Sum(d => d.Quantity),
                    RevenueGenerated = g.Sum(d => d.SubTotal)
                };
            })
            .OrderByDescending(p => p.QuantitySold)
            .Take(10)
            .ToList();

        report.GlobalTopProducts = globalTop;

        // 3. System Daily Revenue
        var systemDaily = orders.GroupBy(o => o.CreatedAt.Date)
            .Select(g => new DailyRevenueReportDto
            {
                Date = g.Key,
                DateStr = g.Key.ToString("yyyy-MM-dd"),
                Revenue = g.Sum(o => o.FinalAmount),
                OrderCount = g.Count()
            })
            .OrderBy(d => d.Date)
            .ToList();

        report.SystemDailyRevenue = systemDaily;

        return ApiResult<SystemReportDto>.Success(report);
    }
}
