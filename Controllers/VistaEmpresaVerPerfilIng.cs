using IngeLab.Models;
using Microsoft.AspNetCore.Mvc;
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

        private List<Post> ObtenerPosts(int idUsuario)
        {
            var posts = new List<Post>();

            using (var conexion = bD.establecerConexion())
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