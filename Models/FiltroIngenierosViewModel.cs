namespace IngeLab.Models
{
    public class FiltroIngenieroViewModel
    {
        public string PalabrasClave { get; set; } 
        public string Disponibilidad { get; set; }
        public int? Anios_Experiencia { get; set; }
        public string Nivel_Academico { get; set; }

        public int IdEmpresa { get; set; }

        public List<Ingenieros> Contactados { get; set; } = new List<Ingenieros>();

        public List<Ingenieros> Resultados { get; set; } = new List<Ingenieros>();
    }
}