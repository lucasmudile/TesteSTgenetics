using System.Net.Http.Json;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public class ApiService(HttpClient http)
{
    private readonly HttpClient _http = http;

    public Task<List<MenuItemModel>?> GetMenuAsync() =>
        _http.GetFromJsonAsync<List<MenuItemModel>>("api/menu");

  
    public Task<List<OrderModel>?> GetOrdersAsync() =>
        _http.GetFromJsonAsync<List<OrderModel>>("api/orders");

    public async Task<PagedResultModel<OrderModel>?> GetOrdersPagedAsync(OrderQueryModel q)
    {
        var qs = new List<string>
        {
            $"page={q.Page}",
            $"pageSize={q.PageSize}",
            $"sortBy={Uri.EscapeDataString(q.SortBy)}",
            $"sortDesc={q.SortDesc.ToString().ToLower()}"
        };

        if (!string.IsNullOrWhiteSpace(q.Search))
            qs.Add($"search={Uri.EscapeDataString(q.Search.Trim())}");
        if (q.MinTotal.HasValue)
            qs.Add($"minTotal={q.MinTotal.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        if (q.MaxTotal.HasValue)
            qs.Add($"maxTotal={q.MaxTotal.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        if (q.HasDiscount.HasValue)
            qs.Add($"hasDiscount={q.HasDiscount.Value.ToString().ToLower()}");
        if (q.DateFrom.HasValue)
            qs.Add($"dateFrom={q.DateFrom.Value:yyyy-MM-dd}");
        if (q.DateTo.HasValue)
            qs.Add($"dateTo={q.DateTo.Value:yyyy-MM-dd}");

        var url = "api/orders/paged?" + string.Join("&", qs);
        return await _http.GetFromJsonAsync<PagedResultModel<OrderModel>>(url);
    }

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
