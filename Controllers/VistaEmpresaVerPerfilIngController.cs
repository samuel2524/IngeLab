using IngeLab.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;

namespace IngeLab.Controllers
{
    public class VistaEmpresaVerPerfilIng : Controller
    {
        BD bD = new BD();
        public IActionResult Index(int idUsuario)
        {
              var perfil = ObtenerPerfilIngeniero(idUsuario);
              var datosProfesionales = ObtenerDatosProfesionales(idUsuario);

              if (perfil == null)
              return NotFound();

              var viewModel = new IngenieroPerfilViewModel
              {
                    Perfil_Ingeniero = perfil,
                    Lista_Habilidades = datosProfesionales?.Habilidades_Tecnicas?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                            .Select(x => x.Trim()).ToList() ?? new List<string>(),
                    Idiomas = datosProfesionales?.Idiomas?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                            .Select(x => x.Trim()).ToList() ?? new List<string>(),
                    Posts = ObtenerPosts(idUsuario),
                    IsOwnProfile = false
              };

                return View("~/Views/VerPerfilIngeniero/Index.cshtml", viewModel);
        }


        private Ingenieros ObtenerPerfilIngeniero(int idUsuario)
        {
            using (var conexion = bD.establecerConexion())
            {
                 string query = @"SELECT 
                            u.id_usuario AS usuario_id,
                            u.nombre,
                            u.apellidos,
                            dp.especializacion,
                            dp.disponibilidad,
                            dp.nivel_academico,
                            dp.anios_experiencia,
                            dp.habilidades_tecnicas,
                            dp.idiomas
                         FROM usuarios u
                         LEFT JOIN datosprofesionales dp ON u.id_usuario = dp.id_usuario
                         WHERE u.id_usuario = @IdUsuario";

                using (var cmd = new NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdUsuario", idUsuario);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Ingenieros
                            {
                                Id_Usuario = Convert.ToInt32(reader["usuario_id"]),
                                Nombre = reader["nombre"].ToString(),
                                Apellido = reader["apellidos"].ToString(),
                                Especializacion = reader["especializacion"].ToString(),
                                Disponibilidad = reader["disponibilidad"].ToString(),
                                Nivel_Academico = reader["nivel_academico"].ToString(),
                                Anios_Experiencia = reader["anios_experiencia"] != DBNull.Value
                                                    ? Convert.ToInt32(reader["anios_experiencia"])
                                                    : 0,
                                
                            };
                        }
                    }
                }

                return null;
            }

        }

        // En Controllers/VistaEmpresaVerPerfilIngController.cs

        private List<Post> ObtenerPosts(int idUsuario)
        {
            var posts = new List<Post>();
            using (var conexion = bD.establecerConexion()) // Tu variable de conexión es bD
            {
                // ✨ ESTA ES LA QUERY ACTUALIZADA CON LOS JOINS MÁGICOS ✨
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
                p.fecha_public DESC";

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

                                // Mapeamos los nuevos campos de contexto
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
                                catch
                                {
                                    post.TextoExplicativo = "";
                                    post.Codigo = post.Contenido;
                                }
                            }

                            posts.Add(post);
                        }
                    }
                }
            }
            return posts;
        }


        private Ingenieros ObtenerDatosProfesionales(int idUsuario)
        {
            using var conexion = bD.establecerConexion();
            string query = @"SELECT anios_experiencia, nivel_academico, habilidades_tecnicas, especializacion, idiomas, disponibilidad
                            FROM datosprofesionales
                            WHERE id_usuario = @IdUsuario";

            using var cmd = new NpgsqlCommand(query, conexion);
            cmd.Parameters.AddWithValue("IdUsuario", idUsuario);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Ingenieros
                {
                    Id_Usuario = idUsuario,
                    Anios_Experiencia = reader["anios_experiencia"] != DBNull.Value
                                        ? Convert.ToInt32(reader["anios_experiencia"])
                                        : 0,
                    Nivel_Academico = reader["nivel_academico"].ToString(),
                    Habilidades_Tecnicas = reader["habilidades_tecnicas"].ToString(),
                    Especializacion = reader["especializacion"].ToString(),
                    Idiomas = reader["idiomas"].ToString(),
                    Disponibilidad = reader["disponibilidad"].ToString()
                };
            }

            return null; // si no hay datos para ese usuario
        }



    }
}