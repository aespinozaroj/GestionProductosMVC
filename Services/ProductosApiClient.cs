using System.Net.Http.Json;
using GestionProductosMvc.Models;

namespace GestionProductosMvc.Services;

public class ProductosApiClient
{
    private readonly HttpClient _httpClient;

    public ProductosApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Producto>> ObtenerTodosAsync()
    {
        var productos = await _httpClient.GetFromJsonAsync<List<Producto>>("");

        return productos ?? [];
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id)
    {
        var respuesta = await _httpClient.GetAsync($"{id}");

        if (!respuesta.IsSuccessStatusCode)
        {
            return null;
        }

        return await respuesta.Content.ReadFromJsonAsync<Producto>();
    }

    public async Task CrearAsync(Producto producto)
    {
        var respuesta = await _httpClient.PostAsJsonAsync("", producto);
        respuesta.EnsureSuccessStatusCode();
    }
}
