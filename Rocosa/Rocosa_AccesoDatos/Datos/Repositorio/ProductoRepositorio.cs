using Microsoft.AspNetCore.Mvc.Rendering;
using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using Rocosa_Modelos;
using Rocosa_Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocosa_AccesoDatos.Datos.Repositorio
{
    public class ProductoRepositorio : Repositorio<Producto>, IProductoRepositorio
    {
        private readonly ApplicationDBContext _db;
        public ProductoRepositorio(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Producto producto)
        {
            _db.Update(producto);
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropDownList(string obj)
        {
            if (obj == WC.CategoriaNombre)
            {
                return _db.Categoria.Select(c => new SelectListItem
                {                               //Por cada elemento c de la tabla se crea un listitem
                    Text = c.NombreCategoria,  //display 
                    Value = c.Id.ToString()   //valor
                });
            }
            if (obj == WC.TipoAplicacionNombre)
            {
               return _db.TipoAplicacion.Select(t => new SelectListItem
                {
                    Text = t.Nombre,
                    Value = t.Id.ToString()
                });
            }
            return null;
        }
    }
}
