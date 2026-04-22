using GoodHamburger.Api.Domain.Enums;

namespace GoodHamburger.Api.Application.DTOs;

// ── Requests ────────────────────────────────────────────────────────────────

public record CreateOrderRequest(
    List<MenuItemCode> Items
);

public record UpdateOrderRequest(
    List<MenuItemCode> Items
);

// ── Query / Pagination ───────────────────────────────────────────────────────

public record OrderQuery(
    string?   Search      = null,   
    decimal?  MinTotal    = null,
    decimal?  MaxTotal    = null,
    bool?     HasDiscount = null,  
    DateTime? DateFrom    = null,
    DateTime? DateTo      = null,
    string    SortBy      = "date",
    bool      SortDesc    = true,
    int       Page        = 1,
    int       PageSize    = 10
);

public record PagedResult<T>(
    List<T> Items,
    int     TotalCount,
    int     Page,
    int     PageSize,
    int     TotalPages
);

// ── Responses ───────────────────────────────────────────────────────────────

public record MenuItemResponse(
    MenuItemCode Code,
    string Name,
    decimal Price,
    string Type
);

public record OrderItemResponse(
    MenuItemCode Code,
    string Name,
    decimal Price,
    string Type
);

public record OrderResponse(
    Guid Id,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<OrderItemResponse> Items,
    decimal Subtotal,
    decimal DiscountPercentage,
    decimal DiscountAmount,
    decimal Total
);

