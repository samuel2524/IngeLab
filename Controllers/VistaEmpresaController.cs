using IngeLab.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace IngeLab.Controllers
{
    public class VistaEmpresaController : Controller
    {

        public IActionResult Index()
        {
            // Simulación de Ingenieros, luego se reemplaza con la BD
            var ingenierosDestacados = new List<Ingenieros>
            {
                new Ingenieros { Nombre = "Valeria", Apellido = "Rojas", Especializacion = "Ingeniería de Software", Habilidades_Tecnicas = "C#, .NET, Azure, Microservicios" },
                new Ingenieros { Nombre = "Mateo", Apellido = "García", Especializacion = "Ingeniería Civil", Habilidades_Tecnicas = "AutoCAD, Revit, Gestión de Proyectos" },
                new Ingenieros { Nombre = "Sofía", Apellido = "Restrepo", Especializacion = "Ingeniería Eléctrica", Habilidades_Tecnicas = "Sistemas de Potencia, PLC, Energías Renovables" },
                new Ingenieros { Nombre = "Daniel", Apellido = "Correa", Especializacion = "Ingeniería Mecánica", Habilidades_Tecnicas = "SolidWorks, ANSYS, Termodinámica" }
            };

            // Pasamos la lista de ingenieros a la vista.
            return View(ingenierosDestacados);
        }

    }
}