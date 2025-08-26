// Esperamos a que todo el contenido de la página se cargue antes de ejecutar el script.
document.addEventListener('DOMContentLoaded', function () {

    // Referencias a los elementos del DOM que vamos a manipular.
    const selectorTipoUsuario = document.getElementById('tipoUsuario');
    const camposIngeniero = document.getElementById('camposIngeniero');
    const camposEmpresa = document.getElementById('camposEmpresa');

    // Función para actualizar la visibilidad de los formularios.
    function actualizarFormulario() {
        const seleccion = selectorTipoUsuario.value;
        
        const inputsIngeniero = camposIngeniero.querySelectorAll("input, select");
        const inputsEmpresa = camposEmpresa.querySelectorAll("input, select");

        // "If" para controlar el flow.
        if (seleccion === 'ingeniero') {
            // Si es ingeniero, mostramos sus campos y ocultamos los de la empresa.
            camposIngeniero.classList.remove('hidden');
            camposEmpresa.classList.add('hidden');

            //desactivamos los inputs del formulario oculto para que no se envíen
            inputsIngeniero.forEach(el => el.disabled = false);
            inputsEmpresa.forEach(el => el.disabled = true);
        } else if (seleccion === 'empresa') {
            // Si es empresa, hacemos lo contrario.
            camposIngeniero.classList.add('hidden');
            camposEmpresa.classList.remove('hidden');

            //desactivamos los inputs del formulario oculto para que no se envíen
            inputsIngeniero.forEach(el => el.disabled = true);
            inputsEmpresa.forEach(el => el.disabled = false);
        }
    }

    // Añadimos un "event listener" que llama a nuestra función cada vez que el usuario cambia la selección.
    selectorTipoUsuario.addEventListener('change', actualizarFormulario);

    // Llamamos a la función una vez al cargar la página para asegurar el estado inicial correcto.
    actualizarFormulario();
});