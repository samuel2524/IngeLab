using Microsoft.AspNetCore.Mvc;
using IngeLab.Models;

namespace IngeLab.Controllers
{
    public class RegistroController : Controller
    {
        BD bd = new BD();
        [HttpGet]
        public IActionResult Index()
        {

            return View();  
        }

    
        [HttpPost]
        public IActionResult Registro(Registro_Ingenieros_empresas modelo)
        {
            
            ModelState.Clear(); // limpiar errores automaticcos antes de la validación personalizada

            try
            {   
           
                if (modelo.TipoUsuario == "ingeniero")
                {
                    return RegistroUsuario(modelo.Ingeniero);
                }
                else if (modelo.TipoUsuario == "empresa")
                {
                    return RegistroEmpresa(modelo.Empresa);
                }


                return null; 
            
            }





            catch (Exception e)
            {

                return Content("Error al registrar el usuario: " + e.Message);
            }


        }




        public IActionResult RegistroUsuario(Ingenieros Ingeniero)
        {
            ModelState.Clear();
            Ingeniero.ControlDeErrores(ModelState);

            if (!ModelState.IsValid)
            {
                // Si hay errores de validación, vuelve a mostrar el formulario con los mensajes de error
                ViewBag.mensaje = "Por favor corrige los errores en el formulario.";
                return View("~/Views/Registro/Index.cshtml");
            }

            try
            {
                using (var conexion = bd.establecerConexion())
                {
                    string query = "INSERT INTO usuarios (nombre, apellidos, tipo_documento, numero_documento, correo, contraseña, fecha_nacimiento, telefono) " +
                    "VALUES (@Nombre, @Apellido, @TipoDocumento, @NumeroDocumento, @Correo, @Contraseña, @FechaNacimiento, @Telefono)";


                    using (var cmd = new Npgsql.NpgsqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("Nombre", Ingeniero.Nombre);
                        cmd.Parameters.AddWithValue("Apellido", Ingeniero.Apellido);
                        cmd.Parameters.AddWithValue("TipoDocumento", Ingeniero.TipoDocumento);
                        cmd.Parameters.AddWithValue("NumeroDocumento", Ingeniero.NumeroDocumento);
                        cmd.Parameters.AddWithValue("Correo", Ingeniero.Correo);
                        cmd.Parameters.AddWithValue("Contraseña", Ingeniero.Contraseña);
                        cmd.Parameters.AddWithValue("FechaNacimiento", Ingeniero.FechaNacimiento);
                        cmd.Parameters.AddWithValue("Telefono", Ingeniero.Telefono);
                        cmd.ExecuteNonQuery();
                    }
                }

                ViewBag.Exito = "Usuario registrado exitosamente";
                return View("~/Views/Registro/Index.cshtml");
                
            }
            catch (System.Exception e)
            {

                return Content("Error al registrar el usuario: " + e.Message);
            }

            
        }



        [HttpPost]

        public IActionResult RegistroEmpresa(Empresas empresas)
        {


            ModelState.Clear(); // limpiar errores automaticcos antes de la validación personalizada

            empresas.ControlDeErrores(ModelState);

            if (!ModelState.IsValid)
            {
                // Si hay errores de validación, vuelve a mostrar el formulario con los mensajes de error
                ViewBag.mensaje = "Por favor corrige los errores en el formulario.";
                return View("~/Views/Registro/Index.cshtml",
               new Registro_Ingenieros_empresas { Empresa = empresas, TipoUsuario = "empresa" });
            }
    
    
            try
            {

                
                using (var conexion = bd.establecerConexion())
                {
                    string query = "INSERT INTO empresas (nombre, nit, correo, contraseña, telefono) " +
                        "VALUES (@Nombre, @NIT, @Correo, @Contraseña, @Telefono)";

                    using (var cmd = new Npgsql.NpgsqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("Nombre", empresas.Nombre);
                        cmd.Parameters.AddWithValue("NIT", empresas.NIT);
                        cmd.Parameters.AddWithValue("Correo", empresas.Correo);
                        cmd.Parameters.AddWithValue("Contraseña", empresas.Contraseña);
                        cmd.Parameters.AddWithValue("Telefono", empresas.Telefono);
                        cmd.ExecuteNonQuery();
                    }
                }

                ViewBag.Exito = "Empresa registrada exitosamente";
                return View("~/Views/Registro/Index.cshtml",
                new Registro_Ingenieros_empresas { Empresa = empresas, TipoUsuario = "empresa" });
            }
            catch (System.Exception e)
            {
                return Content("Error al registrar la empresa: " + e.Message);
            }
        }
      
                
    }
}