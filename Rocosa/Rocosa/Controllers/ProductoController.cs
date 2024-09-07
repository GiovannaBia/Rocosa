using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using Rocosa.Datos;
using Rocosa.Models;
using Rocosa.Models.ViewModels;

namespace Rocosa.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductoController(ApplicationDBContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;   
        }
        public IActionResult Index()
        {
            IEnumerable<Producto> lista = _db.Producto.Include(c => c.Categoria)
                                                       .Include(t => t.TipoAplicacion);
            return View(lista);
        }
         
        //Get Upsert
        public IActionResult Upsert(int? Id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _db.Categoria.Select(c => new SelectListItem
                {
                    Text = c.NombreCategoria,
                    Value = c.Id.ToString()
                }),
                TipoAplicacionLista = _db.TipoAplicacion.Select(t => new SelectListItem
                {
                    Text = t.Nombre,
                    Value = t.Id.ToString()
                })              
            };
            if(Id== null) //Creamos producto
            {
                return View(productoVM);  //Enviamos el producto vacío, para llenarlo 
            }
            else
            {
                productoVM.Producto = _db.Producto.Find(Id);
                
                if (productoVM.Producto == null)
                    return NotFound();
                
                return View(productoVM); //Enviamos el producto con sus datos
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert (ProductoVM productoVM)
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
                    _db.Producto.Add(productoVM.Producto);
                }
                else
                {
                    //Actualizar producto
                    var objProducto = _db.Producto.AsNoTracking().FirstOrDefault(p => p.Id == productoVM.Producto.Id);

                    if(files.Count > 0) //Si está intentando cargr una nueva img
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
                    _db.Producto.Update(productoVM.Producto);
                }

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else //Si el modelo no es valido
            {
                //se cargan las listas nuevamente
                productoVM.CategoriaLista = _db.Categoria.Select(c => new SelectListItem
                {
                    Text = c.NombreCategoria,
                    Value = c.Id.ToString()
                });
                productoVM.TipoAplicacionLista = _db.TipoAplicacion.Select(t => new SelectListItem
                {
                    Text = t.Nombre,
                    Value = t.Id.ToString()
                });
                return View(productoVM);
            }
  
        }
    }
}
