using GoodHamburger.Api.Domain.Enums;

namespace GoodHamburger.Api.Application.DTOs;

// ── Requests ────────────────────────────────────────────────────────────────

public record CreateOrderRequest(
    List<MenuItemCode> Items
);

public record UpdateOrderRequest(
    List<MenuItemCode> Items
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
