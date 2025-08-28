
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Npgsql;

namespace IngeLab.Controllers
{
    public class BD : Controller
    {
        NpgsqlConnection conexion = new NpgsqlConnection();
        static string servidor = "localhost";
        static string bd = "mi_basedatos";
        static string usuario = "Ingelab";
        static string contraseña = "Ingelab2025";
        static string puerto = "5432";

        string cadenaConexion = $"Host={servidor};Port={puerto};Database={bd};Username={usuario};Password={contraseña}";

        public NpgsqlConnection establecerConexion()
        {
            conexion.ConnectionString = cadenaConexion;
            conexion.Open();

            return conexion;
        }

        

        



     

    }
    
}