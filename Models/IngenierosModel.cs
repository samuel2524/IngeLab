using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IngeLab.Models
{

    public class Ingenieros : Usuarios
    {

        public string Apellido { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime FechaNacimiento { get; set; }




        
        public void ControlDeErrores(ModelStateDictionary modelState)
        {
            bool soloLetrasNombre = !string.IsNullOrWhiteSpace(Nombre) && Nombre.All(c => char.IsLetter(c));
            bool soloNumerosDocumento = !string.IsNullOrWhiteSpace(NumeroDocumento) && NumeroDocumento.All(c => char.IsDigit(c));
            bool soloNumerosTelefono = !string.IsNullOrWhiteSpace(Telefono)&& Telefono.All(c => char.IsDigit(c));
            bool apellidoValido = Regex.IsMatch(Apellido, @"^[a-zA-Z\s]+$");
           


            if (string.IsNullOrWhiteSpace(Nombre))
            {
                modelState.AddModelError("Ingeniero.Nombre", "El nombre es obligatorio.");
            }
            if (!soloLetrasNombre)
            {
                modelState.AddModelError("Ingeniero.Nombre", "El nombre solo debe contener letras.");
            }

            if (string.IsNullOrWhiteSpace(Apellido))
            {
                modelState.AddModelError("Ingeniero.Apellido", "El apellido es obligatorio.");
            }
            if(!apellidoValido)
            {
                modelState.AddModelError("Ingeniero.Apellido", "El apellido solo debe contener letras y espacios.");
            }

            if (string.IsNullOrWhiteSpace(TipoDocumento))
            {
                modelState.AddModelError("Ingeniero.TipoDocumento", "El tipo de documento es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(NumeroDocumento))
            {
                modelState.AddModelError("Ingeniero.NumeroDocumento", "El número de documento es obligatorio.");
            }

            if (!soloNumerosDocumento)
            {
                modelState.AddModelError("Ingeniero.NumeroDocumento", "El número de documento solo debe contener números.");
            }

            if (string.IsNullOrWhiteSpace(Correo))
            {
                modelState.AddModelError("Ingeniero.Correo", "El correo es obligatorio.");
            }

            else if (!new EmailAddressAttribute().IsValid(Correo))
            {
                modelState.AddModelError("Ingeniero.Correo", "El correo no es válido.");
            }

            if (string.IsNullOrWhiteSpace(Contraseña))
            {
                modelState.AddModelError("Ingeniero.Contraseña", "La contraseña es obligatoria.");
            }
            else
            {
                bool contraseñaMayuscula = Contraseña.Any(c => char.IsUpper(c));
                bool caracteresEspeciales = Contraseña.Any(c => !char.IsLetterOrDigit(c));

                if (Contraseña.Length < 6)
                {
                    modelState.AddModelError("Ingeniero.Contraseña", "La contraseña debe tener al menos 6 caracteres.");
                }

                if (!contraseñaMayuscula)
                {
                    modelState.AddModelError("Ingeniero.Contraseña", "La contraseña debe contener al menos una letra mayúscula.");
                }
                if (!caracteresEspeciales)
                {
                    modelState.AddModelError("Ingeniero.Contraseña", "La contraseña debe contener al menos un carácter especial.");
                }

                
            }
           

          

            if (FechaNacimiento == default)
            {
                modelState.AddModelError("Ingeniero.FechaNacimiento", "La fecha de nacimiento es obligatoria.");
            }
            else if (FechaNacimiento > DateTime.Now)
            {
                modelState.AddModelError("Ingeniero.FechaNacimiento", "La fecha de nacimiento no puede ser en el futuro.");
            }

            if (string.IsNullOrWhiteSpace(Telefono))
            {
                modelState.AddModelError("Ingeniero.Telefono", "El teléfono es obligatorio.");
            }
            
            if (!soloNumerosTelefono)
            {
                modelState.AddModelError("Ingeniero.Telefono", "El teléfono solo debe contener números.");
            }
            else if (Telefono.Length < 10)
            {
                modelState.AddModelError("Ingeniero.Telefono", "El teléfono debe tener 10 dígitos.");
            }
        }

    }
    
}