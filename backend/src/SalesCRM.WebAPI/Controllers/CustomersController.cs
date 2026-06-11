using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesCRM.Domain.Entities;
using SalesCRM.Infrastructure.Data;
using SalesCRM.Shared.Models;

namespace SalesCRM.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public CustomersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<PagedResult<CustomerDto>>>> GetCustomers(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _dbContext.Customers
            .Include(c => c.Orders)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => (c.Phone != null && c.Phone.Contains(searchTerm)) || c.Name.Contains(searchTerm));
        }

        int totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                LoyaltyPoints = (int)((c.Orders.Sum(o => o.FinalAmount) - _dbContext.ReturnOrders.Where(ro => ro.OriginalOrder != null && ro.OriginalOrder.CustomerId == c.Id).Sum(ro => (decimal?)ro.RefundAmount) ?? 0) / 10000),
                OrdersCount = c.Orders.Count
            })
            .ToListAsync();

        var pagedResult = new PagedResult<CustomerDto>(items, pageNumber, pageSize, totalCount);
        return Ok(ApiResult<PagedResult<CustomerDto>>.Success(pagedResult));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<Guid>>> CreateCustomer([FromBody] CreateCustomerDto model)
    {
        if (string.IsNullOrWhiteSpace(model.FullName))
        {
            return BadRequest(ApiResult<Guid>.Failure("Họ tên không được để trống."));
        }

        var customer = new Customer
        {
            Name = model.FullName,
            Phone = model.PhoneNumber,
            Email = model.Email
        };

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        return Ok(ApiResult<Guid>.Success(customer.Id));
    }

    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<ApiResult<List<CustomerHistoryDto>>>> GetHistory(Guid id)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.CustomerId == id)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var historyList = orders.Select(o => new CustomerHistoryDto
        {
            OrderCode = o.OrderCode,
            DateStr = o.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
            TotalAmount = o.FinalAmount,
            PointsEarned = (int)(o.FinalAmount / 10000)
        }).ToList();

        return Ok(ApiResult<List<CustomerHistoryDto>>.Success(historyList));
    }
}

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullName => Name;
    public string? Phone { get; set; }
    public string? PhoneNumber => Phone;
    public string? Email { get; set; }
    public int LoyaltyPoints { get; set; }
    public int OrdersCount { get; set; }
}

public class CreateCustomerDto
{
    public required string FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

public class CustomerHistoryDto
{
    public string OrderCode { get; set; } = string.Empty;
    public string DateStr { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int PointsEarned { get; set; }
}
