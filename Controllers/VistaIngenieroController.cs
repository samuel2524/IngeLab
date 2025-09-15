// En Controllers/VistaIngenierosController.cs

using Microsoft.AspNetCore.Mvc;
using IngeLab.Models; // Asegúrate de tener los usings
using System.Collections.Generic;
using System;

namespace IngeLab.Controllers
{
    public class VistaIngenierosController : Controller
    {
        public IActionResult Index()
        {
            // Simulación del Dashboard del Ingeniero 
            var viewModel = new IngenieroDashboardViewModel
            {
                PerfilActual = new Ingenieros { Nombre = "Carlos", Apellido = "Vallejo", Especializacion = "Ingeniería de Software" },

                PostsFeed = new List<Post>
                {
                    new Post { Id = 1, AutorNombre = "Carlos Vallejo", AutorEspecialidad = "Ingeniería de Software", Contenido = "Acabo de terminar un curso de optimización de bases de datos con PostgreSQL. ¡Una locura lo que se puede lograr con los índices correctos! #Database #Performance", FechaCreacion = DateTime.Now.AddHours(-2) },
                    new Post { Id = 2, AutorNombre = "Valeria Rojas", AutorEspecialidad = "Ingeniería de Software", Contenido = "Explorando el nuevo SDK de .NET 9. Las mejoras en AOT nativo son un cambio de juego para las aplicaciones serverless.", FechaCreacion = DateTime.Now.AddHours(-5) },
                    new Post { Id = 3, AutorNombre = "Mateo García", AutorEspecialidad = "Ingeniería Civil", Contenido = "Comparto un render del último proyecto de puente atirantado en el que participé. La simulación de vientos fue todo un reto.", FechaCreacion = DateTime.Now.AddDays(-1) }
                },

                Notificaciones = new List<NotificacionOferta>
                {
                    new NotificacionOferta { EmpresaNombre = "TechSolutions S.A.", TituloOferta = ".NET Developer Senior", IsLeida = false },
                    new NotificacionOferta { EmpresaNombre = "InnovaCore", TituloOferta = "Cloud Architect (Azure)", IsLeida = false },
                    new NotificacionOferta { EmpresaNombre = "DataDriven Co.", TituloOferta = "Backend Engineer", IsLeida = true }
                },

                HabilidadesEnTendencia = new List<string> { "IA Generativa", "Rust", "Clean Architecture", "DevSecOps", "Blazor" }
            };

            return View(viewModel);
        }
    }
}