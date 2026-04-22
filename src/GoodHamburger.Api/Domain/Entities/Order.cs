namespace GoodHamburger.Api.Domain.Entities;

/// <summary>
/// Representa um pedido completo. As regras de negócio de desconto
/// </summary>
public class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    private readonly List<MenuItem> _items = [];
    public IReadOnlyList<MenuItem> Items => _items.AsReadOnly();

    public decimal Subtotal => _items.Sum(i => i.Price);

    public decimal DiscountPercentage => CalculateDiscountPercentage();

    public decimal DiscountAmount => Math.Round(Subtotal * DiscountPercentage / 100m, 2);

    public decimal Total => Math.Round(Subtotal - DiscountAmount, 2);

    public static Order Create(IEnumerable<MenuItem> items)
    {
        var order = new Order();
        foreach (var item in items)
            order._items.Add(item);
        return order;
    }

    public void UpdateItems(IEnumerable<MenuItem> newItems)
    {
        _items.Clear();
        foreach (var item in newItems)
            _items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Regras de desconto
    ///   Sanduíche + batata + refrigerante → 20%
    ///   Sanduíche + refrigerante           → 15%
    ///   Sanduíche + batata                 → 10%
    ///   Demais combinações                 → 0%
    /// </summary>
    private decimal CalculateDiscountPercentage()
    {
        bool hasSandwich = _items.Any(i => i.IsSandwich);
        bool hasFries    = _items.Any(i => i.IsFries);
        bool hasSoda     = _items.Any(i => i.IsSoda);

        return (hasSandwich, hasFries, hasSoda) switch
        {
            (true, true, true)   => 20m,
            (true, false, true)  => 15m,
            (true, true, false)  => 10m,
            _                    => 0m
        };
    }
}
