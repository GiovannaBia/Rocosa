using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocosa_Modelos;

namespace Rocosa_AccesoDatos.Datos.Repositorio
{
    public class TipoAplicacionRepositorio : Repositorio<TipoAplicacion>, ITipoAplicacionRepositorio
    {
        private readonly ApplicationDBContext _db;
        public TipoAplicacionRepositorio(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(TipoAplicacion tipoAplicacion)
        {
            var tipoAnterior = _db.TipoAplicacion.FirstOrDefault(t => t.Id == tipoAplicacion.Id);
            if (tipoAnterior != null)
            {
                tipoAnterior.Nombre = tipoAplicacion.Nombre;
            }
        }
    }
}
