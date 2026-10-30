using AsesoriaLegalGlobal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsesoriaLegalGlobal.Pages;

public class RegistroModel : PageModel
{
    [BindProperty]
    public RegistroInputModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        HttpContext.Session.SetString("Usuario", Input.Usuario.Trim());
        HttpContext.Session.SetString("Nombre", Input.Nombre.Trim());
        HttpContext.Session.SetString("Apellido", Input.Apellido.Trim());
        HttpContext.Session.SetString("Correo", Input.Correo.Trim());
        HttpContext.Session.SetString("Edad", Input.Edad!.Value.ToString());
        HttpContext.Session.SetString("Autenticado", "false");

        Response.Cookies.Append(
            "ContrasenaUsuario",
            Uri.EscapeDataString(Input.Contrasena),
            new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddHours(4),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });

        TempData["Mensaje"] = "Registro exitoso. Inicie sesión para continuar.";
        return RedirectToPage("/Login");
    }
}

