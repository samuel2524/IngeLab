// En Controllers/VistaIngenierosController.cs

using Microsoft.AspNetCore.Mvc;
using IngeLab.Models; // Asegúrate de tener los usings
using System.Collections.Generic;
using System;
using Npgsql;
using IngeLab.Models.NotificacionesIngeniero;
using Newtonsoft.Json;

namespace IngeLab.Controllers
{
    public class VistaIngenierosController : Controller
    {
        BD bd = new BD();

        //public IActionResult Index()
        //{
        //    var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
        //    if (!idUsuario.HasValue)
        //    {
        //        return RedirectToAction("Login", "Cuenta");
        //    }

        //    var perfil = ObtenerPerfilPorId(idUsuario.Value);
        //    var notificaciones = ObtenerNotificaciones(idUsuario.Value);

        //    var viewModel = new IngenieroDashboardViewModel
        //    {
        //        PerfilActual = perfil,
        //        // ✨ AHORA SE LLAMA AL NUEVO MÉTODO PARA EL FEED GLOBAL ✨
        //        PostsPublicados = ObtenerFeedGlobal(),
        //        Notificaciones = notificaciones,
        //        HabilidadesEnTendencia = new List<string> { "IA Generativa", "Rust", "Clean Architecture", "DevSecOps", "Blazor" }
        //    };

        //    return View(viewModel);
        //}

        public IActionResult Index()
        {
            var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            if (!idUsuario.HasValue)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var perfil = ObtenerPerfilPorId(idUsuario.Value);
            // ✨ Llamamos al método que ahora trae TODAS las notis pendientes ✨
            var notificacionesPendientes = ObtenerNotificacionesPendientes(idUsuario.Value);

            var viewModel = new IngenieroDashboardViewModel
            {
                PerfilActual = perfil,
                PostsPublicados = ObtenerFeedGlobal(),
                // La lista completa para el dropdown
                Notificaciones = notificacionesPendientes,
                // El conteo de solo las no leídas para la burbuja
                NotificacionesNoLeidasCount = notificacionesPendientes.Count(n => !n.IsLeida),
                HabilidadesEnTendencia = new List<string> { "IA Generativa", "Rust", "Clean Architecture" }
            };

            return View(viewModel);
        }

