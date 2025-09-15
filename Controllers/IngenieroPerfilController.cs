// En Controllers/IngenieroPerfilController.cs
using Microsoft.AspNetCore.Mvc;
using IngeLab.Models;
using System.Collections.Generic;
using System;

namespace IngeLab.Controllers
{
    public class IngenieroPerfilController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new IngenieroPerfilViewModel
            {
                // En un caso real, buscarías el ingeniero en la BD.
                // Aquí simulamos que estamos viendo nuestro propio perfil.
                IsOwnProfile = true,

                Perfil = new Ingenieros
                {
                    Nombre = "Carlos",
                    Apellido = "Vallejo",
                    Especializacion = "Ingeniero de Software & Cloud Architect"
                },

                Habilidades = new List<string> { "C#", ".NET", "Azure", "DevOps", "Microservicios", "SQL", "React" },

                Experiencias = new List<Experiencia>
                {
                    new Experiencia { Cargo = "Senior Cloud Developer", Empresa = "TechSolutions S.A.", Periodo = "2022 - Presente", Descripcion = "Lideré la migración de sistemas monolíticos a una arquitectura de microservicios en Azure..." },
                    new Experiencia { Cargo = "Backend Developer", Empresa = "InnovaCore", Periodo = "2019 - 2022", Descripcion = "Desarrollo y mantenimiento de APIs RESTful para el sector financiero..." }
                },

                Educaciones = new List<Educacion>
                {
                    new Educacion { Institucion = "Universidad de Antioquia", Titulo = "Ingeniería de Sistemas", Periodo = "2014 - 2019" }
                },

                Posts = new List<Post>
                {
                    new Post { Id = 1, AutorNombre = "Carlos Vallejo", Contenido = "Mi post fijado sobre Clean Architecture...", FechaCreacion = DateTime.Now.AddDays(-10), IsPinned = true },
                    new Post { Id = 2, AutorNombre = "Carlos Vallejo", Contenido = "Explorando las nuevas características de .NET 9...", FechaCreacion = DateTime.Now.AddHours(-5), IsPinned = false }
                }
            };

            return View(viewModel);
        }
    }
}