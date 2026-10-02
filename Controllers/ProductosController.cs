using GestionProductosMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GestionProductosMvc.Controllers;

public class ProductosController : Controller
{
    private readonly IMemoryCache _cache;
    private const string ClaveCacheProductos = "lista_productos";

    private static readonly List<Producto> Productos =
    [
        new Producto
        {
            Id = 1,
            Nombre = "Teclado inalambrico",
            Descripcion = "Teclado compacto para oficina y estudio.",
            Precio = 18500,
            Categoria = "Accesorios",
            Disponible = true
        },
        new Producto
        {
            Id = 2,
            Nombre = "Mouse ergonomico",
            Descripcion = "Mouse comodo para jornadas largas de trabajo.",
            Precio = 12500,
            Categoria = "Accesorios",
            Disponible = true
        },
        new Producto
        {
            Id = 3,
            Nombre = "Monitor LED 24 pulgadas",
            Descripcion = "Monitor de 24 pulgadas con resolucion Full HD.",
            Precio = 95000,
            Categoria = "Monitores",
            Disponible = false
        }
    ];

    public ProductosController(IMemoryCache cache)
    {
        _cache = cache;
    }

    public IActionResult Index()
    {
        // ViewData: envía el título al layout y a la vista.
        ViewData["Title"] = "Listado de productos";

        // ViewBag: envía un mensaje simple a la vista.
        ViewBag.Mensaje = "Productos disponibles";

        // Caché en memoria: evita regenerar la lista si ya fue almacenada.
        if (!_cache.TryGetValue(ClaveCacheProductos, out List<Producto>? productosEnCache))
        {
            productosEnCache = Productos;

            var opciones = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            _cache.Set(ClaveCacheProductos, productosEnCache, opciones);
        }

        return View(productosEnCache);
    }

    public IActionResult Detalle(int id)
    {
        var producto = Productos.FirstOrDefault(p => p.Id == id);

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
    public IActionResult DetalleDirecto(int id)
    {
        var producto = Productos.FirstOrDefault(p => p.Id == id);

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
        return View(new Producto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Producto producto)
    {
        if (!ModelState.IsValid)
        {
            return View(producto);
        }

        producto.Id = Productos.Max(p => p.Id) + 1;
        Productos.Add(producto);

        // Invalidar el caché para que la próxima solicitud refleje el nuevo producto.
        _cache.Remove(ClaveCacheProductos);

        return RedirectToAction(nameof(Index));
    }

    // Ejemplo de caché con GetOrCreate (sección 11.5 del material).
    public IActionResult Categorias()
    {
        var categorias = _cache.GetOrCreate("categorias", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return new List<string> { "Accesorios", "Monitores", "Computadoras" };
        });

        return View(categorias);
    }
}
