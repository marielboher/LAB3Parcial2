using AsesoriaLegalGlobal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsesoriaLegalGlobal.Pages;

[RequestSizeLimit(6 * 1024 * 1024)]
[RequestFormLimits(MultipartBodyLengthLimit = 6 * 1024 * 1024)]
public class GestionModel : PageModel
{
    private static readonly HashSet<string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".txt", ".jpg", ".jpeg", ".png"
    };

    private static readonly HashSet<string> ContentTypesPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "text/plain",
        "image/jpeg",
        "image/png"
    };

    private const long TamanoMaximo = 5 * 1024 * 1024;
    private readonly IWebHostEnvironment _entorno;

    public GestionModel(IWebHostEnvironment entorno)
    {
        _entorno = entorno;
    }

    public string Usuario { get; private set; } = string.Empty;
    public string NombreCompleto { get; private set; } = string.Empty;
    public string Correo { get; private set; } = string.Empty;
    public string Edad { get; private set; } = string.Empty;
    public string? MensajeOk { get; set; }
    public string? MensajeError { get; set; }
    public List<DocumentoItem> Documentos { get; private set; } = new();

    [BindProperty]
    public IFormFile? Documento { get; set; }

    public IActionResult OnGet()
    {
        if (!EstaAutenticado())
        {
            TempData["Mensaje"] = "Debe iniciar sesión para acceder a su espacio de documentos.";
            return RedirectToPage("/Login");
        }

        CargarDatos();
        return Page();
    }

    public async Task<IActionResult> OnPostSubirAsync()
    {
        if (!EstaAutenticado())
        {
            return RedirectToPage("/Login");
        }

        CargarDatos();

        if (Documento is null || Documento.Length == 0)
        {
            MensajeError = "Seleccione un archivo para subir.";
            return Page();
        }

        if (Documento.Length > TamanoMaximo)
        {
            MensajeError = "El archivo supera el tamaño máximo de 5 MB.";
            return Page();
        }

        var extension = Path.GetExtension(Documento.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !ExtensionesPermitidas.Contains(extension))
        {
            MensajeError = "Tipo de archivo no permitido. Use PDF, DOC, DOCX, TXT, JPG o PNG.";
            return Page();
        }

        if (!string.IsNullOrWhiteSpace(Documento.ContentType) && !ContentTypesPermitidos.Contains(Documento.ContentType))
        {
            MensajeError = "El contenido del archivo no es válido.";
            return Page();
        }

        var carpeta = CarpetaUsuario();
        Directory.CreateDirectory(carpeta);

        var nombreOriginal = Path.GetFileName(Documento.FileName);
        foreach (var caracter in Path.GetInvalidFileNameChars())
        {
            nombreOriginal = nombreOriginal.Replace(caracter, '_');
        }

        var nombreFinal = $"{DateTime.Now:yyyyMMddHHmmss}_{nombreOriginal}";
        var ruta = Path.Combine(carpeta, nombreFinal);

        await using (var stream = System.IO.File.Create(ruta))
        {
            await Documento.CopyToAsync(stream);
        }

        MensajeOk = $"El documento \"{nombreOriginal}\" se cargó correctamente.";
        CargarDatos();
        return Page();
    }

    public IActionResult OnGetVer(string archivo)
    {
        if (!EstaAutenticado())
        {
            return RedirectToPage("/Login");
        }

        var ruta = RutaSegura(archivo);
        if (ruta is null)
        {
            return NotFound();
        }

        var nombreVisible = ExtraerNombreOriginal(Path.GetFileName(ruta));
        var contentType = ObtenerContentType(Path.GetExtension(ruta));
        Response.Headers.ContentDisposition = $"inline; filename=\"{nombreVisible}\"";
        return PhysicalFile(ruta, contentType);
    }

    public IActionResult OnPostEliminar(string archivo)
    {
        if (!EstaAutenticado())
        {
            return RedirectToPage("/Login");
        }

        var ruta = RutaSegura(archivo);
        if (ruta is not null && System.IO.File.Exists(ruta))
        {
            System.IO.File.Delete(ruta);
            MensajeOk = "El documento se eliminó correctamente.";
        }
        else
        {
            MensajeError = "No se encontró el documento indicado.";
        }

        CargarDatos();
        return Page();
    }

    private bool EstaAutenticado() =>
        HttpContext.Session.GetString("Autenticado") == "true"
        && !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Usuario"));

    private void CargarDatos()
    {
        Usuario = HttpContext.Session.GetString("Usuario") ?? string.Empty;
        var nombre = HttpContext.Session.GetString("Nombre") ?? string.Empty;
        var apellido = HttpContext.Session.GetString("Apellido") ?? string.Empty;
        NombreCompleto = $"{nombre} {apellido}".Trim();
        if (string.IsNullOrWhiteSpace(NombreCompleto))
        {
            NombreCompleto = Usuario;
        }

        Correo = HttpContext.Session.GetString("Correo") ?? "-";
        Edad = HttpContext.Session.GetString("Edad") ?? "-";
        Documentos = ListarDocumentos();
    }

    private List<DocumentoItem> ListarDocumentos()
    {
        var carpeta = CarpetaUsuario();
        if (!Directory.Exists(carpeta))
        {
            return new List<DocumentoItem>();
        }

        return Directory.GetFiles(carpeta)
            .Select(ruta => new FileInfo(ruta))
            .OrderByDescending(info => info.CreationTime)
            .Select(info => new DocumentoItem
            {
                Nombre = info.Name,
                NombreOriginal = ExtraerNombreOriginal(info.Name),
                TamanoBytes = info.Length,
                Fecha = info.CreationTime
            })
            .ToList();
    }

    private string CarpetaUsuario()
    {
        var usuario = HttpContext.Session.GetString("Usuario") ?? "anonimo";
        foreach (var caracter in Path.GetInvalidFileNameChars())
        {
            usuario = usuario.Replace(caracter, '_');
        }

        return Path.Combine(_entorno.WebRootPath, "documentos", usuario);
    }

    private string? RutaSegura(string? archivo)
    {
        if (string.IsNullOrWhiteSpace(archivo))
        {
            return null;
        }

        var nombre = Path.GetFileName(archivo);
        var ruta = Path.GetFullPath(Path.Combine(CarpetaUsuario(), nombre));
        var raiz = Path.GetFullPath(CarpetaUsuario());

        if (!ruta.StartsWith(raiz, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(ruta))
        {
            return null;
        }

        return ruta;
    }

    private static string ExtraerNombreOriginal(string nombreArchivo)
    {
        var posicion = nombreArchivo.IndexOf('_');
        return posicion > 0 && posicion < nombreArchivo.Length - 1
            ? nombreArchivo[(posicion + 1)..]
            : nombreArchivo;
    }

    private static string ObtenerContentType(string extension) => extension.ToLowerInvariant() switch
    {
        ".pdf" => "application/pdf",
        ".txt" => "text/plain; charset=utf-8",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".doc" => "application/msword",
        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        _ => "application/octet-stream"
    };
}
