using GoodHamburger.Api.Application.DTOs;
using GoodHamburger.Api.Application.Interfaces;
using GoodHamburger.Api.Domain.Catalog;
using GoodHamburger.Api.Domain.Entities;
using GoodHamburger.Api.Domain.Enums;

namespace GoodHamburger.Api.Application.Services;

public class OrderService(IOrderRepository repository)
{
    private readonly IOrderRepository _repository = repository;

    // ── Mapping ─────────────────────────────────────────────────────────────

    private static OrderResponse MapToResponse(Order order) => new(
        order.Id,
        order.CreatedAt,
        order.UpdatedAt,
        order.Items.Select(i => new OrderItemResponse(
            i.Code, i.Name, i.Price, i.Type.ToString())).ToList(),
        order.Subtotal,
        order.DiscountPercentage,
        order.DiscountAmount,
        order.Total
    );

    // ── Queries ─────────────────────────────────────────────────────────────

    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        var orders = await _repository.GetAllAsync();
        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponse> GetByIdAsync(Guid id)
    {
        var order = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Pedido com ID '{id}' não encontrado.");
        return MapToResponse(order);
    }

    // ── Commands ────────────────────────────────────────────────────────────

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        var items = ResolveAndValidateItems(request.Items);
        var order = Order.Create(items);
        var created = await _repository.CreateAsync(order);
        return MapToResponse(created);
    }

    public async Task<OrderResponse> UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var order = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Pedido com ID '{id}' não encontrado.");

        var items = ResolveAndValidateItems(request.Items);
        order.UpdateItems(items);

        var updated = await _repository.UpdateAsync(order);
        return MapToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted)
            throw new KeyNotFoundException($"Pedido com ID '{id}' não encontrado.");
    }

    // ── Validation ──────────────────────────────────────────────────────────

    /// <summary>
    /// Valida e resolve os códigos de itens enviados pelo cliente.
    /// Regras:
    ///   • Ao menos um item deve ser informado.
    ///   • Todos os códigos devem existir no cardápio.
    ///   • Máximo de 1 sanduíche, 1 batata e 1 refrigerante.
    ///   • Itens duplicados não são permitidos.
    /// </summary>
    private static List<MenuItem> ResolveAndValidateItems(List<MenuItemCode> codes)
    {
        if (codes is null || codes.Count == 0)
            throw new ArgumentException("O pedido deve conter ao menos um item.");

        // Duplicates check
        var duplicates = codes.GroupBy(c => c).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicates.Count > 0)
        {
            var names = duplicates.Select(d => d.ToString());
            throw new ArgumentException(
                $"Itens duplicados não são permitidos: {string.Join(", ", names)}.");
        }

        // Resolve against catalog
        var items = new List<MenuItem>();
        foreach (var code in codes)
        {
            var item = Menu.FindByCode(code)
                ?? throw new ArgumentException($"Item '{code}' não existe no cardápio.");
            items.Add(item);
        }

        // Quantity constraints
        var sandwiches = items.Where(i => i.IsSandwich).ToList();
        var fries      = items.Where(i => i.IsFries).ToList();
        var sodas      = items.Where(i => i.IsSoda).ToList();

        if (sandwiches.Count > 1)
            throw new ArgumentException("Um pedido pode conter apenas um sanduíche.");
        if (fries.Count > 1)
            throw new ArgumentException("Um pedido pode conter apenas uma porção de batata frita.");
        if (sodas.Count > 1)
            throw new ArgumentException("Um pedido pode conter apenas um refrigerante.");

        return items;
    }
}
