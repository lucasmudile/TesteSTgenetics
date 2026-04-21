using GoodHamburger.Api.Application.DTOs;
using GoodHamburger.Api.Domain.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MenuController : ControllerBase
{
    /// <summary>Retorna o cardápio completo com preços</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MenuItemResponse>), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        var items = Menu.Items.Select(i => new MenuItemResponse(
            i.Code,
            i.Name,
            i.Price,
            i.Type.ToString()
        ));
        return Ok(items);
    }
}
