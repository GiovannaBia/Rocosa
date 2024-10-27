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
    public class VentaDetalleRepositorio : Repositorio<VentaDetalle>, IVentaDetalleRepositorio
    {
        private readonly ApplicationDBContext _db;
        public VentaDetalleRepositorio(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(VentaDetalle ventaDetalle)
        {
            _db.Update(ventaDetalle);
        } 
       
    }
}
