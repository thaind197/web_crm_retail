using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalesCRM.Domain.Entities;
using SalesCRM.Application.Interfaces.Services;

namespace SalesCRM.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<BranchInventory> BranchInventories => Set<BranchInventory>();
    public DbSet<ProductBatch> ProductBatches => Set<ProductBatch>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<ProductLocation> ProductLocations => Set<ProductLocation>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<StockTransferDetail> StockTransferDetails => Set<StockTransferDetail>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<ReturnOrder> ReturnOrders => Set<ReturnOrder>();
    public DbSet<ReturnOrderItem> ReturnOrderItems => Set<ReturnOrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Auditable / BaseEntity configurations if any (cascade delete settings, etc.)
        
        // Multi-Branch Global Query Filters
        modelBuilder.Entity<BranchInventory>().HasQueryFilter(x => _currentUserService.IsAdmin || x.BranchId == _currentUserService.BranchId);
        modelBuilder.Entity<ProductBatch>().HasQueryFilter(x => _currentUserService.IsAdmin || x.BranchId == _currentUserService.BranchId);
        modelBuilder.Entity<Location>().HasQueryFilter(x => _currentUserService.IsAdmin || x.BranchId == _currentUserService.BranchId);
        modelBuilder.Entity<ProductLocation>().HasQueryFilter(x => _currentUserService.IsAdmin || x.BranchId == _currentUserService.BranchId);
        modelBuilder.Entity<InventoryTransaction>().HasQueryFilter(x => _currentUserService.IsAdmin || x.BranchId == _currentUserService.BranchId);
        modelBuilder.Entity<Order>().HasQueryFilter(x => _currentUserService.IsAdmin || x.BranchId == _currentUserService.BranchId);
        modelBuilder.Entity<StockTransfer>().HasQueryFilter(x => _currentUserService.IsAdmin || x.FromBranchId == _currentUserService.BranchId || x.ToBranchId == _currentUserService.BranchId);
        modelBuilder.Entity<Shift>().HasQueryFilter(x => _currentUserService.IsAdmin || x.BranchId == _currentUserService.BranchId);
        modelBuilder.Entity<ReturnOrder>().HasQueryFilter(x => _currentUserService.IsAdmin || x.BranchId == _currentUserService.BranchId);

        // Configure Composite Keys / Indexes if needed
        modelBuilder.Entity<Product>()
            .HasIndex(x => x.Code)
            .IsUnique();

        modelBuilder.Entity<BranchInventory>()
            .HasIndex(x => new { x.BranchId, x.ProductId })
            .IsUnique();

        modelBuilder.Entity<ProductBatch>()
            .HasIndex(x => new { x.BranchId, x.ProductId, x.BatchCode })
            .IsUnique();

        modelBuilder.Entity<Location>()
            .HasIndex(x => new { x.BranchId, x.LocationCode })
            .IsUnique();

        // Configure double navigation properties for StockTransfer
        modelBuilder.Entity<StockTransfer>()
            .HasOne(x => x.FromBranch)
            .WithMany()
            .HasForeignKey(x => x.FromBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockTransfer>()
            .HasOne(x => x.ToBranch)
            .WithMany()
            .HasForeignKey(x => x.ToBranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService.Username ?? "System";
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = _currentUserService.Username ?? "System";
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
