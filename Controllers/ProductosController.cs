using GestionProductosMvc.Models;
using GestionProductosMvc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;

namespace GestionProductosMvc.Controllers;

public class ProductosController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ProductosApiClient _api;
    private const string ClaveCacheProductos = "lista_productos";

    public ProductosController(IMemoryCache cache, ProductosApiClient api)
    {
        _cache = cache;
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        // ViewData: envía el título al layout y a la vista.
        ViewData["Title"] = "Listado de productos";

        // ViewBag: envía un mensaje simple a la vista.
        ViewBag.Mensaje = "Productos disponibles";

        // Caché en memoria: evita volver a llamar a la API si ya se guardó la respuesta.
        if (!_cache.TryGetValue(ClaveCacheProductos, out List<Producto>? productosEnCache))
        {
            productosEnCache = await _api.ObtenerTodosAsync();

            var opciones = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            _cache.Set(ClaveCacheProductos, productosEnCache, opciones);
        }

        return View(productosEnCache);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var producto = await _api.ObtenerPorIdAsync(id);

        if (producto is null)
        {
            return NotFound();
        }

        ViewBag.EsPremium = producto.Precio > 50000;

        return View(producto);
    }

    // Ruta por atributos (sección 9.3 del material): además de la ruta por
    // convención heredada de Program.cs, esta acción también responde en
    // /productos/detalle-directo/{id}, sin importar el nombre del controlador o de la acción.
    [HttpGet("productos/detalle-directo/{id:int}")]
    public async Task<IActionResult> DetalleDirecto(int id)
    {
        var producto = await _api.ObtenerPorIdAsync(id);

        if (producto is null)
        {
            return NotFound();
        }

        ViewBag.EsPremium = producto.Precio > 50000;

        return View("Detalle", producto);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        if (!EsAdministrador())
        {
            return RedirectToAction("Login", "Acceso");
        }

        var modelo = new ProductoFormViewModel
        {
            Producto = new Producto(),
            Categorias = ObtenerCategoriasParaFormulario()
        };

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ProductoFormViewModel modelo)
    {
        if (!EsAdministrador())
        {
            return RedirectToAction("Login", "Acceso");
        }

        if (!ModelState.IsValid)
        {
            // El <select> necesita la lista de categorías en cada renderizado,
            // tanto en el GET inicial como aquí, si la validación falla.
            modelo.Categorias = ObtenerCategoriasParaFormulario();
            return View(modelo);
        }

        await _api.CrearAsync(modelo.Producto);

        // Invalidar el caché para que la próxima solicitud refleje el nuevo producto.
        _cache.Remove(ClaveCacheProductos);

        return RedirectToAction(nameof(Index));
    }

    // Ejemplo de caché con GetOrCreate (sección 11.5 del material).
    public IActionResult Categorias()
    {
        var categorias = ObtenerCategorias();

        return View(categorias);
    }

    private bool EsAdministrador()
    {
        return HttpContext.Session.GetString("UsuarioRol") == "Administrador";
    }

    private List<string> ObtenerCategorias()
    {
        return _cache.GetOrCreate("categorias", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return new List<string> { "Accesorios", "Monitores", "Computadoras" };
        })!;
    }

    private List<SelectListItem> ObtenerCategoriasParaFormulario()
    {
        return ObtenerCategorias()
            .Select(categoria => new SelectListItem(categoria, categoria))
            .ToList();
    }
}
