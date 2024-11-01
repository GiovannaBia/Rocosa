using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using Rocosa_Modelos.ViewModels;
using Rocosa_Utilidades;

namespace Rocosa.Controllers
{
    public class VentaController : Controller
    {
        private readonly IVentaRepositorio _ventaRepo;
        private readonly IVentaDetalleRepositorio _ventaDetalleRepo;

        public VentaController(IVentaRepositorio ventaRepo, IVentaDetalleRepositorio ventaDetalleRepo )
        {
                _ventaRepo = ventaRepo;
                _ventaDetalleRepo = ventaDetalleRepo;
        }

        public IActionResult Index(string buscarNombre=null, string buscarEmail=null, string buscarTelefono = null, string Estado=null)
        {
            VentaVM ventaVM = new VentaVM()
            {
                VentaLista = _ventaRepo.ObtenerTodos(),
                EstadoLista = WC.ListaEstados.ToList().Select(l => new SelectListItem
                {
                    Text = l,
                    Value = l
                })
            };

            if(!string.IsNullOrEmpty(buscarNombre))
            {
                ventaVM.VentaLista = ventaVM.VentaLista.Where(l=>l.NombreCompleto.ToLower().Contains(buscarNombre.ToLower())); 
            }
            if (!string.IsNullOrEmpty(buscarEmail))
            {
                ventaVM.VentaLista = ventaVM.VentaLista.Where(l => l.Email.ToLower().Contains(buscarEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(buscarTelefono))
            {
                ventaVM.VentaLista = ventaVM.VentaLista.Where(l => l.Telefono.ToLower().Contains(buscarTelefono.ToLower()));
            }
            if (!string.IsNullOrEmpty(Estado) && Estado != "--Estado--")
            {
                ventaVM.VentaLista = ventaVM.VentaLista.Where(l => l.EstadoVenta.ToLower().Contains(Estado.ToLower()));
            }
            return View(ventaVM);
        }
    }
}
