using FluentAssertions;
using GoodHamburger.Api.Application.DTOs;
using GoodHamburger.Api.Application.Interfaces;
using GoodHamburger.Api.Application.Services;
using GoodHamburger.Api.Domain.Entities;
using GoodHamburger.Api.Domain.Enums;

namespace GoodHamburger.Tests;

// ── Fake Repository ──────────────────────────────────────────────────────────

/// <summary>
/// Repositório em memória para uso nos testes, sem concorrência e totalmente
/// controlado. Evita dependência de infraestrutura real.
/// </summary>
public class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _store = new();

    public Task<IEnumerable<Order>> GetAllAsync() =>
        Task.FromResult<IEnumerable<Order>>(_store.Values.ToList());

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

// ── Tests ────────────────────────────────────────────────────────────────────

public class OrderServiceTests
{
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(new FakeOrderRepository());
    }

    // ─── Discount Rules ────────────────────────────────────────────────────

    [Fact]
    public async Task Create_SandwichFriesSoda_ShouldApply20PercentDiscount()
    {
        // Arrange: X Burger (5) + Batata (2) + Refrigerante (2.5) = 9.5
        var request = new CreateOrderRequest([MenuItemCode.XBurger, MenuItemCode.Fries, MenuItemCode.Soda]);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.DiscountPercentage.Should().Be(20m);
        result.Subtotal.Should().Be(9.50m);
        result.DiscountAmount.Should().Be(1.90m);
        result.Total.Should().Be(7.60m);
    }

    [Fact]
    public async Task Create_SandwichSoda_ShouldApply15PercentDiscount()
    {
        // Arrange: X Bacon (7) + Refrigerante (2.5) = 9.5
        var request = new CreateOrderRequest([MenuItemCode.XBacon, MenuItemCode.Soda]);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.DiscountPercentage.Should().Be(15m);
        result.Subtotal.Should().Be(9.50m);
        // Math.Round com MidpointRounding.ToEven (padrão .NET): 1.425 → 1.42
        result.DiscountAmount.Should().Be(1.42m);
        result.Total.Should().Be(8.08m);
    }

    [Fact]
    public async Task Create_SandwichFries_ShouldApply10PercentDiscount()
    {
        // Arrange: X Egg (4.5) + Batata (2) = 6.5
        var request = new CreateOrderRequest([MenuItemCode.XEgg, MenuItemCode.Fries]);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.DiscountPercentage.Should().Be(10m);
        result.Subtotal.Should().Be(6.50m);
        result.DiscountAmount.Should().Be(0.65m);
        result.Total.Should().Be(5.85m);
    }

    [Fact]
    public async Task Create_SandwichOnly_ShouldApplyNoDiscount()
    {
        // Arrange: X Burger (5) = 5
        var request = new CreateOrderRequest([MenuItemCode.XBurger]);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.DiscountPercentage.Should().Be(0m);
        result.DiscountAmount.Should().Be(0m);
        result.Total.Should().Be(5.00m);
    }

    [Fact]
    public async Task Create_FriesOnly_ShouldApplyNoDiscount()
    {
        // Arrange
        var request = new CreateOrderRequest([MenuItemCode.Fries]);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.DiscountPercentage.Should().Be(0m);
        result.Total.Should().Be(2.00m);
    }

    // ─── Validation Rules ──────────────────────────────────────────────────

    [Fact]
    public async Task Create_DuplicateSandwich_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateOrderRequest([MenuItemCode.XBurger, MenuItemCode.XBurger]);

        // Act
        var act = () => _sut.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*duplicados*");
    }

    [Fact]
    public async Task Create_TwoSandwichTypes_ShouldThrowArgumentException()
    {
        // Arrange: dois sanduíches diferentes
        var request = new CreateOrderRequest([MenuItemCode.XBurger, MenuItemCode.XEgg]);

        // Act
        var act = () => _sut.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*apenas um sanduíche*");
    }

    [Fact]
    public async Task Create_EmptyItems_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateOrderRequest([]);

        // Act
        var act = () => _sut.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*ao menos um item*");
    }

    [Fact]
    public async Task Create_DuplicateFries_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateOrderRequest([MenuItemCode.XBurger, MenuItemCode.Fries, MenuItemCode.Fries]);

        // Act
        var act = () => _sut.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    // ─── CRUD Operations ───────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_AfterCreate_ShouldReturnCreatedOrder()
    {
        // Arrange
        var request = new CreateOrderRequest([MenuItemCode.XBurger, MenuItemCode.Soda]);
        var created = await _sut.CreateAsync(request);

        // Act
        var all = (await _sut.GetAllAsync()).ToList();

        // Assert
        all.Should().ContainSingle(o => o.Id == created.Id);
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturnOrder()
    {
        // Arrange
        var created = await _sut.CreateAsync(
            new CreateOrderRequest([MenuItemCode.XBacon, MenuItemCode.Fries]));

        // Act
        var found = await _sut.GetByIdAsync(created.Id);

        // Assert
        found.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ShouldThrowKeyNotFoundException()
    {
        // Act
        var act = () => _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Update_ExistingOrder_ShouldRecalculateDiscount()
    {
        // Arrange: criar sem desconto
        var created = await _sut.CreateAsync(new CreateOrderRequest([MenuItemCode.XBurger]));

        // Act: atualizar para combo completo
        var updateRequest = new UpdateOrderRequest([MenuItemCode.XBurger, MenuItemCode.Fries, MenuItemCode.Soda]);
        var updated = await _sut.UpdateAsync(created.Id, updateRequest);

        // Assert
        updated.DiscountPercentage.Should().Be(20m);
    }

    [Fact]
    public async Task Delete_ExistingOrder_ShouldRemoveIt()
    {
        // Arrange
        var created = await _sut.CreateAsync(new CreateOrderRequest([MenuItemCode.XEgg]));

        // Act
        await _sut.DeleteAsync(created.Id);
        var act = () => _sut.GetByIdAsync(created.Id);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Delete_NonExistingOrder_ShouldThrowKeyNotFoundException()
    {
        // Act
        var act = () => _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
