using System.Net.Http.Json;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public class ApiService(HttpClient http)
{
    private readonly HttpClient _http = http;

    // ── Menu ────────────────────────────────────────────────────────────────
    public Task<List<MenuItemModel>?> GetMenuAsync() =>
        _http.GetFromJsonAsync<List<MenuItemModel>>("api/menu");

    // ── Orders ───────────────────────────────────────────────────────────────
    public Task<List<OrderModel>?> GetOrdersAsync() =>
        _http.GetFromJsonAsync<List<OrderModel>>("api/orders");

    public Task<OrderModel?> GetOrderAsync(Guid id) =>
        _http.GetFromJsonAsync<OrderModel>($"api/orders/{id}");

    public async Task<OrderModel?> CreateOrderAsync(CreateOrderRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/orders", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<OrderModel>();
    }

    public async Task<OrderModel?> UpdateOrderAsync(Guid id, CreateOrderRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/orders/{id}", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<OrderModel>();
    }

    public async Task<bool> DeleteOrderAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/orders/{id}");
        return response.IsSuccessStatusCode;
    }

    /// <summary>Extrai a mensagem de erro de um ProblemDetails retornado pela API.</summary>
    public async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsModel>();
            return problem?.Detail ?? problem?.Title ?? "Erro inesperado.";
        }
        catch
        {
            return "Erro inesperado ao comunicar com a API.";
        }
    }
}
