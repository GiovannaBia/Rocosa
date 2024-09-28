using Microsoft.EntityFrameworkCore;
using Rocosa_Modelos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Rocosa_AccesoDatos.Datos
{
    public class ApplicationDBContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options)
        {              
        }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<TipoAplicacion> TipoAplicacion { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<UsuarioAplicacion> UsuarioAplicacion { get; set; }

    }
}
