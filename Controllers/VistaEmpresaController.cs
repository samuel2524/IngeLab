using IngeLab.Models;
using IngeLab.Models.NotificacionesIngeniero;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;


namespace IngeLab.Controllers
{
    public class VistaEmpresaController : Controller
    {
        BD bd = new BD();
        public IActionResult Index()
        {

            var idEmpresa = HttpContext.Session.GetInt32("EmpresaId");
            if (idEmpresa == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            var viewModel = new EmpresaDashboardViewModel
            {
                PerfilActual = ObtenerPerfilEmpresa(idEmpresa.Value),
                Notificaciones = ObtenerNotificacionesEmpresa(idEmpresa.Value),
                Contactados = ObtenerContactados(idEmpresa.Value),
                Deseados = ObtenerDeseados(idEmpresa.Value)

            };





            // Pasamos la lista de ingenieros a la vista.
            return View(viewModel);
        }

        private Empresas ObtenerPerfilEmpresa(int idEmpresa)
        {
            using (var conexion = bd.establecerConexion())
            {
                string query = "SELECT id_empresa, nombre, correo, telefono FROM empresas WHERE id_empresa = @IdEmpresa";
                using (var cmd = new NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Empresas
                            {
                                Id_empresa = Convert.ToInt32(reader["id_empresa"]),
                                Nombre = reader["nombre"].ToString(),
                                Correo = reader["correo"].ToString(),
                                Telefono = reader["telefono"].ToString()
                                // agrega otros campos que tenga tu clase Empresas
                            };
                        }
                    }
                }
            }
            return null;
        }

