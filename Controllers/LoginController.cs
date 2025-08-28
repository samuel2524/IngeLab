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
                    string sqlIngenieros = "SELECT * FROM usuarios WHERE correo = @Correo AND contraseña = @Contraseña";

                    using (var cmd = new NpgsqlCommand(sqlIngenieros, conexion))
                    {
                        cmd.Parameters.AddWithValue("Correo", usuario.Correo);
                        cmd.Parameters.AddWithValue("Contraseña", usuario.Contraseña);

                        int countUsuarios = Convert.ToInt32(cmd.ExecuteScalar());

                        if (countUsuarios > 0)
                        {
                            return RedirectToAction("Index", "Usuario");
                        }
                        // else
                        // {
                        //     ViewBag.Error = "Correo o contraseña incorrectos.";
                            
                        // }

                    }

                    string sqlEmpresas = "SELECT * FROM empresas WHERE correo = @Correo AND contraseña = @Contraseña";

                    using (var cmd = new NpgsqlCommand(sqlEmpresas, conexion))
                    {
                        cmd.Parameters.AddWithValue("Correo", usuario.Correo);
                        cmd.Parameters.AddWithValue("Contraseña", usuario.Contraseña);

                        int countEmpresas = Convert.ToInt32(cmd.ExecuteScalar());

                        if (countEmpresas > 0)
                        {
                            return RedirectToAction("Index", "Empresa");
                        }
                        // {
                        //     ViewBag.Error = "Correo o contraseña incorrectos.";
                        // }

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