// wwwroot/js/perfil.js

document.addEventListener('DOMContentLoaded', function () {

    const especialidadSelect = document.getElementById('especialidad');
    const otroContainer = document.getElementById('otro-especialidad-container');

    especialidadSelect.addEventListener('change', function () {
        if (this.value === 'otro') {
            otroContainer.classList.remove('hidden');
        } else {
            otroContainer.classList.add('hidden');
        }
    });
});