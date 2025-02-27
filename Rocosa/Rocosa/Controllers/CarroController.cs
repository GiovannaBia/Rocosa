﻿using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocosa_AccesoDatos.Datos;
using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using Rocosa_Modelos;
using Rocosa_Modelos.ViewModels;
using Rocosa_Utilidades;
using Rocosa_Utilidades.BrainTree;
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
        private readonly IVentaRepositorio _ventaRepo;
        private readonly IVentaDetalleRepositorio _ventaDetalleRepo;
        private readonly IBrainTreeGate _brain;

        [BindProperty]
       public ProductoUsuarioVM productoUsuarioVM { get; set; }
        public CarroController(IProductoRepositorio prodRepo, ICategoriaRepositorio categoriaRepo, IUsuarioAplicacionRepositorio usuarioRepo, IOrdenRepositorio ordenRepo, IOrdenDetalleRepositorio ordenDetalleRepo, IVentaRepositorio ventaRepo, IVentaDetalleRepositorio ventaDetalleRepo, IBrainTreeGate brain )
        {
            _prodRepo = prodRepo;
            _categoriaRepo = categoriaRepo;
            _usuarioRepo = usuarioRepo;
            _ordenRepo = ordenRepo;
            _ordenDetalleRepo = ordenDetalleRepo;
            _ventaRepo = ventaRepo;
            _ventaDetalleRepo = ventaDetalleRepo;
            _brain = brain;
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
            List<Producto> prodListFinal = new List<Producto>();
            foreach (var prod in carroComprasList) 
            {
                Producto prodTemp = prodList.FirstOrDefault(p => p.Id == prod.ProductoId);
                prodTemp.TempMetroCuadrado = prod.MetroCuadrado;
                prodListFinal.Add(prodTemp);
            }
            
            return View(prodListFinal); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Producto> ProdLista)
        {
            List<CarroCompras> carroComprasList = new List<CarroCompras>();

            foreach (Producto prod in ProdLista)
            {
                carroComprasList.Add(new CarroCompras
                {
                    ProductoId = prod.Id,
                    MetroCuadrado = prod.TempMetroCuadrado,
                });
            }
            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasList);
            return RedirectToAction(nameof(Resumen));
        }

        public IActionResult Resumen()
        {
            UsuarioAplicacion usuarioAplicacion;

            if (User.IsInRole(WC.AdminRole))
            {
                if(HttpContext.Session.Get<int>(WC.SessionOrdenId)!= 0)
                {
                    Orden orden = _ordenRepo.ObtenerPrimero(u => u.Id == HttpContext.Session.Get<int>(WC.SessionOrdenId));
                    usuarioAplicacion = new UsuarioAplicacion()
                    {
                        Email = orden.Email,
                        NombreCompleto = orden.NombreCompleto,
                        PhoneNumber = orden.Telefono,
                    };
                }
                else //Si no hay una orden 
                {
                    usuarioAplicacion = new UsuarioAplicacion();
                }
                var gateway = _brain.GetGateway();
                var clientToken = gateway.ClientToken.Generate();
                ViewBag.ClientToken = clientToken;
            }
            else
            {
                //Capturar el usuario conectado
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                usuarioAplicacion = _usuarioRepo.ObtenerPrimero(u=>u.Id == claim.Value);
            }


            
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
                UsuarioAplicacion = usuarioAplicacion,
                // ProductoLista = prodList.ToList()
            };

            foreach (var carro in carroComprasList)
            {
                Producto prodTemp = _prodRepo.ObtenerPrimero(p => p.Id == carro.ProductoId);
                prodTemp.TempMetroCuadrado = carro.MetroCuadrado;
                productoUsuarioVM.ProductoLista.Add(prodTemp);
            }

            return View(productoUsuarioVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Resumen")]
        public IActionResult ResumenPost(IFormCollection collection,ProductoUsuarioVM productoUsuarioVM)
        {
            //Capturar el usuario conectado
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (User.IsInRole(WC.AdminRole))
            {
                //Crear venta
                Venta venta = new Venta()
                {
                    CreadoPorUsusarioId = claim.Value,
                    FinalVentaTotal = productoUsuarioVM.ProductoLista.Sum(x => x.TempMetroCuadrado * x.Precio),
                    Direccion = productoUsuarioVM.UsuarioAplicacion.Direccion,
                    Ciudad = productoUsuarioVM.UsuarioAplicacion.Ciudad,
                    Telefono = productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                    NombreCompleto = productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                    Email = productoUsuarioVM.UsuarioAplicacion.Email,
                    FechaVenta = DateTime.Now,
                    EstadoVenta = WC.EstadoPendiente,
                    TransaccionId = 0.ToString(),
                };

                _ventaRepo.Agregar(venta);
                _ventaRepo.Guardar();

                foreach (var producto in productoUsuarioVM.ProductoLista)
                {
                    VentaDetalle ventaDetalle = new VentaDetalle()
                    {
                        VentaId = venta.Id,
                        ProductoId = producto.Id,
                        MetroCuadrado = producto.TempMetroCuadrado,
                        PrecioPorMetroCuadrado = producto.Precio,
                    };
                    _ventaDetalleRepo.Agregar(ventaDetalle);
                }
                _ventaDetalleRepo.Guardar();

                string nonceFromTheClient = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(venta.FinalVentaTotal),
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId = venta.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };
                var gateway = _brain.GetGateway();
                Result<Transaction> result = gateway.Transaction.Sale(request);

                //Modificar la venta
                if(result.Target.ProcessorResponseText == "Approved")
                {
                    venta.TransaccionId = result.Target.Id;
                    venta.EstadoVenta = WC.EstadoAprobado;
                } 
                else
                {
                    venta.EstadoVenta = WC.EstadoCancelado;
                    venta.TransaccionId = 0.ToString();
                }
                _ventaRepo.Guardar();

                return RedirectToAction(nameof(Confirmacion), new {id = venta.Id});
            } 
            else
            {
                //Enviar orden
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

            }
            return RedirectToAction(nameof(Confirmacion));
        }

        public IActionResult Confirmacion(int id = 0)
        {
            Venta venta = _ventaRepo.ObtenerPrimero(v => v.Id == id);
            HttpContext.Session.Clear(); //Limpio la sesion, el carro 
            return View(venta);  
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarCarro(IEnumerable<Producto> ProdLista)
        {
            List<CarroCompras> carroComprasList = new List<CarroCompras>();

            foreach (Producto prod in ProdLista)
            {
                carroComprasList.Add(new CarroCompras
                {
                    ProductoId = prod.Id,
                    MetroCuadrado = prod.TempMetroCuadrado,
                });
            }
            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Limpiar()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
