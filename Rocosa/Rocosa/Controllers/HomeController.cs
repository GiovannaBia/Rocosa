using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocosa_AccesoDatos.Datos;
using Rocosa_AccesoDatos.Datos.Repositorio;
using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using Rocosa_Modelos;
using Rocosa_Modelos.ViewModels;
using Rocosa_Utilidades;

namespace Rocosa.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductoRepositorio _prodRepo;
        private readonly ICategoriaRepositorio _categoriaRepo;
        public HomeController(IProductoRepositorio prodRepo, ICategoriaRepositorio categoriaRepo)
        {
            _prodRepo = prodRepo;
            _categoriaRepo = categoriaRepo;
        }
        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Productos = _prodRepo.ObtenerTodos(incluirPropiedades:"Categoria,TipoAplicacion"),
                Categorias = _categoriaRepo.ObtenerTodos()
            };

            return View(homeVM);
        }

        public IActionResult Detalle(int Id)
        {
            List<CarroCompras> carroComprasLista = new List<CarroCompras>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasLista = HttpContext.Session.Get<List<CarroCompras>>(WC.SessionCarroCompras);
            }
            DetalleVM detalleVM = new DetalleVM()
            {
                Producto = _prodRepo.ObtenerPrimero(p=> p.Id == Id,incluirPropiedades:"Categoria,TipoAplicacion"),
                ExisteEnCarro = false
            };

            foreach (var item in carroComprasLista)
            {
                if (item.ProductoId == Id)
                {
                    detalleVM.ExisteEnCarro = true;
                }
            }

            return View(detalleVM);  
        }

        [HttpPost, ActionName("Detalle")]
        public IActionResult DetallePost(int Id)
        {
            List<CarroCompras> carroComprasLista = new List<CarroCompras>();
            if(HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras) != null && 
                HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasLista = HttpContext.Session.Get<List<CarroCompras>>(WC.SessionCarroCompras);
            }
            carroComprasLista.Add(new CarroCompras() { ProductoId = Id });
            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasLista);

            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult RemoverDeCarro(int Id)
        {
            List<CarroCompras> carroComprasLista = new List<CarroCompras>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasLista = HttpContext.Session.Get<List<CarroCompras>>(WC.SessionCarroCompras);
            }
            
            var productoARemover = carroComprasLista.SingleOrDefault(x => x.ProductoId  == Id);
            if(productoARemover != null)
            {
                carroComprasLista.Remove(productoARemover);
            }

            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasLista);

            return RedirectToAction(nameof(Index));
        }
    }
}
