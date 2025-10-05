// En Models/IngenieroDashboardViewModel.cs

using System.Collections.Generic;
using IngeLab.Models.NotificacionesIngeniero;

namespace IngeLab.Models
{
    // El ViewModel principal que agrupa toda la data
    public class IngenieroDashboardViewModel
    {
        public Ingenieros PerfilActual { get; set; }
        public List<PostViewModel> PostsPublicados { get; set; }
        public List<string> HabilidadesEnTendencia { get; set; }
        public List<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
        public int NotificacionesNoLeidasCount { get; set; }

    }

    public class EmpresaDashboardViewModel
    {
        public Empresas PerfilActual { get; set; }
        public List<NotificacionEmpresa> Notificaciones { get; set; } = new List<NotificacionEmpresa>();
        public List<Ingenieros> Deseados { get; set; } = new List<Ingenieros>();
        
        public List<Ingenieros> Contactados { get; set; } = new List<Ingenieros>();

        public List<Ingenieros> Resultados { get; set; } = new List<Ingenieros>();
    }



}