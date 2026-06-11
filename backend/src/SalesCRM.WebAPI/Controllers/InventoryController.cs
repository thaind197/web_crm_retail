using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesCRM.Application.DTOs.Inventory;
using SalesCRM.Application.Features.Inventory.Commands;
using SalesCRM.Application.Features.Inventory.Queries;
using SalesCRM.Domain.Entities;
using SalesCRM.Domain.Enums;
using SalesCRM.Infrastructure.Data;
using SalesCRM.Shared.Models;

namespace SalesCRM.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ApplicationDbContext _dbContext;

    public InventoryController(IMediator mediator, ApplicationDbContext dbContext)
    {
        _mediator = mediator;
        _dbContext = dbContext;
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("import")]
    public async Task<ActionResult<ApiResult<Guid>>> ImportBatches([FromBody] BatchImportCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("near-expiry")]
    public async Task<ActionResult<ApiResult<PagedResult<NearExpiryProductDto>>>> GetNearExpiryProducts(
        [FromQuery] Guid? branchId,
        [FromQuery] int thresholdDays = 30,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetNearExpiryProductsQuery(branchId, thresholdDays, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<ApiResult<PagedResult<LowStockProductDto>>>> GetLowStockProducts(
        [FromQuery] Guid? branchId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetLowStockProductsQuery(branchId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("transfers")]
    public async Task<ActionResult<ApiResult<Guid>>> CreateTransfer([FromBody] CreateStockTransferCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetTransferById), new { id = result.Data }, result);
    }

    [HttpGet("transfers")]
    public async Task<ActionResult<ApiResult<PagedResult<StockTransferDto>>>> GetTransfers(
        [FromQuery] Guid? fromBranchId,
        [FromQuery] Guid? toBranchId,
        [FromQuery] TransferStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetStockTransfersQuery(fromBranchId, toBranchId, status, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("transfers/{id:guid}")]
    public async Task<ActionResult<ApiResult<StockTransferDto>>> GetTransferById(Guid id)
    {
        var query = new GetStockTransferByIdQuery(id);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("transfers/{id:guid}/receive")]
    public async Task<ActionResult<ApiResult>> ReceiveTransfer(Guid id)
    {
        var command = new ReceiveStockTransferCommand(id);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("replenish")]
    public async Task<ActionResult<ApiResult>> VerifyReplenishment([FromBody] VerifyReplenishmentCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("shelf-stock")]
    public async Task<ActionResult<ApiResult<List<SalesCRM.Application.DTOs.Inventory.ProductLocationDto>>>> GetShelfStock([FromQuery] Guid? branchId)
    {
        Guid? targetBranchId = branchId;
        
        var claimBranchId = User.FindFirst("BranchId")?.Value;
        var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
        
        if (!isAdmin && Guid.TryParse(claimBranchId, out var userBranchId))
        {
            targetBranchId = userBranchId;
        }

        if (!targetBranchId.HasValue && Guid.TryParse(claimBranchId, out var resolvedBranchId))
        {
            targetBranchId = resolvedBranchId;
        }

        if (!targetBranchId.HasValue)
        {
            var defaultBranch = await _dbContext.Branches.FirstOrDefaultAsync();
            targetBranchId = defaultBranch?.Id;
        }

        if (!targetBranchId.HasValue)
        {
            return Ok(ApiResult<List<SalesCRM.Application.DTOs.Inventory.ProductLocationDto>>.Success([]));
        }

        var list = await _dbContext.ProductLocations
            .Include(pl => pl.Location)
            .Include(pl => pl.Product)
            .Include(pl => pl.ProductBatch)
            .Where(pl => pl.BranchId == targetBranchId.Value && pl.Quantity > 0)
            .Select(pl => new SalesCRM.Application.DTOs.Inventory.ProductLocationDto
            {
                Id = pl.Id,
                LocationCode = pl.Location != null ? pl.Location.LocationCode : "N/A",
                ProductCode = pl.Product != null ? pl.Product.Code : "N/A",
                ProductName = pl.Product != null ? pl.Product.Name : "N/A",
                BatchCode = pl.ProductBatch != null ? pl.ProductBatch.BatchCode : "N/A",
                Quantity = pl.Quantity
            })
            .ToListAsync();

        return Ok(ApiResult<List<SalesCRM.Application.DTOs.Inventory.ProductLocationDto>>.Success(list));
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<ApiResult<List<InventoryTransaction>>>> GetTransactions(
        [FromQuery] Guid? branchId,
        [FromQuery] TransactionType? type)
    {
        var query = _dbContext.InventoryTransactions
            .Include(t => t.Product)
            .Include(t => t.ProductBatch)
            .AsNoTracking();

        if (branchId.HasValue)
        {
            query = query.Where(t => t.BranchId == branchId.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        var list = await query
            .OrderByDescending(t => t.CreatedAt)
            .Take(100)
            .ToListAsync();

        return Ok(ApiResult<List<InventoryTransaction>>.Success(list));
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("stocktake")]
    public async Task<ActionResult<ApiResult>> SubmitStocktake([FromBody] StocktakeRequest model)
    {
        if (model.BranchId == Guid.Empty)
        {
            return BadRequest(ApiResult.Failure("Mã chi nhánh không được để trống."));
        }

        foreach (var item in model.Items)
        {
            // 1. Update general branch inventory
            var inv = await _dbContext.BranchInventories
                .FirstOrDefaultAsync(x => x.BranchId == model.BranchId && x.ProductId == item.ProductId);

            int currentQty = inv?.Quantity ?? 0;
            int diff = item.PhysicalQuantity - currentQty;

            if (inv == null)
            {
                inv = new BranchInventory
                {
                    BranchId = model.BranchId,
                    ProductId = item.ProductId,
                    Quantity = item.PhysicalQuantity,
                    MinimumStockLevel = 5
                };
                _dbContext.BranchInventories.Add(inv);
            }
            else
            {
                inv.Quantity = item.PhysicalQuantity;
            }

            // 2. Update specific batch if specified
            Guid? batchId = null;
            if (!string.IsNullOrWhiteSpace(item.BatchCode))
            {
                var batch = await _dbContext.ProductBatches
                    .FirstOrDefaultAsync(x => x.BranchId == model.BranchId && x.ProductId == item.ProductId && x.BatchCode == item.BatchCode);

                if (batch != null)
                {
                    batchId = batch.Id;
                    batch.Quantity = item.PhysicalQuantity;
                }
            }

            // 3. Log transaction
            if (diff != 0)
            {
                var tx = new InventoryTransaction
                {
                    BranchId = model.BranchId,
                    ProductId = item.ProductId,
                    ProductBatchId = batchId,
                    Quantity = diff,
                    Type = TransactionType.Replenish, // stocktake adjustment mapped to Replenish
                    ReferenceCode = "STOCKTAKE_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    Notes = $"Điều chỉnh kiểm kê kho. Chênh lệch: {(diff > 0 ? "+" : "")}{diff}"
                };
                _dbContext.InventoryTransactions.Add(tx);
            }
        }

        await _dbContext.SaveChangesAsync();
        return Ok(ApiResult.Success());
    }

    [HttpPost("write-off")]
    public async Task<ActionResult<ApiResult>> WriteOffStock([FromBody] WriteOffStockCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("batches")]
    public async Task<ActionResult<ApiResult<List<ProductBatch>>>> GetProductBatches([FromQuery] Guid productId)
    {
        Guid? targetBranchId = null;
        var claimBranchId = User.FindFirst("BranchId")?.Value;
        var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
        
        if (!isAdmin && Guid.TryParse(claimBranchId, out var userBranchId))
        {
            targetBranchId = userBranchId;
        }
        else
        {
            var defaultBranch = await _dbContext.Branches.FirstOrDefaultAsync();
            targetBranchId = defaultBranch?.Id;
        }

        if (!targetBranchId.HasValue)
        {
            return Ok(ApiResult<List<ProductBatch>>.Success([]));
        }

        var list = await _dbContext.ProductBatches
            .Where(b => b.BranchId == targetBranchId.Value && b.ProductId == productId && b.Quantity > 0)
            .OrderBy(b => b.ExpiryDate)
            .ToListAsync();

        return Ok(ApiResult<List<ProductBatch>>.Success(list));
    }
}

public class StocktakeRequest
{
    public Guid BranchId { get; set; }
    public List<StocktakeItem> Items { get; set; } = [];
}

public class StocktakeItem
{
    public Guid ProductId { get; set; }
    public string? BatchCode { get; set; }
    public int PhysicalQuantity { get; set; }
}
