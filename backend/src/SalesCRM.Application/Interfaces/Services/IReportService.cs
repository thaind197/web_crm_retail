using SalesCRM.Application.DTOs.Reports;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Interfaces.Services;

public interface IReportService
{
    Task<ApiResult<BranchReportDto>> GetBranchReportAsync(Guid? branchId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<ApiResult<SystemReportDto>> GetSystemReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
