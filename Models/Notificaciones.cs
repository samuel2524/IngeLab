namespace IngeLab.Models.NotificacionesIngeniero
{
    public class Notificacion
    {
        public int IdContacto { get; set; }
        public string Oferta { get; set; }
        public string Empresa { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class NotificacionEmpresa
    {
        public int IdNotificacion { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leido { get; set; }
    }
}