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
    }
}