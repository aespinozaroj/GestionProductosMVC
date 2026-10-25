namespace GestionProductosMvc.Models;

public abstract class Persona
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
}
