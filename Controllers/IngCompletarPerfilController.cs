using Microsoft.AspNetCore.Mvc;
using IngeLab.Models;

namespace IngeLab.Controllers
{
    public class IngCompletarPerfilController : Controller
    {
        BD bd = new BD();
        public IActionResult Index()
        {
            return View();
        }

        // Esta es la "acción" que mostrará la vista. 
        [HttpGet] // Especificamos que responde a peticiones GET (cuando escribes la URL en el nav) ES TEMPORAL PARA PRUEB
        public IActionResult Completar()
        {
            // Esta línea busca y devuelve tu archivo .cshtml
            return View("IngCompletarPerfil");
        }

    [HttpPost]
    public IActionResult CompletarPerfil(Ingenieros ingenieros)
        {
            try
            {
                using (var conexion = bd.establecerConexion())
                {
                    string query = @"INSERT INTO datosprofesionales 
                                    (id_usuario, anios_experiencia, nivel_academico, habilidades_tecnicas, especializacion, idiomas,disponibilidad)
                                    VALUES(@Id_Usuario, @Anios_Experiencia, @Nivel_Academico, @Habilidades_Tecnicas, @Especializacion, @Idiomas,@Disponibilidad)";

                    using (var cmd = new Npgsql.NpgsqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("Id_Usuario", ingenieros.Id_Usuario);
                        cmd.Parameters.AddWithValue("disponibilidad", ingenieros.Disponibilidad);
                        cmd.Parameters.AddWithValue("Anios_Experiencia", ingenieros.Anios_Experiencia);
                        cmd.Parameters.AddWithValue("Nivel_Academico", ingenieros.Nivel_Academico);
                        cmd.Parameters.AddWithValue("Habilidades_Tecnicas", ingenieros.Habilidades_Tecnicas);
                        cmd.Parameters.AddWithValue("Especializacion", ingenieros.Especializacion);
                        cmd.Parameters.AddWithValue("Idiomas", ingenieros.Idiomas);
                        cmd.ExecuteNonQuery();
                    }
                }

                ViewBag.Exito = "Perfil completado exitosamente";
                return RedirectToAction("Index", "VistaIngenieros");
            }
            catch (Exception e)
            {
                return Content("Error al completar el perfil: " + e.Message);
            }
        }


        
    }
}