        // ⛔️ REEMPLAZA tu antiguo `ObtenerNotificaciones` por este ⛔️
        private List<Notificacion> ObtenerNotificacionesPendientes(int idUsuario)
        {
            var notificaciones = new List<Notificacion>();
            using (var conexion = bd.establecerConexion())
            {
                // La query ahora trae todas las que no han sido aceptadas o rechazadas,
                // sin importar si fueron leídas o no.
                string query = @"
                SELECT ic.id_contacto, ic.oferta, ic.fecha_contacto, e.nombre, ic.leido
                FROM ingenieros_contactados ic
                INNER JOIN empresas e ON ic.id_empresa = e.id_empresa
                WHERE ic.id_usuario = @Id
                AND (ic.estado IS NULL OR ic.estado NOT IN ('aceptada', 'rechazada'))
                ORDER BY ic.fecha_contacto DESC";

                using (var cmd = new NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("Id", idUsuario);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notificaciones.Add(new Notificacion
                            {
                                IdContacto = Convert.ToInt32(reader["id_contacto"]),
                                Oferta = reader["oferta"].ToString(),
                                Empresa = reader["nombre"].ToString(),
                                Fecha = Convert.ToDateTime(reader["fecha_contacto"]),
                                // Mapeamos el estado 'leido' de la base de datos
                                IsLeida = reader["leido"] != DBNull.Value && Convert.ToBoolean(reader["leido"])
                            });
                        }
                    }
                }
            }
            return notificaciones;
        }

        // ✨ ESTA ES LA NUEVA FUNCIÓN QUE TRAE TODOS LOS POSTS DE TODOS LOS INGENIEROS ✨
        // En tu archivo Controllers/VistaIngenierosController.cs

        private List<PostViewModel> ObtenerFeedGlobal()
        {
            var feed = new List<PostViewModel>();
            using (var conexion = bd.establecerConexion())
            {
                // ✨ CAMBIO 1: La query ahora es más poderosa.
                // 1. Añadimos un subquery para CONTAR las respuestas de cada post.
                // 2. Filtramos con WHERE para traer solo los posts PADRE (los que inician hilos).
                var query = @"
            SELECT p.id_post, p.id_usuario, p.contenido, p.fecha_public, 
                   p.tipo_contenido, p.fijado, u.nombre, u.apellidos,
                   (SELECT COUNT(*) FROM postingeniero AS r WHERE r.id_post_padre = p.id_post) AS cantidad_respuestas
            FROM postingeniero p
            INNER JOIN usuarios u ON p.id_usuario = u.id_usuario
            WHERE p.id_post_padre IS NULL  -- ¡Solo traemos los posts que inician un hilo!
            ORDER BY 
                 CASE WHEN p.id_usuario = @IdUsuario THEN 0 ELSE 1 END,
                 p.fijado DESC, 
                 p.fecha_public DESC
            LIMIT 50;";

                using (var comando = new NpgsqlCommand(query, conexion))
                {
                    var idUsuarioActual = HttpContext.Session.GetInt32("UsuarioId");
                    comando.Parameters.AddWithValue("@IdUsuario", idUsuarioActual ?? -1);
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new PostViewModel
                            {
                                Id_Post = reader.GetInt32(reader.GetOrdinal("id_post")),
                                Id_Usuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                Contenido = reader.GetString(reader.GetOrdinal("contenido")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_public")),
                                Tipo_Contenido = reader.GetString(reader.GetOrdinal("tipo_contenido")),
                                Fijado = reader.GetBoolean(reader.GetOrdinal("fijado")),
                                AutorNombre = reader.GetString(reader.GetOrdinal("nombre")),
                                AutorApellido = reader.GetString(reader.GetOrdinal("apellidos")),

                                // ✨ CAMBIO 2: Mapeamos el nuevo campo del conteo de respuestas.
                                CantidadRespuestas = Convert.ToInt32(reader["cantidad_respuestas"])
                            };

                            // ... (el resto de tu lógica para desempaquetar el JSON se mantiene igual) ...
                            if (post.Tipo_Contenido != "texto" && !string.IsNullOrEmpty(post.Contenido))
                            {
                                try
                                {
                                    dynamic data = JsonConvert.DeserializeObject(post.Contenido);
                                    post.TextoExplicativo = data.texto;
                                    post.Codigo = data.codigo;
                                }
                                catch { /* fallback */ }
                            }

                            feed.Add(post);
                        }
                    }
                }
            }
            return feed;
        }

        public IActionResult Post(Ingenieros ingenieros)
        {
            try
            {
                using (var conexion = bd.establecerConexion())
                {
                    var query = "INSERT INTO postingeniero (id_usuario, contenido, fecha_public, tipo_contenido) VALUES (@Id_Usuario, @Contenido, @FechaCreacion, @Tipo_Contenido)";
                    using (var comando = new Npgsql.NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@Id_Usuario", ingenieros.Id_Usuario);
                        comando.Parameters.AddWithValue("@Contenido", ingenieros.Contenido);
                        comando.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                        comando.Parameters.AddWithValue("@Tipo_Contenido", "texto");
                        int filasAfectadas = comando.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            HttpContext.Session.SetInt32("UsuarioId", ingenieros.Id_Usuario);
                        }
                    }
                }
                return RedirectToAction("Index", "VistaIngenieros");
            }
            catch (System.Exception e)
            {
                return Content("Error al crear el post" + e.Message);
            }
        }

        [HttpPost]
        public IActionResult PublicarCodigo(string TextoExplicativo, string Codigo, string Tipo_Contenido)
        {
            try
            {
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (!idUsuario.HasValue)
                {
                    return Unauthorized();
                }

                var contenidoJson = JsonConvert.SerializeObject(new
                {
                    texto = TextoExplicativo,
                    codigo = Codigo
                });

                using (var conexion = bd.establecerConexion())
                {
                    var query = @"INSERT INTO postingeniero 
                                (id_usuario, contenido, fecha_public, tipo_contenido, fijado) 
                            VALUES 
                                (@Id_Usuario, @Contenido, @FechaCreacion, @Tipo_Contenido, false)";
                    using (var comando = new Npgsql.NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@Id_Usuario", idUsuario.Value);
                        comando.Parameters.AddWithValue("@Contenido", contenidoJson);
                        comando.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                        comando.Parameters.AddWithValue("@Tipo_Contenido", Tipo_Contenido);
                        comando.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Index", "VistaIngenieros");
            }
            catch (System.Exception e)
            {
                return Content("Error al publicar el fragmento de código: " + e.Message);
            }
        }


        // En tu archivo Controllers/VistaIngenierosController.cs, añádelo después de PublicarCodigo

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PublicarRespuesta(string Contenido, string Tipo_Contenido, string Codigo, string TextoExplicativo, int IdPostPadre)
        {
            var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            if (!idUsuario.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                string contenidoFinal = Contenido;

                // Si es un post con código, empaquetamos el JSON
                if (Tipo_Contenido != "texto")
                {
                    contenidoFinal = JsonConvert.SerializeObject(new { texto = TextoExplicativo, codigo = Codigo });
                }

                using (var conexion = bd.establecerConexion())
                {
                    var query = @"INSERT INTO postingeniero 
                          (id_usuario, contenido, fecha_public, tipo_contenido, id_post_padre)
                          VALUES (@IdUsuario, @Contenido, NOW(), @TipoContenido, @IdPostPadre)";

                    using (var comando = new NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdUsuario", idUsuario.Value);
                        comando.Parameters.AddWithValue("@Contenido", contenidoFinal);
                        comando.Parameters.AddWithValue("@TipoContenido", Tipo_Contenido);
                        comando.Parameters.AddWithValue("@IdPostPadre", IdPostPadre);
                        comando.ExecuteNonQuery();
                    }
                }

                // 🔥 Detecta si vienes desde la vista del hilo y mantente ahí
                var referer = Request.Headers["Referer"].ToString();
                if (referer.Contains("/Hilo/Detalle"))
                {
                    return Redirect(referer);
                }

                // Si no, vuelve al feed
                return RedirectToAction("Index", "VistaIngenieros");
            }
            catch (Exception e)
            {
                return Content("Error al publicar la respuesta: " + e.Message);
            }
        }



        public IActionResult EliminarPost(int idPost)
        {
            try
            {
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (!idUsuario.HasValue)
                {
                    return RedirectToAction("Login", "Cuenta");
                }
                using (var conexion = bd.establecerConexion())
                {
                    var query = "DELETE FROM postingeniero WHERE id_post = @IdPost AND id_usuario = @IdUsuario";
                    using (var comando = new NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdPost", idPost);
                        comando.Parameters.AddWithValue("@IdUsuario", idUsuario.Value);
                        int filasAfectadas = comando.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            HttpContext.Session.SetInt32("UsuarioId", idUsuario.Value);
                        }
                    }
                }
                return RedirectToAction("Index", "VistaIngenieros");
            }
            catch (Exception e)
            {
                return Content("Error al eliminar el post: " + e.Message);
            }
        }

        [HttpGet]
        public IActionResult EditarPost(int idPost)
        {
            try
            {
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (!idUsuario.HasValue)
                {
                    return RedirectToAction("Login", "Cuenta");
                }
                using (var conexion = bd.establecerConexion())
                {
                    var query = "SELECT id_post, contenido FROM postingeniero WHERE id_post = @IdPost AND id_usuario = @IdUsuario";
                    using (var comando = new NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdPost", idPost);
                        comando.Parameters.AddWithValue("@IdUsuario", idUsuario.Value);
                        using (var reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var post = new Post
                                {
                                    Id_Post = reader.GetInt32(0),
                                    Contenido = reader.GetString(1)
                                };
                                return View(post);
                            }
                        }
                    }
                }
                return RedirectToAction("Index", "VistaIngenieros");
            }
            catch (Exception e)
            {
                return Content("Error al cargar el post: " + e.Message);
            }
        }

        [HttpPost]
        public IActionResult EditarPost(int idPost, string contenido)
        {
            try
            {
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (!idUsuario.HasValue)
                {
                    return RedirectToAction("Login", "Cuenta");
                }
                using (var conexion = bd.establecerConexion())
                {
                    var query = "UPDATE postingeniero SET contenido = @Contenido WHERE id_post = @IdPost AND id_usuario = @IdUsuario";
                    using (var comando = new NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@Contenido", contenido);
                        comando.Parameters.AddWithValue("@IdPost", idPost);
                        comando.Parameters.AddWithValue("@IdUsuario", idUsuario.Value);
                        comando.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Index", "VistaIngenieros");
            }
            catch (Exception e)
            {
                return Content("Error al editar el post: " + e.Message);
            }
        }

        [HttpPost]
        public IActionResult FijarPost(int idPost, bool fijar)
        {
            try
            {
                using (var conexion = bd.establecerConexion())
                {
                    var query = "UPDATE postingeniero SET fijado = @Fijado WHERE id_post = @IdPost";
                    using (var comando = new NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@Fijado", fijar);
                        comando.Parameters.AddWithValue("@IdPost", idPost);
                        comando.ExecuteNonQuery();
                    }
                }
                // ✨ AQUÍ ESTÁ LA MAGIA: LA REDIRECCIÓN INTELIGENTE ✨
                // Obtenemos la URL de la página desde la que se hizo la petición.
                string referer = Request.Headers["Referer"].ToString();

                // Si la URL existe (que casi siempre lo hará), redirigimos a esa misma página.
                if (!string.IsNullOrEmpty(referer))
                {
                    return Redirect(referer); // Esto te devolverá a /IngenieroPerfil
                }

                // Si por alguna extraña razón no encuentra la URL anterior, te manda al feed como plan B.
                return RedirectToAction("Index", "VistaIngenieros");
            }
            catch (Exception e)
            {
                return Content("Error al fijar el post: " + e.Message);
            }
        }

        public Ingenieros ObtenerPerfilPorId(int id)
        {
            using (var conexion = bd.establecerConexion())
            {
                var query = "SELECT id_usuario, nombre, apellidos FROM usuarios WHERE id_usuario = @Id_Usuario";
                using (var comando = new NpgsqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Id_Usuario", id);
                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Ingenieros
                            {
                                Id_Usuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        //private List<Notificacion> ObtenerNotificaciones(int idUsuario)
        //{
        //    var notificaciones = new List<Notificacion>();
        //    using (var conexion = bd.establecerConexion())
        //    {
        //        string query = @"
        //            SELECT ic.id_contacto, ic.oferta, ic.fecha_contacto, e.nombre
        //            FROM ingenieros_contactados ic
        //            INNER JOIN empresas e ON ic.id_empresa = e.id_empresa
        //            WHERE ic.id_usuario = @Id 
        //            AND (ic.leido = false OR ic.leido IS NULL) 
        //            ORDER BY ic.fecha_contacto DESC";
        //        using (var cmd = new NpgsqlCommand(query, conexion))
        //        {
        //            cmd.Parameters.AddWithValue("Id", idUsuario);
        //            using (var reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    notificaciones.Add(new Notificacion
        //                    {
        //                        IdContacto = Convert.ToInt32(reader["id_contacto"]),
        //                        Oferta = reader["oferta"].ToString(),
        //                        Empresa = reader["nombre"].ToString(),
        //                        Fecha = Convert.ToDateTime(reader["fecha_contacto"])
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    return notificaciones;
        //}

        [HttpPost]
        public IActionResult AceptarOferta(int idNotificacion)
        {
            try
            {
                int idContacto = idNotificacion;
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (!idUsuario.HasValue)
                {
                    return RedirectToAction("Login", "Cuenta");
                }
                int? idEmpresa = null;
                using (var conexion = bd.establecerConexion())
                {
                    string query = @"UPDATE ingenieros_contactados 
                                     SET estado = 'aceptada', leido = true
                                     WHERE id_contacto = @IdContacto 
                                     AND id_usuario = @IdUsuario
                                     RETURNING id_empresa";
                    using (var comando = new NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("IdContacto", idContacto);
                        comando.Parameters.AddWithValue("IdUsuario", idUsuario.Value);
                        var result = comando.ExecuteScalar();
                        if (result != null)
                        {
                            idEmpresa = Convert.ToInt32(result);
                        }
                    }
                }
                if (idEmpresa.HasValue)
                {
                    using (var conexion = bd.establecerConexion())
                    {
                        string mensaje = "El ingeniero aceptó tu oferta";
                        string insertQuery = @"
                            INSERT INTO notificaciones_empresa (id_empresa, id_usuario, mensaje)
                            VALUES (@IdEmpresa, @IdUsuario, @Mensaje)";
                        using (var cmd = new NpgsqlCommand(insertQuery, conexion))
                        {
                            cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa.Value);
                            cmd.Parameters.AddWithValue("IdUsuario", idUsuario.Value);
                            cmd.Parameters.AddWithValue("Mensaje", mensaje);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return Json(new { success = true, idEmpresa = idEmpresa.Value });
                }
                else
                {
                    return Json(new { success = false, message = "No se actualizó ningún registro" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult RechazarOferta(int idNotificacion)
        {
            try
            {
                int idContacto = idNotificacion;
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (!idUsuario.HasValue)
                {
                    return RedirectToAction("Login", "Cuenta");
                }
                int? idEmpresa = null;
                using (var conexion = bd.establecerConexion())
                {
                    string query = @"UPDATE ingenieros_contactados 
                                     SET estado = 'rechazada', leido = true
                                     WHERE id_contacto = @IdContacto 
                                     AND id_usuario = @IdUsuario
                                     RETURNING id_empresa"
                                     ;
                    using (var comando = new NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("IdContacto", idContacto);
                        comando.Parameters.AddWithValue("IdUsuario", idUsuario.Value);
                        var result = comando.ExecuteScalar();
                        if (result != null)
                        {
                            idEmpresa = Convert.ToInt32(result);
                        }


                    }
                }

                if (idEmpresa.HasValue)
                {
                    using (var conexion = bd.establecerConexion())
                    {
                        string mensaje = "El ingeniero rechazo tu oferta";
                        string insertQuery = @"
                            INSERT INTO notificaciones_empresa (id_empresa, id_usuario, mensaje)
                            VALUES (@IdEmpresa, @IdUsuario, @Mensaje)";
                        using (var cmd = new NpgsqlCommand(insertQuery, conexion))
                        {   
                            cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa.Value);
                            cmd.Parameters.AddWithValue("IdUsuario", idUsuario.Value);
                            cmd.Parameters.AddWithValue("Mensaje", mensaje);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return Json(new { success = true, idEmpresa = idEmpresa.Value });

                }
                else
                {
                    return Json(new { success = false, message = "No se actualizó ningún registro" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult MarcarTodasComoLeidas()
        {
            try
            {
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (!idUsuario.HasValue)
                {
                    return Unauthorized();
                }

                using (var conexion = bd.establecerConexion())
                {
                    string query = @"UPDATE ingenieros_contactados
                             SET leido = true
                             WHERE id_usuario = @IdUsuario
                             AND (leido = false OR leido IS NULL)";
                    using (var comando = new NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdUsuario", idUsuario.Value);
                        comando.ExecuteNonQuery();
                    }
                }

                // 👇 Devolvemos un resultado JSON, no redirigimos
                return Json(new { success = true, message = "Notificaciones marcadas como leídas." });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error: " + e.Message });
            }
        }


    }
}




