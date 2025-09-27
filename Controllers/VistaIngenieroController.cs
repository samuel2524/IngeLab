// En Controllers/VistaIngenierosController.cs

using Microsoft.AspNetCore.Mvc;
using IngeLab.Models; // Aseg�rate de tener los usings
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
        public IActionResult Index()
        {

            var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            if (!idUsuario.HasValue)
            {
                // Redirige al login o muestra un mensaje claro
                return RedirectToAction("Login", "Cuenta");
            }
            var perfil = ObtenerPerfilPorId(idUsuario.Value);
            var post = ObtenerPostsPorUsuario(idUsuario.Value);
            var notificaciones = ObtenerNotificaciones(idUsuario.Value);

            // Simulación del Dashboard del Ingeniero 
            var viewModel = new IngenieroDashboardViewModel
            {
                PerfilActual = perfil,
                PostsPublicados = post,
                Notificaciones = notificaciones,
                HabilidadesEnTendencia = new List<string> { "IA Generativa", "Rust", "Clean Architecture", "DevSecOps", "Blazor" }


                // PostsFeed = new List<Post>
                // {
                //     new Post { Id = 1, AutorNombre = "Carlos Vallejo", AutorEspecialidad = "Ingenier�a de Software", Contenido = "Acabo de terminar un curso de optimizaci�n de bases de datos con PostgreSQL. �Una locura lo que se puede lograr con los �ndices correctos! #Database #Performance", FechaCreacion = DateTime.Now.AddHours(-2) },
                //     new Post { Id = 2, AutorNombre = "Valeria Rojas", AutorEspecialidad = "Ingenier�a de Software", Contenido = "Explorando el nuevo SDK de .NET 9. Las mejoras en AOT nativo son un cambio de juego para las aplicaciones serverless.", FechaCreacion = DateTime.Now.AddHours(-5) },
                //     new Post { Id = 3, AutorNombre = "Mateo Garc�a", AutorEspecialidad = "Ingenier�a Civil", Contenido = "Comparto un render del �ltimo proyecto de puente atirantado en el que particip�. La simulaci�n de vientos fue todo un reto.", FechaCreacion = DateTime.Now.AddDays(-1) }
                // },

                // Notificaciones = new List<NotificacionOferta>
                // {
                //     new NotificacionOferta { EmpresaNombre = "TechSolutions S.A.", TituloOferta = ".NET Developer Senior", IsLeida = false },
                //     new NotificacionOferta { EmpresaNombre = "InnovaCore", TituloOferta = "Cloud Architect (Azure)", IsLeida = false },
                //     new NotificacionOferta { EmpresaNombre = "DataDriven Co.", TituloOferta = "Backend Engineer", IsLeida = true }
                // },


            };

            return View(viewModel);
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
                        comando.Parameters.AddWithValue("@Id_Usuario", ingenieros.Id_Usuario); // Asegúrate de que Id_Usuario esté correctamente asignado
                        comando.Parameters.AddWithValue("@Contenido", ingenieros.Contenido); // Reemplaza con el contenido real del post
                        comando.Parameters.AddWithValue("@FechaCreacion", DateTime.Now); // Reemplaza con la fecha real de creación
                        comando.Parameters.AddWithValue("@Tipo_Contenido", "texto"); // Reemplaza con el tipo de contenido real
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
        // 👇 CAMBIAMOS LOS PARÁMETROS DE LA ACCIÓN
        public IActionResult PublicarCodigo(string TextoExplicativo, string Codigo, string Tipo_Contenido)
        {
            try
            {
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (!idUsuario.HasValue)
                {
                    return Unauthorized();
                }

                // ✨ AQUÍ CREAMOS EL PAQUETE JSON ✨
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
                        comando.Parameters.AddWithValue("@Contenido", contenidoJson); // Guardamos el string JSON
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



        // EN: VistaIngenierosController.cs

        public List<Post> ObtenerPostsPorUsuario(int idUsuario)
        {
            var posts = new List<Post>();
            using (var conexion = bd.establecerConexion())
            {
                var query = "SELECT id_post, contenido, fecha_public, tipo_contenido, fijado FROM postingeniero WHERE id_usuario = @IdUsuario ORDER BY fijado DESC, fecha_public DESC";
                using (var comando = new NpgsqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Post
                            {
                                Id_Post = reader.GetInt32(0),
                                Contenido = reader.GetString(1),
                                FechaCreacion = reader.GetDateTime(2),
                                Tipo_Contenido = reader.GetString(3),
                                Fijado = reader.GetBoolean(4)
                            };

                            // ✨ AQUÍ DESEMPACAMOS EL JSON ✨
                            if (post.Tipo_Contenido != "texto" && !string.IsNullOrEmpty(post.Contenido))
                            {
                                try
                                {
                                    // Leemos el JSON y lo separamos en nuestras propiedades "ayudantes"
                                    dynamic data = JsonConvert.DeserializeObject(post.Contenido);
                                    post.TextoExplicativo = data.texto;
                                    post.Codigo = data.codigo;
                                }
                                catch
                                {
                                    // Si falla (por si tienes posts antiguos), muestra el contenido como código
                                    post.TextoExplicativo = "";
                                    post.Codigo = post.Contenido;
                                }
                            }
                            else
                            {
                                // Para posts de texto, el contenido es el texto explicativo.
                                post.TextoExplicativo = post.Contenido;
                            }

                            posts.Add(post);
                        }
                    }
                }
            }
            return posts;
        }

        // public IActionResult Notificaciones()
        // {
        //     var idIngeniero = ObtenerPerfilPorId(HttpContext.Session.GetInt32("UsuarioId") ?? 0)?.Id_Usuario ?? 0;
        //     Console.WriteLine($"IdIngeniero en sesión -> {idIngeniero}");
        //     var perfil = ObtenerPerfilPorId(idIngeniero);
        //     var posts = ObtenerPostsPorUsuario(idIngeniero);
        //     var notificaciones = ObtenerNotificaciones(idIngeniero);
        //     var viewModel = new IngenieroDashboardViewModel
        //     {
        //         Notificaciones = notificaciones,
        //         PerfilActual = perfil,
        //         PostsPublicados = posts
        //     };

        //     return View(viewModel);
        // }

        private List<Notificacion> ObtenerNotificaciones(int idUsuario)
        {
            var notificaciones = new List<Notificacion>();

            using (var conexion = bd.establecerConexion())
            {
                string query = @"
                    SELECT ic.id_contacto, ic.oferta, ic.fecha_contacto, e.nombre
                    FROM ingenieros_contactados ic
                    INNER JOIN empresas e ON ic.id_empresa = e.id_empresa
                    WHERE ic.id_usuario = @Id 
                    AND (ic.leido = false OR ic.leido IS NULL) 
                    ORDER BY ic.fecha_contacto DESC";

                using (var cmd = new NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("Id", idUsuario);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"DEBUG DB -> id_contacto: {reader["id_contacto"]}, usuario: {idUsuario}");
                            notificaciones.Add(new Notificacion
                            {
                                IdContacto = Convert.ToInt32(reader["id_contacto"]),
                                Oferta = reader["oferta"].ToString(),
                                Empresa = reader["nombre"].ToString(),
                                Fecha = Convert.ToDateTime(reader["fecha_contacto"])
                            });
                        }
                    }
                }
            }

            return notificaciones;
        }


        [HttpPost]
        public IActionResult AceptarOferta(int idNotificacion)
        {
            try
            {
                int idContacto = idNotificacion; // ID del contacto a aceptar
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (!idUsuario.HasValue)
                {
                    return RedirectToAction("Login", "Cuenta");
                }
                int? idEmpresa = null;
                using (var conexion = bd.establecerConexion())
                {
                    Console.WriteLine($"Contacto: {idContacto}, Usuario: {idUsuario.Value}");

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

                using (var conexion = bd.establecerConexion())
                {
                    Console.WriteLine($"Rechazar -> Contacto: {idContacto}, Usuario: {idUsuario.Value}");

                    string query = @"UPDATE ingenieros_contactados 
                                    SET estado = 'rechazada', leido = true
                                    WHERE id_contacto = @IdContacto 
                                    AND id_usuario = @IdUsuario";

                    using (var comando = new NpgsqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("IdContacto", idContacto);
                        comando.Parameters.AddWithValue("IdUsuario", idUsuario.Value);
                        int filas = comando.ExecuteNonQuery();

                        if (filas > 0)
                            return Json(new { success = true });
                        else
                            return Json(new { success = false, message = "No se actualizó ningún registro" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        

        
    }

}

