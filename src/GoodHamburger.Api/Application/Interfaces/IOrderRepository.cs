using GoodHamburger.Api.Application.DTOs;
using GoodHamburger.Api.Domain.Entities;

namespace GoodHamburger.Api.Application.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<(List<Order> Items, int TotalCount)> GetPagedAsync(OrderQuery query);
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<bool> DeleteAsync(Guid id);
}
