using GoodHamburger.Api.Domain.Enums;

namespace GoodHamburger.Api.Domain.Entities;

public class MenuItem
{
    public MenuItemCode Code { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public MenuItemType Type { get; init; }

    public bool IsSandwich => Type == MenuItemType.Sandwich;
    public bool IsSide => Type == MenuItemType.Side;
    public bool IsFries => Code == MenuItemCode.Fries;
    public bool IsSoda => Code == MenuItemCode.Soda;
}
