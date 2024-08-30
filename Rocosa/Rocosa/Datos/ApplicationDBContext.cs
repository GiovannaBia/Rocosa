using Microsoft.EntityFrameworkCore;
using Rocosa.Models;

namespace Rocosa.Datos
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options)
        {              
        }
        public DbSet<Categoria> Categoria { get; set; }
    }
}
