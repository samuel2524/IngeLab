// En Models/PerfilViewModels.cs

using System.Collections.Generic;

namespace IngeLab.Models
{
    // --- Modelos para el Perfil del Ingeniero ---
    public class Experiencia
    {
        public string Cargo { get; set; }
        public string Empresa { get; set; }
        public string Periodo { get; set; }
        public string Descripcion { get; set; }
    }

    public class Educacion
    {
        public string Institucion { get; set; }
        public string Titulo { get; set; }
        public string Periodo { get; set; }
    }

    public class IngenieroPerfilViewModel
    {
        public Ingenieros Perfil { get; set; }
        public bool IsOwnProfile { get; set; } // Clave para mostrar opciones de edición
        public List<Post> Posts { get; set; }
        public List<Experiencia> Experiencias { get; set; }
        public List<Educacion> Educaciones { get; set; }
        public List<string> Habilidades { get; set; }
    }

    // --- Modelos para el Perfil de la Empresa ---
    public class OfertaLaboral
    {
        public string Titulo { get; set; }
        public string Modalidad { get; set; }
        public string Ubicacion { get; set; }
    }

    public class EmpresaPerfilViewModel
    {
        public Empresas Perfil { get; set; }
        public bool IsOwnProfile { get; set; }
        public List<Post> Posts { get; set; }
        public List<OfertaLaboral> OfertasAbiertas { get; set; }
        public List<string> TechStack { get; set; }
    }
}