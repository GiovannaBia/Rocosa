using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Logging;
using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using Rocosa_Modelos;
using Rocosa_Modelos.ViewModels;
using Rocosa_Utilidades;

namespace Rocosa.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class OrdenController : Controller
    {


        private readonly IOrdenRepositorio _ordenRepo;
        private readonly IOrdenDetalleRepositorio _ordenDetalleRepo;
        [BindProperty]
        public OrdenVM OrdenVM { get; set; }

        public OrdenController(IOrdenRepositorio ordenRepo, IOrdenDetalleRepositorio ordenDetalleRepo)
        {
                _ordenDetalleRepo = ordenDetalleRepo;
                _ordenRepo = ordenRepo; 
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detalle(int id)
        {
            OrdenVM = new OrdenVM()
            {
                Orden = _ordenRepo.ObtenerPrimero(o => o.Id == id),
                OrdenDetalle = _ordenDetalleRepo.ObtenerTodos(d=>d.OrdenId == id, incluirPropiedades:"Producto"),
            };

            return View(OrdenVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detalle()
        {
            List<CarroCompras> carroComprasLista = new List<CarroCompras>();
            OrdenVM.OrdenDetalle = _ordenDetalleRepo.ObtenerTodos(d => d.OrdenId == OrdenVM.Orden.Id);

            foreach (var detalle in OrdenVM.OrdenDetalle)
            {
                CarroCompras carroCompras = new CarroCompras()
                {
                    ProductoId = detalle.ProductoId
                };
                carroComprasLista.Add(carroCompras); 
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasLista);
            HttpContext.Session.Set(WC.SessionOrdenId, OrdenVM.Orden.Id);
            TempData[WC.Exitosa] = "Orden generada correctamente";

            return RedirectToAction("Index", "Carro");
        }

        [HttpPost]
        public IActionResult Eliminar()
        {
            Orden orden = _ordenRepo.ObtenerPrimero(o => o.Id == OrdenVM.Orden.Id);
            IEnumerable<OrdenDetalle> ordenDetalles = _ordenDetalleRepo.ObtenerTodos(d => d.OrdenId == OrdenVM.Orden.Id);

            _ordenDetalleRepo.RemoverRango(ordenDetalles);
            _ordenRepo.Remover(orden);
            _ordenRepo.Guardar();
            TempData[WC.Exitosa] = "Orden eliminada";

            return RedirectToAction(nameof(Index));
        }

        #region APIs

        [HttpGet]
        public IActionResult ObtenerListaOrdenes()
        {
            return Json(new { data = _ordenRepo.ObtenerTodos() });
        }

        #endregion

    }  
}
