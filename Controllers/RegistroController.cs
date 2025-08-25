using IngeLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IngeLab.Controllers
{
    public class RegistroController : Controller
    {     


        private readonly BD bd = new BD();  
        // Aquí puedes agregar métodos para manejar el registro de usuarios
        // Por ejemplo, un método para mostrar el formulario de registro
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }
        [HttpPost]

        public IActionResult Registro(Ingenieros ingenieros)
        {

            try
            {
                using (var conexion = bd.establecerConexion())
                {
                    string query = "INSERT INTO usuarios (nombre, apellidos, tipo_documento, numero_documento, correo, contraseña, fecha_nacimiento, telefono) " +
                               "VALUES (@Nombre, @Apellido, @TipoDocumento, @NumeroDocumento, @Correo, @Contraseña, @FechaNacimiento, @Telefono)";
                    

                    using (var cmd = new Npgsql.NpgsqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("Nombre", ingenieros.Nombre);
                        cmd.Parameters.AddWithValue("Apellido", ingenieros.Apellido);
                        cmd.Parameters.AddWithValue("TipoDocumento", ingenieros.TipoDocumento);
                        cmd.Parameters.AddWithValue("NumeroDocumento", ingenieros.NumeroDocumento);
                        cmd.Parameters.AddWithValue("Correo", ingenieros.Correo);
                        cmd.Parameters.AddWithValue("Contraseña", ingenieros.Contraseña);
                        cmd.Parameters.AddWithValue("FechaNacimiento", ingenieros.FechaNacimiento);
                        cmd.Parameters.AddWithValue("Telefono", ingenieros.Telefono);
                        cmd.ExecuteNonQuery();
                    }
                }

                ViewBag.mensaje = "Usuario registrado exitosamente";
                return View("~/Views/Registro/Registro.cshtml");

            }



            catch (Exception e)
            {

                return Content("Error al registrar el usuario: " + e.Message);
            }
                           
        }
    }
}