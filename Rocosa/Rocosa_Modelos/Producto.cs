
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocosa_Modelos
{
    public class Producto
    {
        public Producto()
        {
            TempMetroCuadrado = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]  
        public string NombreProducto { get; set; }

        [Required(ErrorMessage = "La descripcion corta es requerida")]
        public string DescripcionCorta { get; set; }

        [Required(ErrorMessage = "La descripcion del producto es requerida")]
        public string DescripcionProducto { get; set; }

        [Required(ErrorMessage = "El precio del producto es requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero")]
        public double Precio { get; set; }  

        public string? ImagenUrl { get; set; }

        //Foreign Key
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }   
        public int TipoAplicacionId {  get; set; }
        [ForeignKey("TipoAplicacionId")]
        public virtual TipoAplicacion? TipoAplicacion { get; set; }

        //Propiedad temporal
        [NotMapped]
        [Range(1, 10000)]
        public int TempMetroCuadrado { get; set;  }
    }
}
