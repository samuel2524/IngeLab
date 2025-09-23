namespace IngeLab.Models
{
    public class EmpresaPerfilViewModel
    {
        public Empresas Perfil_Empresa { get; set; }
        public List<string> TecnologiasClave { get; set; }

        public bool IsOwnProfile { get; set; }
    }
}