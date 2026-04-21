using System.Collections.Concurrent;
using GoodHamburger.Api.Application.Interfaces;
using GoodHamburger.Api.Domain.Entities;

namespace GoodHamburger.Api.Infrastructure.Repositories;

/// <summary>
/// Repositório em memória thread-safe. Ideal para demonstração sem BD externo.
/// Em produção, substituir por EF Core + PostgreSQL/SQL Server.
/// </summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();

    public Task<IEnumerable<Order>> GetAllAsync() =>
        Task.FromResult<IEnumerable<Order>>(_store.Values.OrderByDescending(o => o.CreatedAt));

    public Task<Order?> GetByIdAsync(Guid id) =>
        Task.FromResult(_store.TryGetValue(id, out var order) ? order : null);

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
        Task.FromResult(_store.TryRemove(id, out _));
}
