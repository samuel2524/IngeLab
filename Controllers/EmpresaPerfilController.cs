// En Controllers/EmpresaPerfilController.cs
using Microsoft.AspNetCore.Mvc;
using IngeLab.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Npgsql;

namespace IngeLab.Controllers
{
    public class EmpresaPerfilController : Controller
    {
        BD bd = new BD();
        public IActionResult Index()
        {

            int idEmpresa = HttpContext.Session.GetInt32("EmpresaId") ?? 0;
            if (idEmpresa == 0)
            {
                return RedirectToAction("Index", "Login");
            }

            var viewModel = new EmpresaPerfilViewModel
            {
                Perfil_Empresa = ObtenerPerfilCompleto(idEmpresa),
                TecnologiasClave = ObtenerTecnologiasClave(idEmpresa),
                IsOwnProfile = true // Por ahora, luego puedes hacer lógica para perfiles de otros
                // OfertasAbiertas = new List<OfertaLaboral>
                // 
                //     new OfertaLaboral { Titulo = ".NET Developer Senior", Modalidad = "Remoto", Ubicacion = "LATAM" },
                // },

            };
            return View(viewModel);
        }


        private Empresas ObtenerPerfilCompleto(int idEmpresa)
        {
            var empresa = new Empresas();
            using (var conexion = bd.establecerConexion())
            {
                string query = @"
                    SELECT e.nombre, e.nit, e.correo, e.telefono,
                        dec.descripcion_empresa, dec.sitio_web, dec.sector, dec.ubicacion, dec.modalidad, dec.tamano, dec.tecnologia_clave
                    FROM empresas e
                    LEFT JOIN datosempresascompletar dec ON e.id_empresa = dec.id_empresa
                    WHERE e.id_empresa = @IdEmpresa";

                using (var cmd = new Npgsql.NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            empresa.Nombre = reader["nombre"]?.ToString() ?? "";
                            empresa.NIT = reader["nit"]?.ToString() ?? "";
                            empresa.Correo = reader["correo"]?.ToString() ?? "";
                            empresa.Telefono = reader["telefono"]?.ToString() ?? "";


                            empresa.Ubicacion = reader["ubicacion"]?.ToString() ?? "";
                            empresa.Sector = reader["sector"]?.ToString() ?? "";
                            empresa.Tamano = reader["tamano"]?.ToString() ?? "";
                            empresa.Modalidad = reader["modalidad"]?.ToString() ?? "";
                            empresa.Sitio_Web = reader["sitio_web"]?.ToString() ?? "";
                            empresa.Descripcion_Empresa = reader["descripcion_empresa"]?.ToString() ?? "";
                            empresa.Tecnologias_Clave = reader["tecnologia_clave"]?.ToString() ?? "";

                        }
                    }
                }
            }
            return empresa;
        }

        private List<string> ObtenerTecnologiasClave(int idEmpresa)
        {
            var tecnologias = new List<string>();

            using (var conexion = bd.establecerConexion())
            {
                string query = "SELECT tecnologia_clave FROM datosempresascompletar WHERE id_empresa = @IdEmpresa";

                using (var cmd = new Npgsql.NpgsqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("IdEmpresa", idEmpresa);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var techString = reader["tecnologia_clave"]?.ToString() ?? "";
                            tecnologias = techString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(t => t.Trim())
                                                    .ToList();
                        }
                    }
                }
            }

            return tecnologias;
        }
        
        [HttpPost]
        public IActionResult Editar(Empresas empresa)
        {
            try
            {
                using (var conexion = bd.establecerConexion())
                {
                    // Actualizar datos básicos en tabla "empresas"
                    string queryEmpresa = @"UPDATE empresas
                                            SET nombre = @Nombre,
                                                nit = @NIT,
                                                correo = @Correo,
                                                telefono = @Telefono
                                            WHERE id_empresa = @IdEmpresa";

                    using (var cmd = new NpgsqlCommand(queryEmpresa, conexion))
                    {
                        cmd.Parameters.AddWithValue("IdEmpresa", empresa.Id_empresa);
                        cmd.Parameters.AddWithValue("Nombre", empresa.Nombre);
                        cmd.Parameters.AddWithValue("NIT", empresa.NIT);
                        cmd.Parameters.AddWithValue("Correo", empresa.Correo);
                        cmd.Parameters.AddWithValue("Telefono", empresa.Telefono);
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            HttpContext.Session.SetInt32("EmpresaId", empresa.Id_empresa);
                        }
                    }

                    // Actualizar datos complementarios en "datosempresascompletar"
                    string queryDatos = @"UPDATE datosempresascompletar
                                        SET descripcion_empresa = @Descripcion,
                                            sitio_web = @SitioWeb,
                                            sector = @Sector,
                                            ubicacion = @Ubicacion,
                                            modalidad = @Modalidad,
                                            tamano = @Tamano,
                                            tecnologia_clave = @TecnologiaClave
                                        WHERE id_empresa = @IdEmpresa";

                    using (var cmd = new NpgsqlCommand(queryDatos, conexion))
                    {
                        cmd.Parameters.AddWithValue("IdEmpresa", empresa.Id_empresa);
                        cmd.Parameters.AddWithValue("Descripcion", empresa.Descripcion_Empresa ?? "");
                        cmd.Parameters.AddWithValue("SitioWeb", empresa.Sitio_Web ?? "");
                        cmd.Parameters.AddWithValue("Sector", empresa.Sector ?? "");
                        cmd.Parameters.AddWithValue("Ubicacion", empresa.Ubicacion ?? "");
                        cmd.Parameters.AddWithValue("Modalidad", empresa.Modalidad ?? "");
                        cmd.Parameters.AddWithValue("Tamano", empresa.Tamano ?? "");
                        cmd.Parameters.AddWithValue("TecnologiaClave", empresa.Tecnologias_Clave ?? "");
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            HttpContext.Session.SetInt32("EmpresaId", empresa.Id_empresa);
                        }
                    }
                }

                return RedirectToAction("Index", "EmpresaPerfil");
            }
            catch (Exception e)
            {
                return Content("No se pudo editar el perfil de empresa: " + e.Message);
            }
        }




    }
}