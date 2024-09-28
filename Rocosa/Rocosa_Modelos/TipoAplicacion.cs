using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace Rocosa_Modelos
{
    public class TipoAplicacion
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio")]
        public string Nombre { get; set; }
    }
}
