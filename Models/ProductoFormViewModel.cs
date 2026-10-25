using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionProductosMvc.Models;

public class ProductoFormViewModel
{
    public Producto Producto { get; set; } = new();

    public List<SelectListItem> Categorias { get; set; } = [];
}
