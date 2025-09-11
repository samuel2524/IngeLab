using Microsoft.AspNetCore.Mvc;
using IngeLab.Models;

namespace IngeLab.Controllers
{
    public class EmpCompletarPerfilController : Controller
    {

        BD bd = new BD();
        public IActionResult Index()
        {
            return View();
        }

        // Esta es la "acción" que mostrará la vista. 
        [HttpGet] // Especificamos que responde a peticiones GET (cuando escribes la URL en el nav) ES TEMPORAL PARA PRUEBAS
        public IActionResult Completar()
        {
            // Esta línea busca y devuelve tu archivo .cshtml
            return View("EmpCompletarPerfil");
        }
        public IActionResult CompletarPerfil(Empresas empresa)
        {
            try
            {
                using (var conexion = bd.establecerConexion())
                {
                    string query = @"INSERT INTO datosempresascompletar
                                    (id_empresa, ubicacion, sector, tamano, modalidad,sitio_web,descripcion_empresa,tecnologia_clave)
                                    VALUES(@Id_empresa, @Ubicacion, @Sector, @Tamano ,@Modalidad,@Sitio_Web,@Descripcion_Empresa,@Tecnologias_Clave)";

                    using (var cmd = new Npgsql.NpgsqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("Id_empresa", empresa.Id_empresa);
                        cmd.Parameters.AddWithValue("Ubicacion", empresa.Ubicacion);
                        cmd.Parameters.AddWithValue("Sector", empresa.Sector);
                        cmd.Parameters.AddWithValue("Tamano", empresa.Tamano);
                        cmd.Parameters.AddWithValue("Modalidad", empresa.Modalidad);
                        cmd.Parameters.AddWithValue("Sitio_Web", empresa.Sitio_Web);
                        cmd.Parameters.AddWithValue("Descripcion_Empresa", empresa.Descripcion_Empresa);
                        cmd.Parameters.AddWithValue("Tecnologias_Clave", empresa.Tecnologias_Clave);
                        cmd.ExecuteNonQuery();
                    }
                }

                ViewBag.Exito = "Perfil completado exitosamente";
                return RedirectToAction("Index", "VistaEmpresa");
            }
            catch (Exception e)
            {
                return Content("Error al completar el perfil: " + e.Message);
            }
        }
    }




    
}
