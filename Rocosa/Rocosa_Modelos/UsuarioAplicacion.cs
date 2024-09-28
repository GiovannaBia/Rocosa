using Microsoft.AspNetCore.Identity;

namespace Rocosa_Modelos
{
    public class UsuarioAplicacion : IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}
