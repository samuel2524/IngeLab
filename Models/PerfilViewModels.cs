// En Models/PerfilViewModels.cs

using System.Collections.Generic;

namespace IngeLab.Models
{


    public class IngenieroPerfilViewModel
    {
        public Ingenieros Perfil_Ingeniero { get; set; }
        public List<string> Lista_Habilidades { get; set; }
        
        public List<string> Idiomas { get; set; }
        public List<Post> Posts { get; set; }
        public bool IsOwnProfile { get; set; }
    }
   
}