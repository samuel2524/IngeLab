using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IngeLab.Models; // Asegúrate de que este using sea correcto
using Npgsql;
using System;

namespace IngeLab.Controllers
{
    public class NotificacionesController : Controller
    {
        private readonly BD _bd = new BD(); // Asumiendo que BD es tu clase de conexión

        [HttpPost]
        [ValidateAntiForgeryToken] // 🛡️ Este es el escudo que valida el token
        public IActionResult MarcarTodasComoLeidas()
        {
            try
            {
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (!idUsuario.HasValue)
                {
                    // Si no hay sesión, es un acceso no autorizado.
                    return Unauthorized(new { success = false, message = "Usuario no autenticado." });
                }

                using (var conexion = _bd.establecerConexion())
                {
                    // La query que actualiza todas las notificaciones pendientes del usuario.
                    var query = @"UPDATE ingenieros_contactados
                                  SET leido = true
                                  WHERE id_usuario = @IdUsuario AND (leido = false OR leido IS NULL)";

                    using (var comando = new NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdUsuario", idUsuario.Value);
                        int filasAfectadas = comando.ExecuteNonQuery();

                        // Log para debugging, por si acaso.
                        Console.WriteLine($"Filas afectadas para el usuario {idUsuario.Value}: {filasAfectadas}");
                    }
                }

                // Si todo sale bien, enviamos una respuesta positiva.
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Si algo se rompe, enviamos un mensaje de error claro.
                Console.WriteLine($"Error en MarcarTodasComoLeidas: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Ocurrió un error en el servidor." });
            }
        }
    }
}