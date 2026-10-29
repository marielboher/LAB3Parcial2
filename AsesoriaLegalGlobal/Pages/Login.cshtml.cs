using AsesoriaLegalGlobal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsesoriaLegalGlobal.Pages;

public class LoginModel : PageModel
{
    [BindProperty]
    public LoginInputModel Input { get; set; } = new();

    public string? Mensaje { get; set; }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("Autenticado") == "true")
        {
            return RedirectToPage("/Gestion");
        }

        Mensaje = TempData["Mensaje"] as string;
        var usuarioSesion = HttpContext.Session.GetString("Usuario");
        if (!string.IsNullOrWhiteSpace(usuarioSesion))
        {
            Input.Usuario = usuarioSesion;
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var usuarioSesion = HttpContext.Session.GetString("Usuario");
        var cookiePass = Request.Cookies["ContrasenaUsuario"];

        if (string.IsNullOrWhiteSpace(usuarioSesion) || string.IsNullOrWhiteSpace(cookiePass))
        {
            ModelState.AddModelError(string.Empty, "No hay un usuario registrado. Complete el formulario de registro primero.");
            return Page();
        }

        var passwordGuardada = Uri.UnescapeDataString(cookiePass);
        var usuarioOk = string.Equals(Input.Usuario.Trim(), usuarioSesion, StringComparison.Ordinal);
        var passwordOk = string.Equals(Input.Contrasena, passwordGuardada, StringComparison.Ordinal);

        if (!usuarioOk || !passwordOk)
        {
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return Page();
        }

        HttpContext.Session.SetString("Autenticado", "true");
        return RedirectToPage("/Gestion");
    }
}
