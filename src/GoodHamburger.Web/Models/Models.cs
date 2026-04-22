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

// Filtros e Paginação

public class OrderQueryModel
{
    public string? Search { get; set; }
    public decimal? MinTotal { get; set; }
    public decimal? MaxTotal { get; set; }
    public bool? HasDiscount { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string SortBy { get; set; } = "date";
    public bool SortDesc { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public bool HasActiveFilters =>
        !string.IsNullOrWhiteSpace(Search) ||
        MinTotal.HasValue || MaxTotal.HasValue ||
        HasDiscount.HasValue ||
        DateFrom.HasValue || DateTo.HasValue;
}

public class PagedResultModel<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

