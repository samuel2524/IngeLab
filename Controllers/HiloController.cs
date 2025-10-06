using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using IngeLab.Models;
using Npgsql;
using System.Collections.Generic;
using Newtonsoft.Json; // Asegúrate de tener este using

namespace IngeLab.Controllers
{
    public class HiloController : Controller
    {
        private readonly BD bd = new BD();

        // Esta es la acción que mostrará la página del hilo
        public IActionResult Detalle(int id)
        {
            var hiloPrincipal = ObtenerPostPorId(id);

            if (hiloPrincipal == null)
            {
                return NotFound();
            }

            // ✨ LÓGICA DE DECISIÓN ✨
            // Revisamos si en la sesión hay una ID de empresa.
            var esUnaEmpresaViendo = HttpContext.Session.GetInt32("EmpresaId").HasValue;

            var viewModel = new HiloDetalleViewModel
            {
                HiloPrincipal = hiloPrincipal,
                Respuestas = ObtenerRespuestasDePost(id),
                // Si NO es una empresa, entonces puede responder.
                PuedeResponder = !esUnaEmpresaViendo,
                // Guardamos el ID del autor del hilo para saber a qué perfil volver.
                PerfilUsuarioId = hiloPrincipal.Id_Usuario
            };

            return View(viewModel);
        }

        // Método privado para buscar un solo post por su ID
        private PostViewModel ObtenerPostPorId(int idPost)
        {
            // Esta lógica es muy parecida a la de tu ObtenerFeedGlobal
            // pero filtrando por un solo id_post.
            PostViewModel post = null;
            using (var conexion = bd.establecerConexion())
            {
                var query = @"
                    SELECT p.id_post, p.id_usuario, p.contenido, p.fecha_public, 
                           p.tipo_contenido, p.fijado, u.nombre, u.apellidos,
                           (SELECT COUNT(*) FROM postingeniero AS r WHERE r.id_post_padre = p.id_post) AS cantidad_respuestas
                    FROM postingeniero p
                    INNER JOIN usuarios u ON p.id_usuario = u.id_usuario
                    WHERE p.id_post = @IdPost";

                using (var comando = new NpgsqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@IdPost", idPost);
                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            post = MapearPostDesdeReader(reader);
                        }
                    }
                }
            }
            return post;
        }

        // Método privado para buscar todas las respuestas de un post padre
        private List<PostViewModel> ObtenerRespuestasDePost(int idPostPadre)
        {
            var respuestas = new List<PostViewModel>();
            using (var conexion = bd.establecerConexion())
            {
                var query = @"
                    SELECT p.id_post, p.id_usuario, p.contenido, p.fecha_public, 
                           p.tipo_contenido, p.fijado, u.nombre, u.apellidos
                    FROM postingeniero p
                    INNER JOIN usuarios u ON p.id_usuario = u.id_usuario
                    WHERE p.id_post_padre = @IdPostPadre
                    ORDER BY p.fecha_public ASC"; // Las respuestas se ordenan de la más antigua a la más nueva

                using (var comando = new NpgsqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@IdPostPadre", idPostPadre);
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            respuestas.Add(MapearPostDesdeReader(reader, esRespuesta: true));
                        }
                    }
                }
            }
            return respuestas;
        }

        // Función de ayuda para no repetir código. Mapea un DataReader a un PostViewModel
        private PostViewModel MapearPostDesdeReader(NpgsqlDataReader reader, bool esRespuesta = false)
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
                CantidadRespuestas = esRespuesta ? 0 : System.Convert.ToInt32(reader["cantidad_respuestas"])
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
            return post;
        }
    }
}

