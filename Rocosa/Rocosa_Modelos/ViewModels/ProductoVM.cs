using Microsoft.AspNetCore.Mvc.Rendering;

namespace Rocosa_Modelos.ViewModels
{
    //View model me permite empaquetar los objetos que necesito para la vista: producto, tipoaplicacion y categoria
    // Me ayuda a implementar el tipado fuerte 
    public class ProductoVM
    {
        public Producto Producto { get; set; }
        public IEnumerable<SelectListItem>? CategoriaLista { get; set; }
        public IEnumerable<SelectListItem>? TipoAplicacionLista  { get; set; }   
    }
}
