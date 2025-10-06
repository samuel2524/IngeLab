using Microsoft.AspNetCore.Mvc;

namespace IngeLab.Models
{
    public class HiloDetalleViewModel
    {
        public PostViewModel HiloPrincipal { get; set; }
        public List<PostViewModel> Respuestas { get; set; }

        // ✨ AÑADE ESTAS DOS LÍNEAS ✨
        public bool PuedeResponder { get; set; } // Nos dirá si mostramos los botones de respuesta
        public int PerfilUsuarioId { get; set; } // Guardará el ID del perfil al que debemos volver
    }
}
