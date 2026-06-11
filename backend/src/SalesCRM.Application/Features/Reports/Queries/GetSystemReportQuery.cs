using MediatR;
using SalesCRM.Application.DTOs.Reports;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Reports.Queries;

public record GetSystemReportQuery(
    DateTime StartDate,
    DateTime EndDate
) : IRequest<ApiResult<SystemReportDto>>;

public class GetSystemReportQueryHandler : IRequestHandler<GetSystemReportQuery, ApiResult<SystemReportDto>>
{
    private readonly IReportService _reportService;

    public GetSystemReportQueryHandler(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<ApiResult<SystemReportDto>> Handle(GetSystemReportQuery request, CancellationToken cancellationToken)
    {
        return await _reportService.GetSystemReportAsync(
            request.StartDate,
            request.EndDate,
            cancellationToken);
    }
}
