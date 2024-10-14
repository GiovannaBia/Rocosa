using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocosa_AccesoDatos.Datos;
using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using Rocosa_Modelos;
using Rocosa_Utilidades;

namespace Rocosa.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class TipoAplicacionController : Controller
    {
        private readonly ITipoAplicacionRepositorio _tipoRepo;

        public TipoAplicacionController(ITipoAplicacionRepositorio tipoRepo)
        {
            _tipoRepo = tipoRepo;
        }
        public IActionResult Index()
        {
            IEnumerable<TipoAplicacion> lista = _tipoRepo.ObtenerTodos();

            return View(lista);
        }

        //Get (llama a la vista "Crear")
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(TipoAplicacion tipoAplicacion)
        {
            if (ModelState.IsValid)
            {
                _tipoRepo.Agregar(tipoAplicacion);
                _tipoRepo.Guardar();
                TempData[WC.Exitosa] = "Tipo de aplicación creado exitosamente";

                return RedirectToAction(nameof(Index));
            }
            TempData[WC.Error] = "Error al crear el tipo de aplicación";
            return View(tipoAplicacion); //Así en lugar de retornar al Index, retorna la vista Crear con las validaciones en rojo
        }

        //Get Editar
        public IActionResult Editar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _tipoRepo.Obtenter(Id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //Editar post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(TipoAplicacion tipoAplicacion)
        {
            if (ModelState.IsValid)
            {
               _tipoRepo.Actualizar(tipoAplicacion);
                _tipoRepo.Guardar();
                TempData[WC.Exitosa] = "Tipo de aplicación editado";

                return RedirectToAction(nameof(Index));
            }
            TempData[WC.Error] = "Error al editar tipo de aplicación";
            return View(tipoAplicacion); //Así en lugar de retornar al Index, retorna la vista Editar con las validaciones en rojo
        }

        //Get Eliminar
        public IActionResult Eliminar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _tipoRepo.Obtenter(Id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //Eliminar post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(TipoAplicacion tipoAplicacion)
        {
            if (tipoAplicacion == null)
            {
                return NotFound();
            }
            _tipoRepo.Remover(tipoAplicacion);
            _tipoRepo.Guardar();
            TempData[WC.Exitosa] = "Tipo de aplicacion eliminado";
            return RedirectToAction(nameof(Index));
        }
    }
}
