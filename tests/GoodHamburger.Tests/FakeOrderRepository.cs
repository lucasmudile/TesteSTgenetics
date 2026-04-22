using GoodHamburger.Api.Application.DTOs;
using GoodHamburger.Api.Application.Interfaces;
using GoodHamburger.Api.Domain.Entities;

namespace GoodHamburger.Tests;


/// <summary>
/// Repositório em memória para os testes.
/// </summary>
public class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _store = new();

    
    public void Seed(params Order[] orders)
    {
        foreach (var o in orders) _store[o.Id] = o;
    }

    public Task<IEnumerable<Order>> GetAllAsync() =>
        Task.FromResult<IEnumerable<Order>>(_store.Values.ToList());

    public Task<(List<Order> Items, int TotalCount)> GetPagedAsync(OrderQuery q)
    {
        var query = _store.Values.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var term = q.Search.Trim().ToLowerInvariant();
            query = query.Where(o => o.Id.ToString().ToLowerInvariant().Contains(term));
        }

        if (q.MinTotal.HasValue)    query = query.Where(o => o.Total >= q.MinTotal.Value);
        if (q.MaxTotal.HasValue)    query = query.Where(o => o.Total <= q.MaxTotal.Value);

        if (q.HasDiscount.HasValue)
            query = q.HasDiscount.Value
                ? query.Where(o => o.DiscountPercentage > 0)
                : query.Where(o => o.DiscountPercentage == 0);

        if (q.DateFrom.HasValue)
            query = query.Where(o => o.CreatedAt >= q.DateFrom.Value.ToUniversalTime());
        if (q.DateTo.HasValue)
            query = query.Where(o => o.CreatedAt <= q.DateTo.Value.ToUniversalTime().AddDays(1).AddTicks(-1));

        query = (q.SortBy?.ToLower(), q.SortDesc) switch
        {
            ("total",    true)  => query.OrderByDescending(o => o.Total),
            ("total",    false) => query.OrderBy(o => o.Total),
            ("discount", true)  => query.OrderByDescending(o => o.DiscountPercentage),
            ("discount", false) => query.OrderBy(o => o.DiscountPercentage),
            (_,          false) => query.OrderBy(o => o.CreatedAt),
            _                   => query.OrderByDescending(o => o.CreatedAt),
        };

        var totalCount = query.Count();
        var pageSize   = Math.Max(1, Math.Min(q.PageSize, 100));
        var page       = Math.Max(1, q.Page);
        var items      = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult((items, totalCount));
    }

    public Task<Order?> GetByIdAsync(Guid id) =>
        Task.FromResult(_store.TryGetValue(id, out var o) ? o : null);

    public Task<Order> CreateAsync(Order order)
    {
        _store[order.Id] = order;
        return Task.FromResult(order);
    }

    public Task<Order> UpdateAsync(Order order)
    {
        _store[order.Id] = order;
        return Task.FromResult(order);
    }

    public Task<bool> DeleteAsync(Guid id) =>
        Task.FromResult(_store.Remove(id));
}
