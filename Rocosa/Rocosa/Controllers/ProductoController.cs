using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using Rocosa_AccesoDatos.Datos;
using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using Rocosa_Modelos;
using Rocosa_Modelos.ViewModels;
using Rocosa_Utilidades;

namespace Rocosa.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductoController : Controller
    {
        private readonly IProductoRepositorio _prodRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductoController(IProductoRepositorio prodRepo, IWebHostEnvironment webHostEnvironment)
        {
            _prodRepo = prodRepo;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            // IEnumerable<Producto> lista = _db.Producto.Include(c => c.Categoria)
            //                                          .Include(t => t.TipoAplicacion);
            IEnumerable<Producto> lista = _prodRepo.ObtenerTodos(incluirPropiedades: "Categoria,TipoAplicacion");
            return View(lista);
        }

        //Get Upsert
        public IActionResult Upsert(int? Id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _prodRepo.ObtenerTodosDropDownList(WC.CategoriaNombre),
                TipoAplicacionLista = _prodRepo.ObtenerTodosDropDownList(WC.TipoAplicacionNombre),
            };
            if (Id == null) //Creamos producto
            {
                return View(productoVM);  //Enviamos el producto vacío, para llenarlo 
            }
            else
            {
                productoVM.Producto = _prodRepo.Obtenter(Id.GetValueOrDefault());

                if (productoVM.Producto == null)
                    return NotFound();

                return View(productoVM); //Enviamos el producto con sus datos
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files; //Obtiene los archivos subidos al formulario
                string webRootPath = _webHostEnvironment.WebRootPath; //Obtiene la ruta fisica de la carpeta wwwroot
                if (productoVM.Producto.Id == 0)
                {
                    //Crear producto
                    string upload = webRootPath + WC.ImagenRuta; //ruta donde guardaremos la img
                    string fileName = Guid.NewGuid().ToString(); //Para que le asigne un id a la img que se guardara
                    string extension = Path.GetExtension(files[0].FileName);
                    //FileStream crea el archivo subido, en la ruta especificada
                    using (var FileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(FileStream);
                    }

                    productoVM.Producto.ImagenUrl = fileName + extension; //en la db solo guardo el nombre de la img
                    _prodRepo.Agregar(productoVM.Producto);
                    TempData[WC.Exitosa] = "Creado";
                }
                else
                {
                    //Actualizar producto
                    var objProducto = _prodRepo.ObtenerPrimero(p => p.Id == productoVM.Producto.Id, isTracking:false);

                    if (files.Count > 0) //Si está intentando cargr una nueva img
                    {
                        string upload = webRootPath + WC.ImagenRuta; //ruta donde guardaremos la img
                        string fileName = Guid.NewGuid().ToString(); //Para que le asigne un id a la img que se guardara
                        string extension = Path.GetExtension(files[0].FileName);
                        //Borro la img anterior
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        //FileStream crea el archivo subido, en la ruta especificada
                        using (var FileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(FileStream);
                        }
                        productoVM.Producto.ImagenUrl = fileName + extension;
                    } //si no se carga una nueva img
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }
                    _prodRepo.Actualizar(productoVM.Producto);
                    TempData[WC.Exitosa] = "Actualizado";
                }

                _prodRepo.Guardar();
                if (TempData[WC.Exitosa] == "Creado")
                {
                    TempData[WC.Exitosa] = "Producto agregado exitosamente";
                }
                else if ((TempData[WC.Exitosa] == "Actualizado"))
                {
                    TempData[WC.Exitosa] = "Producto actualizado exitosamente";
                }
                return RedirectToAction(nameof(Index));
            }
            else //Si el modelo no es valido
            {
                //se cargan las listas nuevamente
                productoVM.CategoriaLista = _prodRepo.ObtenerTodosDropDownList(WC.CategoriaNombre);
                productoVM.TipoAplicacionLista = _prodRepo.ObtenerTodosDropDownList(WC.TipoAplicacionNombre);
                return View(productoVM);
                TempData[WC.Error] = "Error en la carga de producto";
            }

        }

        //Get
        public IActionResult Eliminar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            Producto producto = _prodRepo.ObtenerPrimero(p => p.Id == Id, incluirPropiedades:"Categoria,TipoAplicacion");
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar (Producto producto) 
        { 
            if (producto == null)
            {
                return NotFound();
            }

            //Eliminar primero su imagen del directorio
            string upload = _webHostEnvironment.WebRootPath + WC.ImagenRuta;
            var anteriorFile = Path.Combine(upload, producto.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);    
            }
            //Ahora si, eliminamos producto
            _prodRepo.Remover(producto);
            _prodRepo.Guardar();
            TempData[WC.Exitosa] = "Producto eliminado";
            return RedirectToAction(nameof(Index));
        }

    }
}
