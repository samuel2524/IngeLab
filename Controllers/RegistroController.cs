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
                    modelo.Ingeniero.ControlDeErrores(ModelState);

                    if (!ModelState.IsValid)
                    {
                        // Si hay errores de validación, vuelve a mostrar el formulario con los mensajes de error
                        ViewBag.mensaje = "Por favor corrige los errores en el formulario.";
                        return View("~/Views/Registro/Index.cshtml",modelo);
                    }
                    using (var conexion = bd.establecerConexion())
                    {
                        string query = "INSERT INTO usuarios (nombre, apellidos, tipo_documento, numero_documento, correo, contraseña, fecha_nacimiento, telefono) " +
                            "VALUES (@Nombre, @Apellido, @TipoDocumento, @NumeroDocumento, @Correo, @Contraseña, @FechaNacimiento, @Telefono)";


                        using (var cmd = new Npgsql.NpgsqlCommand(query, conexion))
                        {
                            cmd.Parameters.AddWithValue("Nombre", modelo.Ingeniero.Nombre);
                            cmd.Parameters.AddWithValue("Apellido", modelo.Ingeniero.Apellido);
                            cmd.Parameters.AddWithValue("TipoDocumento", modelo.Ingeniero.TipoDocumento);
                            cmd.Parameters.AddWithValue("NumeroDocumento", modelo.Ingeniero.NumeroDocumento);
                            cmd.Parameters.AddWithValue("Correo", modelo.Ingeniero.Correo);
                            cmd.Parameters.AddWithValue("Contraseña", modelo.Ingeniero.Contraseña);
                            cmd.Parameters.AddWithValue("FechaNacimiento", modelo.Ingeniero.FechaNacimiento);
                            cmd.Parameters.AddWithValue("Telefono", modelo.Ingeniero.Telefono);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    ViewBag.mensaje = "Usuario registrado exitosamente";
                    return View("~/Views/Registro/Index.cshtml",modelo);
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

                ViewBag.mensaje = "Empresa registrada exitosamente";
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