// En Controllers/EmpresaPerfilController.cs
using Microsoft.AspNetCore.Mvc;
using IngeLab.Models;
using System.Collections.Generic;
using System;

namespace IngeLab.Controllers
{
    public class EmpresaPerfilController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new EmpresaPerfilViewModel
            {
                IsOwnProfile = true,
                Perfil = new Empresas
                {
                    Nombre = "TechSolutions S.A.",
                    // Suponiendo que 'Direccion' puede usarse como 'Industria' o similar
                    Direccion = "Software y Servicios en la Nube"
                },
                TechStack = new List<string> { ".NET", "Azure", "React", "SQL Server", "Microservicios", "Kubernetes" },
                OfertasAbiertas = new List<OfertaLaboral>
                {
                    new OfertaLaboral { Titulo = ".NET Developer Senior", Modalidad = "Remoto", Ubicacion = "LATAM" },
                    new OfertaLaboral { Titulo = "Cloud Architect (Azure)", Modalidad = "Híbrido", Ubicacion = "Medellín, CO" }
                },
                Posts = new List<Post>
                {
                    new Post { Id = 1, AutorNombre = "TechSolutions S.A.", Contenido = "¡Estamos contratando! Buscamos a los mejores...", FechaCreacion = DateTime.Now.AddDays(-5), IsPinned = true },
                    new Post { Id = 2, AutorNombre = "TechSolutions S.A.", Contenido = "Nuestro último caso de éxito con un cliente del sector financiero...", FechaCreacion = DateTime.Now.AddDays(-2), IsPinned = false }
                }
            };
            return View(viewModel);
        }
    }
}