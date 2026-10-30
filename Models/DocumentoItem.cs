namespace AsesoriaLegalGlobal.Models;

public class DocumentoItem
{
    public string Nombre { get; set; } = string.Empty;
    public string NombreOriginal { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public DateTime Fecha { get; set; }

    public string TamanoLegible => TamanoBytes < 1024
        ? $"{TamanoBytes} B"
        : TamanoBytes < 1024 * 1024
            ? $"{TamanoBytes / 1024.0:0.0} KB"
            : $"{TamanoBytes / (1024.0 * 1024.0):0.00} MB";
}

