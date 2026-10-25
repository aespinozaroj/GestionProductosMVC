using GestionProductosMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionProductosMvc.Controllers;

public class AccesoController : Controller
{
    private static readonly List<Administrador> Administradores =
    [
        new Administrador
        {
            Id = 1,
            NombreUsuario = "admin",
            Contrasena = "admin123"
        }
    ];

    private static readonly List<Empleado> Empleados =
    [
        new Empleado
        {
            Id = 2,
            NombreUsuario = "jrodriguez",
            Contrasena = "empleado123",
            Departamento = "Bodega"
        }
    ];

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string nombreUsuario, string contrasena)
    {
        var administrador = Administradores.FirstOrDefault(p =>
            p.NombreUsuario == nombreUsuario && p.Contrasena == contrasena);
        if (administrador is not null)
        {
            HttpContext.Session.SetString("UsuarioId",
                administrador.Id.ToString());
            HttpContext.Session.SetString("UsuarioRol", "Administrador");
            return RedirectToAction("Index", "Productos");
        }

        var empleado = Empleados.FirstOrDefault(p =>
            p.NombreUsuario == nombreUsuario && p.Contrasena == contrasena);
        if (empleado is not null)
        {
            HttpContext.Session.SetString("UsuarioId",
                empleado.Id.ToString());
            HttpContext.Session.SetString("UsuarioRol", "Empleado");
            return RedirectToAction("Index", "Productos");
        }

        ViewBag.Error = "Usuario o contraseña incorrectos.";
        return View();
    }

    public IActionResult Salir()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
