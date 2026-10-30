using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsesoriaLegalGlobal.Pages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        HttpContext.Session.SetString("Autenticado", "false");
        TempData["Mensaje"] = "Sesión cerrada. Puede volver a ingresar con el mismo usuario.";
        return RedirectToPage("/Login");
    }
}

