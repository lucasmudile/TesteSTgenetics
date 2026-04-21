using System.Text.Json.Serialization;

namespace GoodHamburger.Web.Models;

public class MenuItemModel
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class OrderItemModel
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class OrderModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItemModel> Items { get; set; } = [];
    public decimal Subtotal { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
}

public class CreateOrderRequest
{
    [JsonPropertyName("items")]
    public List<int> Items { get; set; } = [];
}

public class ProblemDetailsModel
{
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public int? Status { get; set; }
}
