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

        // --- ✨ AÑADE ESTAS TRES PROPIEDADES NUEVAS ---

        // Guardará el ID del post padre. Es un 'int?' (nullable) porque los posts originales no tienen padre.
        public int? IdPostPadre { get; set; }

        // Guardará un fragmento del contenido del post padre para dar contexto.
        public string ContenidoPadre { get; set; }

        // Guardará el nombre del autor del post padre.
        public string AutorPadre { get; set; }
    }
}