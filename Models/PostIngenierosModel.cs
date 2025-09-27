namespace IngeLab.Models
{
    public class Post 
    {
        public int Id_Post { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Tipo_Contenido { get; set; }
        public bool Fijado { get; set; }
        public int Id_Usuario { get; set; }
        public bool IsPinned { get; set; }

        // ✨ AÑADE ESTAS DOS PROPIEDADES NUEVAS ✨
        // Estas son "ayudantes", no se guardan en la DB directamente.
        public string TextoExplicativo { get; set; }
        public string Codigo { get; set; }
    }
}