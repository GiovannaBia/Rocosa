using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using Rocosa_Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocosa_AccesoDatos.Datos.Repositorio
{
    public class CategoriaRepositorio : Repositorio<Categoria>, ICategoriaRepositorio
    {
        private readonly ApplicationDBContext _db;
        public CategoriaRepositorio(ApplicationDBContext db ) : base(db)
        {
                _db = db;   
        }
        public void Actualizar(Categoria categoria)
        {
            var catAnterior = _db.Categoria.FirstOrDefault(c=>c.Id == categoria.Id);
            if (catAnterior != null) 
            {
                catAnterior.NombreCategoria = categoria.NombreCategoria;
                catAnterior.MostrarOrden = categoria.MostrarOrden;
            }
        }
    }
}
