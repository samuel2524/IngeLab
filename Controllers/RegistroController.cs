using Microsoft.AspNetCore.Mvc;
using IngeLab.Models;
using Npgsql; // Asegúrate de tener este using

namespace IngeLab.Controllers
{
    public class RegistroController : Controller
    {
        BD bd = new BD();

        [HttpGet]
        public IActionResult Index()
        {
            // CORRECCIÓN 1: Siempre debemos pasar un modelo a la vista para evitar errores.
            return View(new Registro_Ingenieros_empresas());
        }

        [HttpPost]
        public IActionResult Registro(Registro_Ingenieros_empresas modelo)
        {
            ModelState.Clear();

            try
            {
                if (modelo.TipoUsuario == "ingeniero")
                {
                    // Llamamos al método pasándole el modelo completo
                    return RegistroUsuario(modelo);
                }
                else if (modelo.TipoUsuario == "empresa")
                {
                    // Igual aquí
                    return RegistroEmpresa(modelo);
                }

                return View("Index", modelo);
            }
            catch (Exception e)
            {
                return Content("Error al registrar el usuario: " + e.Message);
            }
        }

        // CORRECCIÓN 2: El método ahora recibe el modelo COMPLETO.
        public IActionResult RegistroUsuario(Registro_Ingenieros_empresas modelo)
        {
            // Validamos la parte del ingeniero que está DENTRO del modelo.
            modelo.Ingeniero.ControlDeErrores(ModelState);

            if (!ModelState.IsValid)
            {
                ViewBag.mensaje = "Por favor corrige los errores en el formulario.";
                // CORRECCIÓN 3: Devolvemos el modelo completo para NO perder los datos.
                return View("~/Views/Registro/Index.cshtml", modelo);
            }

            try
            {
                using (var conexion = bd.establecerConexion())
                {
                    string query = "INSERT INTO usuarios (nombre, apellidos, tipo_documento, numero_documento, correo, contraseña, fecha_nacimiento, telefono) " +
                                 "VALUES (@Nombre, @Apellido, @TipoDocumento, @NumeroDocumento, @Correo, @Contraseña, @FechaNacimiento, @Telefono) RETURNING id";

                    using (var cmd = new NpgsqlCommand(query, conexion))
                    {
                        // Usamos modelo.Ingeniero para acceder a los datos
                        cmd.Parameters.AddWithValue("Nombre", modelo.Ingeniero.Nombre);
                        cmd.Parameters.AddWithValue("Apellido", modelo.Ingeniero.Apellido);
                        cmd.Parameters.AddWithValue("TipoDocumento", modelo.Ingeniero.TipoDocumento);
                        cmd.Parameters.AddWithValue("NumeroDocumento", modelo.Ingeniero.NumeroDocumento);
                        cmd.Parameters.AddWithValue("Correo", modelo.Ingeniero.Correo);
                        cmd.Parameters.AddWithValue("Contraseña", modelo.Ingeniero.Contraseña);
                        cmd.Parameters.AddWithValue("FechaNacimiento", modelo.Ingeniero.FechaNacimiento);
                        cmd.Parameters.AddWithValue("Telefono", modelo.Ingeniero.Telefono);

                        var newUserId = cmd.ExecuteScalar();

                        if (newUserId != null)
                        {
                            return RedirectToAction("CompletarPerfil", "PerfilIngeniero", new { id = Convert.ToInt32(newUserId) });
                        }
                        else
                        {
                            ViewBag.mensaje = "Error inesperado al crear el usuario.";
                            // Devolvemos el modelo por si acaso, para mantener los datos.
                            return View("~/Views/Registro/Index.cshtml", modelo);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                return Content("Error al registrar el usuario: " + e.Message);
            }
        }

        [HttpPost]
        // CORRECCIÓN 2 (aplicada también aquí): Recibe el modelo completo.
        public IActionResult RegistroEmpresa(Registro_Ingenieros_empresas modelo)
        {
            // Validamos la parte de la empresa DENTRO del modelo.
            modelo.Empresa.ControlDeErrores(ModelState);

            if (!ModelState.IsValid)
            {
                ViewBag.mensaje = "Por favor corrige los errores en el formulario.";
                // CORRECCIÓN 3: Devolvemos el modelo completo.
                return View("~/Views/Registro/Index.cshtml", modelo);
            }

            try
            {
                using (var conexion = bd.establecerConexion())
                {
                    string query = "INSERT INTO empresas (nombre, nit, correo, contraseña, telefono) " +
                                   "VALUES (@Nombre, @NIT, @Correo, @Contraseña, @Telefono)";

                    using (var cmd = new NpgsqlCommand(query, conexion))
                    {
                        // Usamos modelo.Empresa para acceder a los datos
                        cmd.Parameters.AddWithValue("Nombre", modelo.Empresa.Nombre);
                        cmd.Parameters.AddWithValue("NIT", modelo.Empresa.NIT);
                        cmd.Parameters.AddWithValue("Correo", modelo.Empresa.Correo);
                        cmd.Parameters.AddWithValue("Contraseña", modelo.Empresa.Contraseña);
                        cmd.Parameters.AddWithValue("Telefono", modelo.Empresa.Telefono);
                        cmd.ExecuteNonQuery();
                    }
                }

                ViewBag.Exito = "Empresa registrada exitosamente";
                // Limpiamos el formulario devolviendo un modelo nuevo y vacío.
                return View("~/Views/Registro/Index.cshtml", new Registro_Ingenieros_empresas());
            }
            catch (System.Exception e)
            {
                return Content("Error al registrar la empresa: " + e.Message);
            }
        }
    }
}