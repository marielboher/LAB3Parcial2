using System.ComponentModel.DataAnnotations;

namespace AsesoriaLegalGlobal.Models;

public class RegistroInputModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico con formato válido.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 30 caracteres.")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "El usuario solo puede contener letras, números, punto, guion y guion bajo.")]
    [Display(Name = "Nombre de usuario")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La edad es obligatoria.")]
    [Range(16, 120, ErrorMessage = "Solo se aceptan usuarios mayores de 15 años.")]
    [Display(Name = "Edad")]
    public int? Edad { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe confirmar la contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarContrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Ingrese un nombre válido.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Ingrese un apellido válido.")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;
}
