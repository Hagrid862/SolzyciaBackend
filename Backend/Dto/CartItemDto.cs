﻿using Backend.Models;

namespace Backend;

public class CartItemDto
{
    public required bool IsEvent { get; set; }
    public required string ItemId { get; set; }
    public required string Name { get; set; }
    public required float Price { get; set; }
    public required int Quantity { get; set; }
    public required Image? Image { get; set; }
    public bool IsOnSale { get; set; } = false;
    public string? SalePrice { get; set; } = null;
    public string? SaleEndDate { get; set; } = null;
    public string? IsArchived { get; set; } = null;
    public string? IsDeleted { get; set; } = null;
}
