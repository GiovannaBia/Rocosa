using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Logging;
using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;

namespace Rocosa.Controllers
{
    public class OrdenController : Controller
    {
        private readonly IOrdenRepositorio _ordenRepo;
        private readonly IOrdenDetalleRepositorio _ordenDetalleRepo;

        public OrdenController(IOrdenRepositorio ordenRepo, IOrdenDetalleRepositorio ordenDetalleRepo)
        {
                _ordenDetalleRepo = ordenDetalleRepo;
                _ordenRepo = ordenRepo; 
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
