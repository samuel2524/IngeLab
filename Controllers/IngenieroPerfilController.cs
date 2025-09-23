// En Controllers/IngenieroPerfilController.cs
using Microsoft.AspNetCore.Mvc;
using IngeLab.Models;
using System.Collections.Generic;
using System;
using Npgsql;

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


        private List<Post> ObtenerPosts(int idUsuario)
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
                            posts.Add(new Post
                            {
                                Id_Post = reader.GetInt32(0),
                                Id_Usuario = idUsuario,  
                                Contenido = reader.GetString(1),
                                FechaCreacion = reader.GetDateTime(2),
                                Tipo_Contenido = reader.GetString(3),
                                Fijado = reader.GetBoolean(4)
                            });
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