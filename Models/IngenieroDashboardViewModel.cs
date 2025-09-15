// En Models/IngenieroDashboardViewModel.cs

using System.Collections.Generic;

namespace IngeLab.Models
{
    // Modelo para un Post individual
    public class Post
    {
        public int Id { get; set; }
        public string AutorNombre { get; set; }
        public string AutorEspecialidad { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaCreacion { get; set; }

        public bool IsPinned { get; set; }
    }

    // Modelo para una Notificación de oferta
    public class NotificacionOferta
    {
        public string EmpresaNombre { get; set; }
        public string TituloOferta { get; set; }
        public bool IsLeida { get; set; }
    }

    // El ViewModel principal que agrupa toda la data
    public class IngenieroDashboardViewModel
    {
        public Ingenieros PerfilActual { get; set; }
        public List<Post> PostsFeed { get; set; }
        public List<NotificacionOferta> Notificaciones { get; set; }
        public List<string> HabilidadesEnTendencia { get; set; }
    }
}