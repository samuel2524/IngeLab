// En Controllers/IngenieroPerfilController.cs
using Microsoft.AspNetCore.Mvc;
using IngeLab.Models;
using System.Collections.Generic;
using System;
using Npgsql;
using Newtonsoft.Json;

namespace IngeLab.Controllers
{
    public class IngenieroPerfilController : Controller
    {
        BD bd = new BD();
        public IActionResult Index()
        {
            int idUsuario = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            if (idUsuario == 0)
            {
                return RedirectToAction("Index", "Login");
            }

            var viewModel = new IngenieroPerfilViewModel
            {
                Perfil_Ingeniero = ObtenerPerfilCompleto(idUsuario),
                Lista_Habilidades = ObtenerHabilidades(idUsuario),
                Idiomas = ObtenerIdiomas(idUsuario),
                Posts = ObtenerPosts(idUsuario),
                IsOwnProfile = true // Por ahora, luego puedes hacer lógica para perfiles de otros
            };
            return View(viewModel);
        }

        private Ingenieros ObtenerPerfilCompleto(int idUsuario)
        {
            var ingeniero = new Ingenieros();
            using (var conexion = bd.establecerConexion())
            {
                string query = @"
                    SELECT u.nombre, u.apellidos, u.correo, u.telefono,
                        u.numero_documento,
                        dp.anios_experiencia, dp.nivel_academico, 
                        dp.habilidades_tecnicas, dp.especializacion,
                        dp.idiomas, dp.disponibilidad,
                        e.nombre AS empresa_contratante
                    FROM usuarios u
                    LEFT JOIN datosprofesionales dp ON u.id_usuario = dp.id_usuario
                    LEFT JOIN ingenieros_contactados ic ON ic.id_usuario = u.id_usuario AND ic.estado = 'aceptada'
                    LEFT JOIN empresas e ON ic.id_empresa = e.id_empresa
                    WHERE u.id_usuario = @IdUsuario
                    LIMIT 1";

                using (var cmd = new Npgsql.NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdUsuario", idUsuario);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ingeniero.Nombre = reader["nombre"].ToString();
                            ingeniero.Apellido = reader["apellidos"].ToString();
                            ingeniero.Correo = reader["correo"].ToString();
                            ingeniero.Telefono = reader["telefono"].ToString();
                            ingeniero.NumeroDocumento = reader["numero_documento"].ToString();

                            ingeniero.Anios_Experiencia = reader["anios_experiencia"] as int? ?? 0;
                            ingeniero.Nivel_Academico = reader["nivel_academico"]?.ToString() ?? "";
                            ingeniero.Habilidades_Tecnicas = reader["habilidades_tecnicas"]?.ToString() ?? "";
                            ingeniero.Especializacion = reader["especializacion"]?.ToString() ?? "";
                            ingeniero.Idiomas = reader["idiomas"]?.ToString() ?? "";
                            ingeniero.Disponibilidad = reader["disponibilidad"]?.ToString() ?? "";
                            ingeniero.ContratadoPor = reader["empresa_contratante"]?.ToString() ??"";


                        }
                    }

                }
            }
            return ingeniero;
        }


        private List<string> ObtenerHabilidades(int idUsuario)
        {
            var habilidades = new List<string>();

            using (var conexion = bd.establecerConexion())
            {
                string query = "SELECT habilidades_tecnicas FROM datosprofesionales WHERE id_usuario = @IdUsuario";

                using (var cmd = new Npgsql.NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdUsuario", idUsuario);
                    var result = cmd.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(result))
                    {
                        habilidades = result.Split(',').Select(h => h.Trim()).ToList();
                    }
                }
            }

            return habilidades;
        }

        private List<string> ObtenerIdiomas(int idUsuario)
        {
            var idiomas = new List<string>();
            using (var conexion = bd.establecerConexion())
            {
                string query = "SELECT idiomas FROM datosprofesionales WHERE id_usuario = @IdUsuario";

                using (var cmd = new Npgsql.NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdUsuario", idUsuario);
                    var result = cmd.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(result))
                    {
                        idiomas = result.Split(',').Select(h => h.Trim()).ToList();
                    }
                }
            }
            return idiomas;

        }


        // En tu archivo Controllers/IngenieroPerfilController.cs

        private List<Post> ObtenerPosts(int idUsuario)
        {
            var posts = new List<Post>();
            using (var conexion = bd.establecerConexion())
            {
                var query = @"
            SELECT
                p.id_post, p.contenido, p.fecha_public, p.tipo_contenido, p.fijado,
                p.id_post_padre,
                padre.contenido AS contenido_padre,
                autor_padre.nombre || ' ' || autor_padre.apellidos AS autor_padre
            FROM
                postingeniero p
            LEFT JOIN
                postingeniero AS padre ON p.id_post_padre = padre.id_post
            LEFT JOIN
                usuarios AS autor_padre ON padre.id_usuario = autor_padre.id_usuario
            WHERE
                p.id_usuario = @IdUsuario
            ORDER BY
                p.fijado DESC, p.fecha_public DESC"; // ✨ ¡AQUÍ ESTABA EL DETALLE! RESTAURAMOS EL ORDEN CORRECTO ✨

                using (var comando = new NpgsqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Post
                            {
                                Id_Post = reader.GetInt32(reader.GetOrdinal("id_post")),
                                Contenido = reader.GetString(reader.GetOrdinal("contenido")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_public")),
                                Tipo_Contenido = reader.GetString(reader.GetOrdinal("tipo_contenido")),
                                Fijado = reader.GetBoolean(reader.GetOrdinal("fijado")),
                                IdPostPadre = reader.IsDBNull(reader.GetOrdinal("id_post_padre")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("id_post_padre")),
                                ContenidoPadre = reader.IsDBNull(reader.GetOrdinal("contenido_padre")) ? null : reader.GetString(reader.GetOrdinal("contenido_padre")),
                                AutorPadre = reader.IsDBNull(reader.GetOrdinal("autor_padre")) ? null : reader.GetString(reader.GetOrdinal("autor_padre"))
                            };

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

                            posts.Add(post);
                        }
                    }
                }
            }
            return posts;
        }



        [HttpPost]
        public IActionResult Editar(Ingenieros ingenieros)
        {
            try
            {


                using (var conexion = bd.establecerConexion())
                {
                    string queryUsuario = @"UPDATE usuarios
                                    SET nombre = @Nombre, apellidos = @Apellido,numero_documento = @NumeroDocumento,
                                    correo = @Correo, telefono = @Telefono
                                    WHERE id_usuario = @IdUsuario";

                    using (var cmd = new NpgsqlCommand(queryUsuario, conexion))
                    {
                        cmd.Parameters.AddWithValue("IdUsuario", ingenieros.Id_Usuario);
                        cmd.Parameters.AddWithValue("Nombre", ingenieros.Nombre);
                        cmd.Parameters.AddWithValue("Apellido", ingenieros.Apellido);
                        cmd.Parameters.AddWithValue("Correo", ingenieros.Correo);
                        cmd.Parameters.AddWithValue("Telefono", ingenieros.Telefono);
                        cmd.Parameters.AddWithValue("NumeroDocumento", ingenieros.NumeroDocumento);
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            HttpContext.Session.SetInt32("UsuarioId", ingenieros.Id_Usuario);
                        }

                    }

                    string queryDatos = @"Update datosprofesionales SET anios_experiencia = @Anios,
                                      nivel_academico = @Nivel,
                                      habilidades_tecnicas = @Habilidades_Tecnicas,
                                      idiomas = @Idiomas,
                                      disponibilidad = @Disponibilidad
                                      WHERE id_usuario = @IdUsuario";
                                      
                    using (var cmd = new Npgsql.NpgsqlCommand(queryDatos, conexion))
                    {
                        cmd.Parameters.AddWithValue("IdUsuario", ingenieros.Id_Usuario);
                        cmd.Parameters.AddWithValue("Disponibilidad", ingenieros.Disponibilidad);
                        cmd.Parameters.AddWithValue("Anios", ingenieros.Anios_Experiencia);
                        cmd.Parameters.AddWithValue("Nivel", ingenieros.Nivel_Academico);
                        cmd.Parameters.AddWithValue("Habilidades_Tecnicas", ingenieros.Habilidades_Tecnicas);
                        cmd.Parameters.AddWithValue("Idiomas", ingenieros.Idiomas);
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            HttpContext.Session.SetInt32("UsuarioId", ingenieros.Id_Usuario);
                        }
                    }
                    
                }




                return RedirectToAction("Index", "IngenieroPerfil");

                
            }
            catch (System.Exception e)
            {

                return Content("no se pudo editar el perfil" + e.Message);
            }
        }



    }
}