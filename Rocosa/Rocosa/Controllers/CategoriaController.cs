using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocosa_AccesoDatos.Datos;
using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using Rocosa_Modelos;
using Rocosa_Utilidades;

namespace Rocosa.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepositorio _catRepo;

        public CategoriaController(ICategoriaRepositorio catRepo)
        {
            _catRepo = catRepo;
        }
        public IActionResult Index()
        {
            IEnumerable<Categoria> lista = _catRepo.ObtenerTodos();

            return View(lista);
        }

        //Get (llama a la vista "Crear")
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Categoria categoria)
        {
            if(ModelState.IsValid)
            {
                _catRepo.Agregar(categoria);
                _catRepo.Guardar();

                return RedirectToAction(nameof(Index));
            }
            return View(categoria); //Así en lugar de retornar al Index, retorna la vista Crear con las validaciones en rojo
        }

        //Get Editar
        public IActionResult Editar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _catRepo.Obtenter(Id.GetValueOrDefault());
            if (obj == null) {
                return NotFound();
            }
            return View(obj);
        }

        //Editar post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Actualizar(categoria);
                _catRepo.Guardar();

                return RedirectToAction(nameof(Index));
            }
            return View(categoria); //Así en lugar de retornar al Index, retorna la vista Editar con las validaciones en rojo
        }

        //Get Eliminar
        public IActionResult Eliminar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _catRepo.Obtenter(Id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //Eliminar post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Categoria categoria)
        {
            if(categoria == null)
            {
                return NotFound();
            }
           _catRepo.Remover(categoria);
            _catRepo.Guardar();
            return RedirectToAction(nameof(Index));
        }
    }
}
