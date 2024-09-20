using Microsoft.AspNetCore.Identity;

namespace Rocosa.Models
{
    public class UsuarioAplicacion : IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}
