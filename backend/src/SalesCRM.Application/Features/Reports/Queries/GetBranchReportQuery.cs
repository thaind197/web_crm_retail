using MediatR;
using SalesCRM.Application.DTOs.Reports;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Reports.Queries;

public record GetBranchReportQuery(
    Guid? BranchId,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<ApiResult<BranchReportDto>>;

public class GetBranchReportQueryHandler : IRequestHandler<GetBranchReportQuery, ApiResult<BranchReportDto>>
{
    private readonly IReportService _reportService;

    public GetBranchReportQueryHandler(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<ApiResult<BranchReportDto>> Handle(GetBranchReportQuery request, CancellationToken cancellationToken)
    {
        return await _reportService.GetBranchReportAsync(
            request.BranchId,
            request.StartDate,
            request.EndDate,
            cancellationToken);
    }
}
