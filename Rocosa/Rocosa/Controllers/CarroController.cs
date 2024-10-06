using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocosa_AccesoDatos.Datos;
using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using Rocosa_Modelos;
using Rocosa_Modelos.ViewModels;
using Rocosa_Utilidades;
using System.Security.Claims;

namespace Rocosa.Controllers
{
    [Authorize]
    public class CarroController : Controller
    {
        private readonly IProductoRepositorio _prodRepo;
        private readonly ICategoriaRepositorio _categoriaRepo;
        private readonly IUsuarioAplicacionRepositorio _usuarioRepo;
        private readonly IOrdenRepositorio _ordenRepo;
        private readonly IOrdenDetalleRepositorio _ordenDetalleRepo;
        [BindProperty]
       public ProductoUsuarioVM productoUsuarioVM { get; set; }
        public CarroController(IProductoRepositorio prodRepo, ICategoriaRepositorio categoriaRepo, IUsuarioAplicacionRepositorio usuarioRepo, IOrdenRepositorio ordenRepo, IOrdenDetalleRepositorio ordenDetalleRepo)
        {
            _prodRepo = prodRepo;
            _categoriaRepo = categoriaRepo;
            _usuarioRepo = usuarioRepo;
            _ordenRepo = ordenRepo;
            _ordenDetalleRepo = ordenDetalleRepo;
        }
        public IActionResult Index()
        {
            List<CarroCompras> carroComprasList = new List<CarroCompras>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompras>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompras>>(WC.SessionCarroCompras);
            }
            List<int> prodEnCarro = carroComprasList.Select(i => i.ProductoId).ToList(); //Lista de ids de los productos
            IEnumerable<Producto> prodList = _prodRepo.ObtenerTodos(p => prodEnCarro.Contains(p.Id)); //Busco en db los prductos que contengan esos ids 
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
            IEnumerable<Producto> prodList = _prodRepo.ObtenerTodos(p => prodEnCarro.Contains(p.Id));
            // IEnumerable<Producto> prodList = _db.Producto.Where(p => prodEnCarro.Contains(p.Id));
            //Llenamos el viewmodel
            productoUsuarioVM = new ProductoUsuarioVM()
            {
                UsuarioAplicacion = _usuarioRepo.ObtenerPrimero(u => u.Id == claim.Value),
                //UsuarioAplicacion = _db.UsuarioAplicacion.FirstOrDefault(u => u.Id == claim.Value),
                ProductoLista = prodList.ToList()
            };
            return View(productoUsuarioVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Resumen")]
        public IActionResult ResumenPost(ProductoUsuarioVM productoUsuarioVM)
        {
            //Capturar el usuario conectado
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //Grabar la orden y detalle en la DB
            Orden orden = new Orden()
            {
                UsuarioAplicacionId = claim.Value,
                NombreCompleto = productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                Email = productoUsuarioVM.UsuarioAplicacion.Email,
                Telefono = productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                FechaOrden = DateTime.Now,

            };

            _ordenRepo.Agregar(orden);
            _ordenRepo.Guardar();

            foreach (var prod in productoUsuarioVM.ProductoLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle()
                {
                    OrdenId = orden.Id,
                    ProductoId = prod.Id,
                };
                _ordenDetalleRepo.Agregar(ordenDetalle);
            }

            _ordenDetalleRepo.Guardar();

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
