using System;

namespace SalesCRM.Application.DTOs.Inventory;

public class ProductLocationDto
{
    public Guid Id { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string BatchCode { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
