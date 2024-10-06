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
    public class OrdenRepositorio : Repositorio<Orden>, IOrdenRepositorio
    {
        private readonly ApplicationDBContext _db;
        public OrdenRepositorio(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Orden orden)
        {
            _db.Update(orden);
        }

    }
}
