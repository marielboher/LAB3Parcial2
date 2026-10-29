# Asesoría Legal Global

Sitio web en **ASP.NET Core Razor Pages** para el Parcial 2 TP4 (LP3): registro de clientes y gestión segura de documentos legales.

## Cómo ejecutarlo

Requisitos: [.NET 8 SDK](https://dotnet.microsoft.com/download) o Visual Studio 2022.

```bash
cd AsesoriaLegalGlobal
dotnet run
```

Luego abrir `https://localhost:7165` o `http://localhost:5216`.

También se puede abrir `AsesoriaLegalGlobal.sln` en Visual Studio y presionar F5.

## Recorrido sugerido para el video

1. **Inicio:** mostrar navbar, logos de la empresa, secciones y footer.
2. **Registro:** intentar enviar vacío, correo inválido, edad 15 o menor y contraseñas distintas. Luego registrar un usuario válido.
3. **Cookies y sesión:** en DevTools (F12) → Application, mostrar la cookie `ContrasenaUsuario` y la sesión `.AsesoriaLegal.Session`.
4. **Login:** iniciar sesión y comprobar la redirección a *Mi espacio*.
5. **Documentos:** subir un PDF (o imagen), listarlo, descargarlo y rechazar un archivo no permitido o demasiado grande.
6. **Responsive:** achicar la ventana o usar la vista móvil del navegador.

## Requisitos cubiertos

| Criterio | Dónde se ve |
| --- | --- |
| Layout / Master Page | `Pages/Shared/_Layout.cshtml` |
| Navbar con enlaces | Inicio, Registro, Iniciar sesión, Mi espacio |
| Footer en todas las páginas | `_Layout.cshtml` |
| Campos de registro | correo, usuario, edad, contraseñas, nombre y apellido |
| Validaciones | obligatorios, formato de correo, coincidencia de contraseñas, edad > 15 |
| Cookie y sesión | contraseña en cookie `ContrasenaUsuario`; usuario en sesión |
| Redirección post-login | `Login` → `Gestion` |
| Subida segura | extensión, content-type, tamaño 5 MB y carpeta por usuario |
| CSS responsivo | `wwwroot/css/site.css` |
| Imágenes de la empresa | `wwwroot/images/` (las 3 logos del enunciado) |

## Flujo técnico

1. Al hacer clic en **Registrar**, el nombre de usuario se guarda en **sesión** y la contraseña en una **cookie**.
2. Al **iniciar sesión**, se comparan esos valores y se marca al usuario como autenticado.
3. El espacio personal (`/Gestion`) solo se abre si hay sesión autenticada.
4. Los archivos se guardan fuera de `wwwroot`, en `App_Data/documentos/{usuario}/`.
