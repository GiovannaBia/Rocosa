using System.Collections.ObjectModel;

namespace Rocosa_Utilidades
{
    public static class WC
    {
        public const string ImagenRuta = @"\imagenes\producto\"; //Aca guardaremos las imagenes
        public const string SessionCarroCompras = "SessionCarroCompras";
        public const string SessionOrdenId = "SessionOrden";
        
        public const string AdminRole = "Admin";
        public const string ClienteRole = "Cliente";

        public const string CategoriaNombre = "Categoria";
        public const string TipoAplicacionNombre = "TipoAplicacion";

        public const string Exitosa = "Exitosa";
        public const string Error = "Error";

        public const string EstadoPendiente = "Pendiente";
        public const string EstadoAprobado = "Aprobado";
        public const string EstadoEnProceso = "Procesando";
        public const string EstadoEnviado = "Enviado";
        public const string EstadoCancelado = "Cancelado";
        public const string EstadoDevuelto = "Devuelto";

        public static readonly IEnumerable<string> ListaEstados = new ReadOnlyCollection<string>(
            new List<string>
            {
                EstadoPendiente, EstadoAprobado, EstadoEnProceso, EstadoCancelado, EstadoDevuelto, EstadoEnviado
            });
    }
}
