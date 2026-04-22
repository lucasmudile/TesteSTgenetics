using FluentAssertions;
using GoodHamburger.Api.Application.DTOs;
using GoodHamburger.Api.Application.Services;
using GoodHamburger.Api.Domain.Enums;

namespace GoodHamburger.Tests;



/// <summary>
/// Service para os testes.
/// </summary>
/// 
public class OrderServiceTests
{
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(new FakeOrderRepository());
    }

   

    [Fact]
    public async Task Create_SandwichFriesSoda_ShouldApply20PercentDiscount()
    {
        // Arrange
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
        // Arrange:
        var request = new CreateOrderRequest([MenuItemCode.XBacon, MenuItemCode.Soda]);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.DiscountPercentage.Should().Be(15m);
        result.Subtotal.Should().Be(9.50m);
       
        result.DiscountAmount.Should().Be(1.42m);
        result.Total.Should().Be(8.08m);
    }

    [Fact]
    public async Task Create_SandwichFries_ShouldApply10PercentDiscount()
    {
        // Arrange:
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
        // Arrange:
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
        // Arrange:
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
        // Arrange:
        var created = await _sut.CreateAsync(new CreateOrderRequest([MenuItemCode.XBurger]));

        // Act:
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
