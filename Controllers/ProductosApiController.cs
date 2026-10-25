using GestionProductosMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionProductosMvc.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductosApiController : ControllerBase
{
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

    [HttpGet]
    public ActionResult<List<Producto>> ObtenerTodos()
    {
        return Productos;
    }

    [HttpGet("{id:int}")]
    public ActionResult<Producto> ObtenerPorId(int id)
    {
        var producto = Productos.FirstOrDefault(p => p.Id == id);

        if (producto is null)
        {
            return NotFound();
        }

        return producto;
    }

    [HttpPost]
    public ActionResult<Producto> Crear(Producto producto)
    {
        producto.Id = Productos.Max(p => p.Id) + 1;
        Productos.Add(producto);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
    }
}
