using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IngeLab.Models
{

    public class Empresas : Usuarios
    {

        public string NIT { get; set; }
        public string Direccion { get; set; }
        


        public void ControlDeErrores(ModelStateDictionary modelState)
        {
            bool soloNumerosNIT = !string.IsNullOrWhiteSpace(NIT) && NIT.All(c => char.IsDigit(c));
            bool soloNumerosTelefono = !string.IsNullOrWhiteSpace(Telefono) && Telefono.All(c => char.IsDigit(c));
    
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                modelState.AddModelError("Empresa.Nombre", "El nombre de la empresa es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(NIT))
            {
                modelState.AddModelError("Empresa.NIT", "El NIT es obligatorio.");
            }
            if (!soloNumerosNIT)
            {
                modelState.AddModelError("Empresa.NIT", "El NIT solo debe contener números.");
            }
            if(NIT.Length != 9)
            {
                modelState.AddModelError("Empresa.NIT", "El NIT debe tener 9 dígitos.");
            }

            if (string.IsNullOrWhiteSpace(Correo))
            {
                modelState.AddModelError("Empresa.Correo", "El correo es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(Contraseña))
            {
                modelState.AddModelError("Empresa.Contraseña", "La contraseña es obligatoria.");
            }
            else
            {
                bool contraseñaMayuscula = Contraseña.Any(c => char.IsUpper(c));
                List<string> caracteresEspeciales = new List<string>() { "!", "#", ",", "$", "%", "&", "/", "(", ")", "=", "?", "¡", "¿", "*", "+", "^", "-", "_", ";", "|", ".", "~" };
                bool contraseñaCaracterEspecial = Contraseña.Any(c => caracteresEspeciales.Contains(c.ToString()));

                if (Contraseña.Length < 6)
                {
                    modelState.AddModelError("Empresa.Contraseña", "La contraseña debe tener al menos 6 caracteres.");
                }
                if (!contraseñaMayuscula)
                {
                    modelState.AddModelError("Empresa.Contraseña", "La contraseña debe contener al menos una letra mayúscula.");
                }
                if (!contraseñaCaracterEspecial)
                {
                    modelState.AddModelError("Empresa.Contraseña", "La contraseña debe contener al menos un carácter especial.");
                }
            }
            
          
            if (string.IsNullOrWhiteSpace(Telefono))
            {
                modelState.AddModelError("Empresa.Telefono", "El teléfono es obligatorio.");
            }

            if (!soloNumerosTelefono)
            {
                modelState.AddModelError("Empresa.Telefono", "El teléfono solo debe contener números.");
            }
        }


    }

}