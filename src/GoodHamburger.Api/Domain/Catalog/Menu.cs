using GoodHamburger.Api.Domain.Entities;
using GoodHamburger.Api.Domain.Enums;

namespace GoodHamburger.Api.Domain.Catalog;

/// <summary>
/// Cardápio da lanchonete. Fonte de preços/nomes.
/// </summary>
public static class Menu
{
    public static readonly IReadOnlyList<MenuItem> Items = new List<MenuItem>
    {
        new() { Code = MenuItemCode.XBurger, Name = "X Burger", Price = 5.00m, Type = MenuItemType.Sandwich },
        new() { Code = MenuItemCode.XEgg,    Name = "X Egg",    Price = 4.50m, Type = MenuItemType.Sandwich },
        new() { Code = MenuItemCode.XBacon,  Name = "X Bacon",  Price = 7.00m, Type = MenuItemType.Sandwich },
        new() { Code = MenuItemCode.Fries,   Name = "Batata Frita", Price = 2.00m, Type = MenuItemType.Side },
        new() { Code = MenuItemCode.Soda,    Name = "Refrigerante", Price = 2.50m, Type = MenuItemType.Side }
    }.AsReadOnly();

    public static MenuItem? FindByCode(MenuItemCode code) =>
        Items.FirstOrDefault(i => i.Code == code);

    public static bool Exists(MenuItemCode code) =>
        Items.Any(i => i.Code == code);
}
