namespace IngeLab.Models
{
    public class Registro_Ingenieros_empresas
    {
        public Ingenieros Ingeniero { get; set; } = new Ingenieros();
        public Empresas Empresa { get; set; } = new Empresas();

        public string TipoUsuario { get; set; }
    }
}