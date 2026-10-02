using System.ComponentModel.DataAnnotations;

namespace GestionProductosMvc.Models;

public class Producto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripcion del producto es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    [Range(1, 1000000, ErrorMessage = "El precio debe ser mayor que cero.")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La categoria del producto es obligatoria.")]
    public string Categoria { get; set; } = string.Empty;

    public bool Disponible { get; set; }
}
