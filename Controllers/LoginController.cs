using IngeLab.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;


namespace IngeLab.Controllers
{
    public class LoginController : Controller
    {

        BD bd = new BD();
        // CAMBIO 1: Renombrar Login a Index
        [HttpGet]
        public IActionResult Index() // <--- ANTES SE LLAMABA Login
        {
            return View();
        }

        [HttpPost]
        public IActionResult ValidarUsuario(Usuarios usuario)
        {
            try
            {
                

                using (var conexion = bd.establecerConexion())
                {
                    string sqlIngenieros = "SELECT id_usuario, contraseña FROM usuarios WHERE correo = @Correo";

                    using (var cmd = new NpgsqlCommand(sqlIngenieros, conexion))
                    {
                        cmd.Parameters.AddWithValue("Correo", usuario.Correo);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int idUsuario = reader.GetInt32(0);
                                 string hashGuardado = reader.GetString(1);

                                // 2️⃣ Comparar contraseñas usando BCrypt
                                if (BCrypt.Net.BCrypt.Verify(usuario.Contraseña, hashGuardado))
                                {
                                    HttpContext.Session.SetInt32("UsuarioId", idUsuario);
                                    return RedirectToAction("Index", "VistaIngenieros");
                                }
                            }
                        }

                        // var result = cmd.ExecuteScalar();

                        // if (result != null) // 🔹 Existe el usuario
                        // {
                        //     int idUsuario = Convert.ToInt32(result);

                        //     // ✅ Guardamos el id en la sesión
                        //     HttpContext.Session.SetInt32("UsuarioId", idUsuario);
                        //     Console.WriteLine($"IdUsuario en sesión -> {idUsuario}");

                        //     // Redirigimos al perfil del ingeniero
                        //     return RedirectToAction("Index", "VistaIngenieros");
                        // }
                        // else
                        // {
                        //     ViewBag.Error = "Correo o contraseña incorrectos.";

                        // }
                    }

                    string sqlEmpresas = sqlEmpresas = "SELECT id_empresa, contraseña FROM empresas WHERE correo = @Correo";

                    using (var cmd = new NpgsqlCommand(sqlEmpresas, conexion))
                    {
                        cmd.Parameters.AddWithValue("Correo", usuario.Correo);
                        

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int idEmpresa = reader.GetInt32(0);
                                string hashGuardado = reader.GetString(1);

                                if (BCrypt.Net.BCrypt.Verify(usuario.Contraseña, hashGuardado))
                                {
                                    HttpContext.Session.SetInt32("EmpresaId", idEmpresa);
                                    return RedirectToAction("Index", "VistaEmpresa");
                                }
                            }
                        }

                        // if (result != null) // 🔹 Existe la empresa
                        // {
                        //     int idEmpresa = Convert.ToInt32(result);

                        //     // ✅ Guardamos el id en la sesión
                        //     HttpContext.Session.SetInt32("EmpresaId", idEmpresa);

                        //     // Redirigimos al perfil de la empresa
                        //     return RedirectToAction("Index", "VistaEmpresa");
                        // }
                        // // {
                        // //     ViewBag.Error = "Correo o contraseña incorrectos.";
                        // // }

                    }

                    ViewBag.Error = "Correo o contraseña incorrectos.";
                    return View("~/Views/Login/Index.cshtml");
                }
           }
            catch (System.Exception e)
            {
                return Content("Error al validar el usuario" + e.Message);
            }       
        }      
    }
}