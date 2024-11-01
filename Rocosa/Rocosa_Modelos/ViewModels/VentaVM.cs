using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocosa_Modelos.ViewModels
{
   public class VentaVM
    {
        public IEnumerable<Venta> VentaLista { get; set; }

        public IEnumerable<SelectListItem> EstadoLista { get; set; }
        public string Estado {  get; set; } 
    }
}
