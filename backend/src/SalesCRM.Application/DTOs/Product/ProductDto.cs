namespace SalesCRM.Application.DTOs.Product;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string? Barcode { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal ImportPrice { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
