using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocosa_AccesoDatos.Datos;
using Rocosa_Modelos;
using Rocosa_Modelos.ViewModels;
using Rocosa_Utilidades;

namespace Rocosa.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDBContext _db;
        public HomeController(ApplicationDBContext db)
        {
                _db = db;
        }
        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Productos = _db.Producto.Include(c=>c.Categoria).Include(t=>t.TipoAplicacion),
                Categorias = _db.Categoria
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
                Producto = _db.Producto.Include(c => c.Categoria).Include(t => t.TipoAplicacion).
                                        Where(p => p.Id == Id).FirstOrDefault(),
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