        [HttpPost]
        public IActionResult FiltrarAvanzado(FiltroIngenieroViewModel filtros)
        {
            var resultados = new List<Ingenieros>();

            using (var conexion = bd.establecerConexion())
            {
                var condiciones = new List<string>();
                var parametros = new Dictionary<string, object>();

                // Filtro por disponibilidad
                if (!string.IsNullOrEmpty(filtros.Disponibilidad))
                {
                    condiciones.Add("dp.disponibilidad = @Disponibilidad");
                    parametros.Add("Disponibilidad", filtros.Disponibilidad);
                }

                // Filtro por nivel académico
                if (!string.IsNullOrEmpty(filtros.Nivel_Academico))
                {
                    condiciones.Add("dp.nivel_academico = @Nivel_Academico");
                    parametros.Add("Nivel_Academico", filtros.Nivel_Academico);
                }

                // Filtro por años de experiencia
                if (filtros.Anios_Experiencia.HasValue)
                {
                    condiciones.Add("dp.anios_experiencia = @Anios_Experiencia");
                    parametros.Add("Anios_Experiencia", filtros.Anios_Experiencia.Value);
                }

                // Filtro por palabras clave
                if (!string.IsNullOrWhiteSpace(filtros.PalabrasClave))
                {
                    var palabras = filtros.PalabrasClave.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                        .Select(p => p.Trim().ToLower());

                    var subCondiciones = new List<string>();
                    int index = 0;
                    foreach (var palabra in palabras)
                    {
                        string paramName = $"Palabra{index}";
                        subCondiciones.Add($@"(
                            COALESCE(dp.habilidades_tecnicas, '') ILIKE '%' || @{paramName} || '%' OR
                            COALESCE(dp.especializacion, '') ILIKE '%' || @{paramName} || '%' OR
                            COALESCE(dp.idiomas, '') ILIKE '%' || @{paramName} || '%'
                        )");
                        parametros.Add(paramName, palabra);
                        index++;
                    }

                    condiciones.Add("(" + string.Join(" AND ", subCondiciones) + ")");
                }


                string traemosTodos = condiciones.Count > 0 ? "WHERE " + string.Join(" AND ", condiciones) : "";

                string query = $@"
                    SELECT u.id_usuario, u.nombre, u.apellidos, u.correo, u.telefono,
                        dp.habilidades_tecnicas, dp.especializacion, dp.idiomas,
                        dp.disponibilidad, dp.nivel_academico, dp.anios_experiencia
                    FROM usuarios u
                    INNER JOIN datosprofesionales dp ON u.id_usuario = dp.id_usuario
                    {traemosTodos}";

                Console.WriteLine("QUERY FINAL: " + query);
                foreach (var param in parametros)
                {
                    Console.WriteLine($"{param.Key} = {param.Value}");
                }


                using (var cmd = new NpgsqlCommand(query, conexion))
                {
                    foreach (var param in parametros)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ing = new Ingenieros
                            {
                                Id_Usuario = Convert.ToInt32(reader["id_usuario"]),
                                Nombre = reader["nombre"].ToString(),
                                Apellido = reader["apellidos"].ToString(),
                                Correo = reader["correo"].ToString(),
                                Telefono = reader["telefono"].ToString(),
                                Habilidades_Tecnicas = reader["habilidades_tecnicas"].ToString(),
                                Especializacion = reader["especializacion"].ToString(),
                                Idiomas = reader["idiomas"].ToString(),
                                Disponibilidad = reader["disponibilidad"].ToString(),
                                Nivel_Academico = reader["nivel_academico"].ToString(),
                                Anios_Experiencia = Convert.ToInt32(reader["anios_experiencia"])
                            };
                            resultados.Add(ing);
                        }
                    }
                }
            }

            filtros.Resultados = resultados;

            return View("~/Views/FiltrarIngeniero/Index.cshtml", filtros);
        }

        [HttpPost]
        public IActionResult ContactarIngeniero(int idIngeniero, string oferta)
        {
            // Obtener el id de la empresa desde la sesión
            int? idEmpresa = HttpContext.Session.GetInt32("EmpresaId");
            if (idEmpresa == null)
            {
                return Unauthorized(new { success = false, message = "Empresa no autenticada." });
            }
            using (var conexion = bd.establecerConexion())
            {
                //Insertar el contacto en la nueva tabla
                string insertQuery = @"
                    INSERT INTO ingenieros_contactados (id_usuario, id_empresa, oferta, fecha_contacto)
                    VALUES (@IdUsuario, @IdEmpresa, @Oferta, NOW())
                ";


                using (var cmd = new NpgsqlCommand(insertQuery, conexion))
                {
                    cmd.Parameters.AddWithValue("IdUsuario", idIngeniero);
                    cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa.Value);

                    cmd.Parameters.AddWithValue("Oferta", oferta);

                    cmd.ExecuteNonQuery();
                }
            }

            // Respuesta para AJAX
            return Ok(new { success = true, message = "  contactado y guardado correctamente" });
        }


        private List<Ingenieros> ObtenerContactados(int idEmpresa)
        {
            var lista = new List<Ingenieros>();

            using (var conexion = bd.establecerConexion())
            {
                string query = @"
                    SELECT ic.id_contacto, ic.oferta, ic.fecha_contacto,
                        u.id_usuario, u.nombre, u.apellidos, u.correo, u.telefono,
                        dp.habilidades_tecnicas, dp.especializacion, dp.idiomas,
                        dp.disponibilidad, dp.nivel_academico, dp.anios_experiencia
                    FROM ingenieros_contactados ic
                    INNER JOIN usuarios u ON ic.id_usuario = u.id_usuario
                    INNER JOIN datosprofesionales dp ON u.id_usuario = dp.id_usuario
                    WHERE ic.id_empresa = @IdEmpresa
                    ORDER BY ic.fecha_contacto DESC";

                using (var cmd = new NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Ingenieros
                            {
                                Id_Usuario = Convert.ToInt32(reader["id_usuario"]),
                                Id_Contactado = Convert.ToInt32(reader["id_contacto"]),
                                Nombre = reader["nombre"].ToString(),
                                Apellido = reader["apellidos"].ToString(),
                                Correo = reader["correo"].ToString(),
                                Telefono = reader["telefono"].ToString(),
                                Habilidades_Tecnicas = reader["habilidades_tecnicas"].ToString(),
                                Especializacion = reader["especializacion"].ToString(),
                                Idiomas = reader["idiomas"].ToString(),
                                Disponibilidad = reader["disponibilidad"].ToString(),
                                Nivel_Academico = reader["nivel_academico"].ToString(),
                                Anios_Experiencia = reader["anios_experiencia"] != DBNull.Value
                                    ? Convert.ToInt32(reader["anios_experiencia"])
                                    : 0,
                                Oferta = reader["oferta"].ToString()
                            });
                        }
                    }
                }
            }

            Console.WriteLine($"Contactados para empresa {idEmpresa}: {lista.Count}");
            return lista;
        }


        [HttpPost]
        public IActionResult EliminarContactado(int idContactado)
        {
            int? idEmpresa = HttpContext.Session.GetInt32("EmpresaId");
            if (idEmpresa == null)
                return RedirectToAction("Login", "Cuenta");

            using (var conexion = bd.establecerConexion())
            {
                string query = "DELETE FROM ingenieros_contactados WHERE id_contacto = @IdContacto AND id_empresa = @IdEmpresa";
                using (var cmd = new NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdContacto", idContactado);
                    cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa.Value);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index"); // Recarga la vista para reflejar cambios
        }

        


        [HttpPost]
        public IActionResult ListaDeseados(int idIngeniero)
        {
            try
            {
                int? idEmpresa = HttpContext.Session.GetInt32("EmpresaId");
                if (idEmpresa == null)
                {
                return Unauthorized(new { success = false, message = "Empresa no autenticada." });
                }

                using (var conexion = bd.establecerConexion())
                {
                    string query = @"
                        INSERT INTO ingenieros_deseados (id_usuario, id_empresa, fecha_deseado)
                        VALUES (@IdUsuario, @IdEmpresa, NOW())
                    ";
                    using (var cmd = new NpgsqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("IdUsuario", idIngeniero);
                        cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Respondemos para AJAX
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }


        private List<Ingenieros> ObtenerDeseados(int idEmpresa)
        {
            var lista = new List<Ingenieros>();

            using (var conexion = bd.establecerConexion())
            {
               string query = @"
                    SELECT 
                        ides.id_ingenierosdeseados,
                        ides.id_usuario,
                        ides.fecha_deseado,
                        u.nombre,
                        u.apellidos,
                        u.correo,
                        u.telefono,
                        dp.habilidades_tecnicas,
                        dp.especializacion,
                        dp.idiomas,
                        dp.disponibilidad,
                        dp.nivel_academico,
                        dp.anios_experiencia
                    FROM ingenieros_deseados ides
                    INNER JOIN usuarios u ON ides.id_usuario = u.id_usuario
                    INNER JOIN datosprofesionales dp ON u.id_usuario = dp.id_usuario
                    WHERE ides.id_empresa = @IdEmpresa
                    ORDER BY ides.fecha_deseado DESC";



                using (var cmd = new NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Ingenieros
                            {
                                Id_Usuario = Convert.ToInt32(reader["id_usuario"]),
                                Id_Deseado = Convert.ToInt32(reader["id_ingenierosdeseados"]),
                                Nombre = reader["nombre"].ToString(),
                                Apellido = reader["apellidos"].ToString(),
                                Correo = reader["correo"].ToString(),
                                Telefono = reader["telefono"].ToString(),
                                Habilidades_Tecnicas = reader["habilidades_tecnicas"].ToString(),
                                Especializacion = reader["especializacion"].ToString(),
                                Idiomas = reader["idiomas"].ToString(),
                                Disponibilidad = reader["disponibilidad"].ToString(),
                                Nivel_Academico = reader["nivel_academico"].ToString(),
                                Anios_Experiencia = reader["anios_experiencia"] != DBNull.Value
                                    ? Convert.ToInt32(reader["anios_experiencia"])
                                    : 0,
                            });
                        }
                    }
                }
            }

            return lista;
        }


        
        [HttpPost]
        public IActionResult EliminarDeseado(int idDeseado)
        {
            int? idEmpresa = HttpContext.Session.GetInt32("EmpresaId");
            if (idEmpresa == null)
                return RedirectToAction("Login", "Cuenta");

            using (var conexion = bd.establecerConexion())
            {
                string query = "DELETE FROM ingenieros_deseados WHERE id_ingenierosdeseados = @IdDeseado AND id_empresa = @IdEmpresa";
                using (var cmd = new NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdDeseado", idDeseado);
                    cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa.Value);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index"); // Recarga la vista para reflejar cambios
        }


        




        private List<NotificacionEmpresa> ObtenerNotificacionesEmpresa(int idEmpresa)
        {
            var lista = new List<NotificacionEmpresa>();

            using (var conexion = bd.establecerConexion())
            {
                string query = @"
                    SELECT ne.id_notificacion, ne.id_usuario, u.nombre, u.apellidos, 
                        ne.mensaje, ne.fecha, ne.leido
                    FROM notificaciones_empresa ne
                    INNER JOIN usuarios u ON ne.id_usuario = u.id_usuario
                    WHERE ne.id_empresa = @IdEmpresa
                    ORDER BY ne.fecha DESC";


                using (var cmd = new NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new NotificacionEmpresa
                            {
                                IdNotificacion = Convert.ToInt32(reader["id_notificacion"]),
                                IdUsuario = Convert.ToInt32(reader["id_usuario"]),
                                Mensaje = reader["mensaje"].ToString(),
                                NombreUsuario = reader["nombre"].ToString() + " " + reader["apellidos"].ToString(),
                                Fecha = Convert.ToDateTime(reader["fecha"]),
                                Leido = Convert.ToBoolean(reader["leido"])

                            });
                        }
                    }
                }
            }

            return lista;
        }
        [HttpPost]
        public IActionResult MarcarComoLeida()
        {
            try
            {
                int? idEmpresa = HttpContext.Session.GetInt32("EmpresaId");
                if (idEmpresa == null) return Unauthorized();

                using (var conexion = bd.establecerConexion())
                {
                    string query = @"UPDATE notificaciones_empresa 
                                    SET leido = true 
                                    WHERE id_empresa = @IdEmpresa";
                    using (var cmd = new NpgsqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Respondemos para AJAX
                return Ok(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }




    }
}