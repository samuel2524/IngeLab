namespace IngeLab.Models
{
    public class PostViewModel
    {
        // Datos del Post
        public int Id_Post { get; set; }
        public int Id_Usuario { get; set; } // ID del autor
        public string Contenido { get; set; }
        public string Codigo { get; set; }
        public string TextoExplicativo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Tipo_Contenido { get; set; }
        public bool Fijado { get; set; }

        // Datos del Autor del Post
        public string AutorNombre { get; set; }
        public string AutorApellido { get; set; }
        // Podrías añadir la URL de la foto de perfil aquí también
        // public string AutorFotoUrl { get; set; } 

        // ✨ AÑADE ESTA LÍNEA ✨
        public int CantidadRespuestas { get; set; }
    }
}
