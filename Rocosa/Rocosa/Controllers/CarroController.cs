using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocosa.Datos;
using Rocosa.Models;
using Rocosa.Models.ViewModels;
using Rocosa.Utilidades;
using System.Security.Claims;

namespace Rocosa.Controllers
{
    [Authorize]
    public class CarroController : Controller
    {
        private readonly ApplicationDBContext _db;
        [BindProperty]
       public ProductoUsuarioVM productoUsuarioVM { get; set; }
        public CarroController(ApplicationDBContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<CarroCompras> carroComprasList = new List<CarroCompras>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompras>>(WC.SessionCarroCompras);
            }
            List<int> prodEnCarro = carroComprasList.Select(i => i.ProductoId).ToList();
            IEnumerable<Producto> prodList = _db.Producto.Where(p => prodEnCarro.Contains(p.Id));
            return View(prodList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Resumen));
        }

        public IActionResult Resumen()
        {
            //Capturar el usuario conectado
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //Capturar su carrito
            List<CarroCompras> carroComprasList = new List<CarroCompras>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompras>>(WC.SessionCarroCompras);
            }
            List<int> prodEnCarro = carroComprasList.Select(i => i.ProductoId).ToList();
            IEnumerable<Producto> prodList = _db.Producto.Where(p => prodEnCarro.Contains(p.Id));
            //Llenamos el viewmodel
            productoUsuarioVM = new ProductoUsuarioVM()
            {
                UsuarioAplicacion = _db.UsuarioAplicacion.FirstOrDefault(u => u.Id == claim.Value),
                ProductoLista = prodList.ToList()
            };
            return View(productoUsuarioVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Resumen")]
        public IActionResult ResumenPost(ProductoUsuarioVM productoUsuarioVM)
        {
            return RedirectToAction(nameof(Confirmacion));
        }

        public IActionResult Confirmacion()
        {
            HttpContext.Session.Clear(); //Limpio la sesion, el carro 
            return View();  
        }

        public IActionResult Remover (int Id)
        {
            List<CarroCompras> carroComprasList = new List<CarroCompras>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompras>>(WC.SessionCarroCompras);
            }
            carroComprasList.Remove(carroComprasList.FirstOrDefault(p=>p.ProductoId==Id));
            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasList); //Actualizar el carro en la sesion

            return RedirectToAction(nameof(Index));
        }
    }
}
